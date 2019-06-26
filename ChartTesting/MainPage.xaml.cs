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

namespace ChartTesting
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        JObject json;

        SKPaint ocPaintPos = new SKPaint
        {
            Style = SKPaintStyle.Fill,
            Color = SKColors.Green
        };

        SKPaint ocPaintNeg = new SKPaint
        {
            Style = SKPaintStyle.Fill,
            Color = SKColors.Red
        };

        SKPaint volumeBarPaintPos = new SKPaint
        {
            Style = SKPaintStyle.Fill,
            Color = SKColors.Gray
        };

        SKPaint volumeBarPaintNeg = new SKPaint
        {
            Style = SKPaintStyle.Fill,
            Color = SKColors.DarkGray
        };

        public MainPage()
        {

            InitializeComponent();
            GetJsonData();

        }

        void GetJsonData()
        {
            // Get JSON data
            string jsonFileName = "chart-legs.json";
            var assembly = typeof(MainPage).GetTypeInfo().Assembly;
            Stream stream = assembly.GetManifestResourceStream($"{assembly.GetName().Name}.{jsonFileName}");
            using (var reader = new System.IO.StreamReader(stream))
            {
                var jsonString = reader.ReadToEnd();
                json = JObject.Parse(jsonString);
            }
            return ;
        }

        // Add OHLC chart
        private void canvasView_Ohlc(object sender, SKPaintSurfaceEventArgs e)
        {
            // Get Canvas info
            SKImageInfo info = e.Info;
            SKSurface surface = e.Surface;
            SKCanvas canvas = surface.Canvas;

            canvas.Clear(SKColors.White);

            // Get LegList data from JSON
            var propLegList = json["LegList"];
            int legListNumber = propLegList.Count();

            // Plot the data in chart
            foreach (var prop in propLegList)
            {
                float ohlcCanvasHeight = ohlc_CanvasView.CanvasSize.Height;
                float ohlcCanvasWidth = ohlc_CanvasView.CanvasSize.Width;
                float candleStickWidth = ohlcCanvasWidth / legListNumber;
                float highPrice = (float)prop[2];
                float lowPrice = (float)prop[3];
                float openPrice = (float)prop[4];
                float closePrice = (float)prop[5];
                float xTranslate = candleStickWidth;
                float highLowHeight = highPrice - lowPrice;
                float ocCenter = candleStickWidth / 2;

                // Plot candleSticks
                // Compare openPrice and closePrice and plot them based on color
                if (openPrice <= closePrice)
                {
                    float openCloseHeight = (openPrice - closePrice);
                    canvas.DrawRect(ocCenter, ohlcCanvasHeight - highPrice, 2, highLowHeight, volumeBarPaintNeg);
                    canvas.DrawRect(0, ohlcCanvasHeight - closePrice, candleStickWidth, openCloseHeight, ocPaintPos);
                    canvas.Translate(xTranslate, 0);
                }
                else
                {
                    float openCloseHeight = (closePrice - openPrice);
                    canvas.DrawRect(ocCenter, ohlcCanvasHeight - highPrice, 2, highLowHeight, volumeBarPaintNeg);
                    canvas.DrawRect(0, ohlcCanvasHeight - openPrice, candleStickWidth, openCloseHeight, ocPaintNeg);
                    canvas.Translate(xTranslate, 0);
                }

                //System.Diagnostics.Debug.WriteLine("__open__" + prop[4]);
                //System.Diagnostics.Debug.WriteLine("__close__" + prop[5]);

            }

        }

        private void canvasView_volume(object sender, SKPaintSurfaceEventArgs e)
        {
            // Get Canvas info
            SKImageInfo info = e.Info;
            SKSurface surface = e.Surface;
            SKCanvas canvas = surface.Canvas;

            canvas.Clear(SKColors.White);

            // Get LegList data from JSON
            var propLegList = json["LegList"];
            int legListNumber = propLegList.Count();

            float volumeCanvasHeight = volume_CanvasView.CanvasSize.Height;
            float volumeCanvasWidth = volume_CanvasView.CanvasSize.Width;
            float volumeBarWidth = volumeCanvasWidth / legListNumber;

            // Plot the data in chart
            foreach (var prop in propLegList)
            {
                float volumeBarHeight = (float)prop[6] / 50;
                float xTranslate = volumeBarWidth;
                float yTranslate = volumeCanvasHeight - volumeBarHeight;

                // Plot Volume bar
                // Compare openPrice and closePrice and plot them based on color
                if ((float)prop[4] <= (float)prop[5])
                {
                    canvas.Translate(0, yTranslate);
                    canvas.DrawRect(0, 0, volumeBarWidth, volumeBarHeight, volumeBarPaintPos);
                    canvas.Translate(xTranslate, -yTranslate);
                } else
                {
                    canvas.Translate(0, yTranslate);
                    canvas.DrawRect(0, 0, volumeBarWidth, volumeBarHeight, volumeBarPaintNeg);
                    canvas.Translate(xTranslate, -yTranslate);
                }
                
                    
            }

        }

    }

}
