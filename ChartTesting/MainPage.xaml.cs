using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using System.IO;
using ChartTesting.Model;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Runtime.CompilerServices;

namespace ChartTesting
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        JObject json;
        double valueX, valueY;
        bool IsTurnX, IsTurnY;
        double xInitial = 0, yInitial = 0;
        double xDistance = 0, yDistance = 0;
        int arrayCount;
        Array testList;

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

            //Testing
            _OpenPrice.Text = "O:" + ArrayStart;

            //Testing//

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

        void OnPanUpdated(object sender, PanUpdatedEventArgs args)
        {
            // PanGestureRecognizer
            var x = args.TotalX * 0.2; // TotalX Left/Right
            var y = args.TotalY * 0.2; // TotalY Up/Down
            //Console.WriteLine("___x = " + x);
            //Console.WriteLine("___y = " + y);

            // StatusType
            switch (args.StatusType)
            {
                case GestureStatus.Started:
                    Console.WriteLine("___Started");
                    break;
                case GestureStatus.Running:

                    // Check that the movement is x or y
                    if ((x >= 5 || x <= -5) && !IsTurnX && !IsTurnY)
                    {
                        IsTurnX = true;
                        Console.WriteLine("___isTurnX: " + x + ":" + y);
                    }

                    if ((y >= 5 || y <= -5) && !IsTurnY && !IsTurnX)
                    {
                        IsTurnY = true;
                        Console.WriteLine("___isTurnY: " + x + ":" + y);
                    }

                    // X (Horizontal)
                    if (IsTurnX && !IsTurnY)
                    {
                        if (x <= valueX)
                        {
                            // Left
                            if (ArrayStart + ArrayRange < arrayCount)
                            {
                                ArrayStart += 1;
                                Console.WriteLine("___left");
                            }
                        }

                        if (x >= valueX)
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
                        if (y <= valueY)
                        {
                            // Up
                            ArrayRange += 1;
                            Console.WriteLine("___up");
                        }

                        if (y >= valueY)
                        {
                            // Down
                            if (ArrayRange > 10)
                            {
                                ArrayRange -= 1;
                                Console.WriteLine("___down");
                            }
                        }
                    }

                    break;
                case GestureStatus.Completed:
                    Console.WriteLine("___Completed");

                    valueX = x;
                    valueY = y;

                    IsTurnX = false;
                    IsTurnY = false;

                    break;
            }
        }

        private void OnTouch(object sender, SKTouchEventArgs e)
        {
            //Console.WriteLine("hell");
            var x = e.Location.X * 0.1;
            var y = e.Location.Y * 0.1;

            // find direction
            switch (e.ActionType)
            {
                case SKTouchAction.Pressed:
                    Console.WriteLine("___pressed");
                    
                    xInitial = x;
                    yInitial = y;

                    // Display OHLC value
                    Console.WriteLine("___x:" + e.Location.X + " y:" + e.Location.Y);
                    Console.WriteLine("___count: " + testList.Length);
                    break;
                case SKTouchAction.Moved:
                    xDistance = x - xInitial;
                    yDistance = y - yInitial;

                    // Get scroll direction here
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

            var subLegList = legList.Skip(ArrayStart).Take(ArrayRange).ToArray();
            testList = subLegList;

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
                //float openCloseStickWidth = canvasWidth / legListNumber;
                float openCloseStickWidth = canvasWidth / ArrayRange;
                float volumeStickWidth = openCloseStickWidth;

                float highPrice = (float)item[2];
                float lowPrice = (float)item[3];
                float openPrice = (float)item[4];
                float closePrice = (float)item[5];
                float volume = 0;

                //testing
                var tapGestureRecognizer = new TapGestureRecognizer();
                tapGestureRecognizer.Tapped += (s, arg) =>
                {
                    // handle the tap
                    _OpenPrice.Text = openPrice.ToString();

                };
                ChartCanvasView.GestureRecognizers.Add(tapGestureRecognizer);
                //testing//

                // Scaling OHLC chart
                //
                float ohlcChartHeight = canvasHeight * (float)0.75;
                // Find the range between HighPrice and lowest (highRange)
                float highRange = highPrice - (float)lowest;
                // Find the range between LowPrice and lowest (lowRange)
                float lowRange = lowPrice - (float)lowest;
                // highPrice = (highRange / range) * chartHeight
                float scaleHighPrice = (highRange / range) * ohlcChartHeight;
                // lowPrice = (lowRange / range) * chartHeight 
                float scaleLowPrice = (lowRange / range) * ohlcChartHeight;
                // 
                // Find the range between Open and smallest (openRange)
                float openRange = openPrice - (float)lowest;
                // Find the range between Close and smallest (closeRange)
                float closeRange = closePrice - (float)lowest;
                // openPrice = (openRange / range) * chartHeight
                float scaleOpenPrice = (openRange / range) * ohlcChartHeight;
                // closePrice = (closeRange / range) * chartHeight
                float scaleClosePrice = (closeRange / range) * ohlcChartHeight;
                //

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
