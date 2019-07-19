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

        public static readonly BindableProperty WidthTestProperty =
            BindableProperty.Create("WidthTest", typeof(float), typeof(MainPage), "");
        public float WidthTest
        {
            get { return (float)GetValue(WidthTestProperty); }
            set { SetValue(WidthTestProperty, value); }
        }

        public MainPage()
        {

            InitializeComponent();
            GetJsonData();

        }

        // Handle Swipe gestures
        void OnSwiped(object sender, SwipedEventArgs e)
        {
            switch (e.Direction)
            {
                case SwipeDirection.Left:
                    System.Console.WriteLine("___test___" + e.Direction.ToString());
                    WidthTest = 400;
                    break;
                case SwipeDirection.Right:
                    System.Console.WriteLine("___test___" + e.Direction.ToString());
                    break;
                case SwipeDirection.Up:
                    System.Console.WriteLine("___test___" + e.Direction.ToString());
                    break;
                case SwipeDirection.Down:
                    System.Console.WriteLine("___test___" + e.Direction.ToString());
                    break;
            }
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

            // testing
            //
            WidthTest = 100;
            canvas.DrawRect(100, 100, WidthTest, 300, openCloseStickPaint);
            //
            // testing //

            // Volume chart background
            float lineHeight = canvasHeight * (float)0.75;
            float lineWidth = canvasWidth;
            canvas.DrawLine(0, lineHeight, lineWidth, lineHeight, linePaint);

            // Get LegList data from JSON
            var propLegList = json["LegList"];
            int legListNumber = propLegList.Count();
            float xPointOpenClose = 0;
            float xPointHighLow = 0;

            // Find biggest volume
            float biggest = 0;

            foreach (var prop in propLegList)
            {
                if (prop[6].Type.ToString() != "Null")
                {
                    float volume = (float)prop[6];
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
            foreach (var prop in propLegList)
            {
                if ((float)prop[2] > highest)
                {
                    highest = (float)prop[2];
                }
                if (lowest == null || (float)prop[3] < lowest)
                {
                    lowest = (float)prop[3];
                }
            }
            range = highest - (float)lowest;

            // Plot the data in chart
            foreach (var prop in propLegList)
            {
                float openCloseStickWidth = canvasWidth / legListNumber;
                float volumeStickWidth = openCloseStickWidth;

                float highPrice = (float)prop[2];
                float lowPrice = (float)prop[3];
                float openPrice = (float)prop[4];
                float closePrice = (float)prop[5];
                float volume = 0;

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

                // Scaling OHLC chart //

                // Plot OHLC chart
                canvas.DrawRect(xPointHighLow - 1, yPointHighLow, 2, highLowHeight, volumeBarPaint);
                canvas.DrawRect(xPointOpenClose, yPointOpenClose, openCloseStickWidth, openCloseHeight, openCloseStickPaint);

                // Plot Volume chart
                // Handle null value
                float scaling = ((canvasHeight * (float)0.25) / biggest);

                if (prop[6].Type.ToString() != "Null")
                {
                    volume = (float)prop[6] * scaling;
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
            if (propertyName == WidthTestProperty.PropertyName)
            {
                //this.InvalidateSurface();
            }
        }

    }

}
