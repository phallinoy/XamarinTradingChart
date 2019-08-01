using System;
using System.ComponentModel;
using System.Linq;
using Xamarin.Forms;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using System.IO;
using System.Reflection;
using Newtonsoft.Json.Linq;

namespace ChartTesting
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        JToken json;
        JToken[] subLegList;
        double valueX, valueY;
        bool IsTurnX, IsTurnY;
        double xInitial = 0, yInitial = 0;
        double xDistance = 0, yDistance = 0;
        int arrayCount;
        
        SKPaint openCloseStickPaint = new SKPaint
        {
            Style = SKPaintStyle.Fill,
            Color = SKColors.Green
        };

        SKPaint volumeBarPaint = new SKPaint
        {
            Style = SKPaintStyle.Fill,
            Color = SKColors.Gray
        };

        SKPaint linePaint = new SKPaint
        {
            Style = SKPaintStyle.Fill,
            Color = SKColors.Gray
        };

        // Create BindableProperty
        public static readonly BindableProperty ArrayStartProperty =
            BindableProperty.Create("ArrayStart", typeof(int), typeof(MainPage), null);
        public int ArrayStart
        {
            get { return (int)GetValue(ArrayStartProperty); }
            set { SetValue(ArrayStartProperty, value); }
        }

        public static readonly BindableProperty ArrayRangeProperty =
            BindableProperty.Create("ArrayRange", typeof(int), typeof(MainPage), null);
        public int ArrayRange
        {
            get { return (int)GetValue(ArrayRangeProperty); }
            set { SetValue(ArrayRangeProperty, value); }
        }

        public MainPage()
        {
            InitializeComponent();
            GetJsonData();

            // Set default array index
            ArrayStart = 0;
            ArrayRange = 30;

        }

        // Get JSON data
        void GetJsonData()
        {
            string jsonFileName = "chart-legs.json";
            var assembly = typeof(MainPage).GetTypeInfo().Assembly;
            Stream stream = assembly.GetManifestResourceStream($"{assembly.GetName().Name}.{jsonFileName}");
            using (var reader = new System.IO.StreamReader(stream))
            {
                var jsonString = reader.ReadToEnd();
                json = JObject.Parse(jsonString);
            }
            return;
        }

        private void OnTouch(object sender, SKTouchEventArgs e)
        {
            var x = e.Location.X * 0.1;
            var y = e.Location.Y * 0.1;

            // find direction
            switch (e.ActionType)
            {
                case SKTouchAction.Pressed:

                    xInitial = x;
                    yInitial = y;

                    // Handle assign OHLC value to UI
                    float canvasWidth = ChartCanvasView.CanvasSize.Width;
                    float candleStart = 0;
                    float candleWidth = canvasWidth / ArrayRange;
                    float candleEnd = candleWidth;

                    int i = 0;
                    int index = ArrayStart;

                    // Find which rectangle was pressed
                    for (var a = 0; a < ArrayRange; a++)
                    {
                        if (e.Location.X >= candleStart && e.Location.X < candleEnd)
                        {
                            Console.WriteLine("___pressed on: " + index);
                            break;
                        }

                        index++;
                        candleStart += candleWidth;
                        candleEnd += candleWidth;
                    }

                    // Assign OHLC value
                    var legList = json["LegList"];

                    foreach (var item in legList)
                    {
                        if (i == index)
                        {
                            _OpenPrice.Text = "O:" + item[4].ToString();
                            _HighPrice.Text = "H:" + item[2].ToString();
                            _LowPrice.Text = "L:" + item[3].ToString();
                            _ClosePrice.Text = "C:" + item[5].ToString();

                            break;
                        }

                        i++;
                    }

                    break;
                case SKTouchAction.Moved:
                    // Handle scroll left, right, up, down
                    xDistance = x - xInitial;
                    yDistance = y - yInitial;

                    // Check that the movement is x or y
                    if ((xDistance >= 5 || xDistance <= -5) && !IsTurnX && !IsTurnY)
                    {
                        IsTurnX = true;
                    }

                    if ((yDistance >= 5 || yDistance <= -5) && !IsTurnY && !IsTurnX)
                    {
                        IsTurnY = true;
                    }

                    // X (Horizontal)
                    if (IsTurnX && !IsTurnY)
                    {
                        if (xDistance <= valueX)
                        {
                            // Left
                            if (ArrayStart + ArrayRange < arrayCount)
                            {
                                ArrayStart += 1;
                                Console.WriteLine("___left");
                            }
                        }

                        if (xDistance >= valueX)
                        {
                            // Right
                            if (ArrayStart >= 1)
                            {
                                ArrayStart -= 1;
                                Console.WriteLine("___right");
                            }
                        }
                    }

                    // Y (Vertical)
                    if (IsTurnY && !IsTurnX)
                    {
                        if (yDistance <= valueY)
                        {
                            // Up
                            ArrayRange += 1;
                            Console.WriteLine("___up");
                        }

                        if (yDistance >= valueY)
                        {
                            // Down
                            if (ArrayRange > 10)
                            {
                                ArrayRange -= 1;
                                Console.WriteLine("___down");
                            }
                        }
                    }
                    // End find direction

                    break;
                case SKTouchAction.Released:
                    Console.WriteLine("___released");

                    IsTurnX = false;
                    IsTurnY = false;
                    break;
            }

            e.Handled = true;
            // update the UI on the screen
            ((SKCanvasView)sender).InvalidateSurface();
        }

        // Plot chart
        private void ChartCanvas_PaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {

            // Get Canvas info
            SKImageInfo info = e.Info;
            SKSurface surface = e.Surface;
            SKCanvas canvas = surface.Canvas;

            canvas.Clear(SKColors.White);

            float canvasHeight = ChartCanvasView.CanvasSize.Height;
            float canvasWidth = ChartCanvasView.CanvasSize.Width;

            // Volume chart background
            float lineHeight = canvasHeight * (float)0.75;
            float lineWidth = canvasWidth;
            canvas.DrawLine(0, lineHeight, lineWidth, lineHeight, linePaint);

            // Get LegList data from JSON
            var legList = json["LegList"];
            arrayCount = legList.Count();

            // SubLegList data to display on screen
            subLegList = legList.Skip(ArrayStart).Take(ArrayRange).ToArray();

            float xPointOpenClose = 0;
            float xPointHighLow = 0;

            // Find biggest volume
            float biggest = 0;

            foreach (var item in subLegList)
            {
                if (item[6].Type.ToString() != "Null")
                {
                    float volume = (float)item[6];
                    if (volume > biggest)
                    {
                        biggest = volume;
                    }
                }
            }

            // Find biggest & smallest number among High & Low
            // Find the range between biggest and smallest (range)
            float highest = 0;
            Nullable<float> lowest = null;
            float range = 0;
            foreach (var item in subLegList)
            {
                if ((float)item[2] > highest)
                {
                    highest = (float)item[2];
                }
                if (lowest == null || (float)item[3] < lowest)
                {
                    lowest = (float)item[3];
                }
            }
            range = highest - (float)lowest;

            // Plot the data in chart
            foreach (var item in subLegList)
            {
                float openCloseStickWidth = canvasWidth / ArrayRange;
                float volumeStickWidth = openCloseStickWidth;

                float highPrice = (float)item[2];
                float lowPrice = (float)item[3];
                float openPrice = (float)item[4];
                float closePrice = (float)item[5];
                float volume = 0;

                // Scaling OHLC chart
                
                float ohlcChartHeight = canvasHeight * (float)0.75;

                // Find the range between HighPrice and lowest (highRange)
                float highRange = highPrice - (float)lowest;
                // Find the range between LowPrice and lowest (lowRange)
                float lowRange = lowPrice - (float)lowest;
                // highPrice = (highRange / range) * chartHeight
                float scaleHighPrice = (highRange / range) * ohlcChartHeight;
                // lowPrice = (lowRange / range) * chartHeight 
                float scaleLowPrice = (lowRange / range) * ohlcChartHeight;
                
                // Find the range between Open and smallest (openRange)
                float openRange = openPrice - (float)lowest;
                // Find the range between Close and smallest (closeRange)
                float closeRange = closePrice - (float)lowest;
                // openPrice = (openRange / range) * chartHeight
                float scaleOpenPrice = (openRange / range) * ohlcChartHeight;
                // closePrice = (closeRange / range) * chartHeight
                float scaleClosePrice = (closeRange / range) * ohlcChartHeight;

                float yPointHighLow = ohlcChartHeight - scaleHighPrice;
                float openCloseHeight = System.Math.Abs(scaleOpenPrice - scaleClosePrice);
                float highLowHeight = scaleHighPrice - scaleLowPrice;
                float yPointOpenClose = 0;
                xPointHighLow += openCloseStickWidth / 2;

                if (scaleOpenPrice <= scaleClosePrice)
                {
                    yPointOpenClose = ohlcChartHeight - scaleClosePrice;
                    openCloseStickPaint.Color = SKColors.Green;
                    volumeBarPaint.Color = SKColors.Gray;
                }
                else
                {
                    yPointOpenClose = ohlcChartHeight - scaleOpenPrice;
                    openCloseStickPaint.Color = SKColors.Red;
                    volumeBarPaint.Color = SKColors.DarkGray;
                }

                // Plot OHLC chart
                canvas.DrawRect(xPointHighLow - 1, yPointHighLow, 2, highLowHeight, volumeBarPaint);
                canvas.DrawRect(xPointOpenClose, yPointOpenClose, openCloseStickWidth, openCloseHeight, openCloseStickPaint);

                // Plot Volume chart
                // Handle null value
                float scaling = ((canvasHeight * (float)0.25) / biggest);

                if (item[6].Type.ToString() != "Null")
                {
                    volume = (float)item[6] * scaling;
                }
                else
                {
                    volume = 0;
                }

                float yPointVolume = canvasHeight - volume;
                canvas.DrawRect(xPointOpenClose, yPointVolume, volumeStickWidth, volume, volumeBarPaint);

                xPointOpenClose += openCloseStickWidth;
                xPointHighLow += openCloseStickWidth / 2;

            }

        }

        protected override void OnPropertyChanged(string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);
            if (propertyName == ArrayStartProperty.PropertyName ||
                propertyName == ArrayRangeProperty.PropertyName)
            {
                ChartCanvasView.InvalidateSurface();
            }
        }

    }

}
