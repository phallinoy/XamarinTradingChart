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

        public MainPage()
        {

            InitializeComponent();
            GetJsonData();

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

            // Get LegList data from JSON
            var propLegList = json["LegList"];
            int legListNumber = propLegList.Count();
            float xPointOpenClose = 0;
            float xPointHighLow = 0;

            // Testing
            float biggest = 0;

            foreach (var prop in propLegList)
            {
                float volume = (float)prop[6];
                if(volume > biggest)
                {
                    biggest = volume;
                }
            }

            foreach (var prop in propLegList)
            {
                float volume = (float)prop[6];
                if (volume > biggest)
                {
                    biggest = volume;
                }
            }

            System.Console.WriteLine("__biggest__" + biggest);

            float scaling = ((canvasHeight/2)/biggest);

            System.Console.WriteLine("__abc__" + scaling);

            // Testing //

            // Plot the data in chart
            foreach (var prop in propLegList)
            {
                float openCloseStickWidth = canvasWidth / legListNumber;
                float volumeStickWidth = openCloseStickWidth;

                float highPrice = (float)prop[2];
                float lowPrice = (float)prop[3];
                float openPrice = (float)prop[4];
                float closePrice = (float)prop[5];
                float yPointHighLow = (canvasHeight / 2) - highPrice;
                float openCloseHeight = System.Math.Abs(openPrice - closePrice);
                float highLowHeight = highPrice - lowPrice;
                float yPointOpenClose = 0;
                xPointHighLow += openCloseStickWidth / 2;

                if (openPrice <= closePrice)
                {
                    yPointOpenClose = (canvasHeight / 2) - closePrice;
                    openCloseStickPaint.Color = SKColors.Green;
                    volumeBarPaint.Color = SKColors.Gray;
                }
                else
                {
                    yPointOpenClose = (canvasHeight / 2) - openPrice;
                    openCloseStickPaint.Color = SKColors.Red;
                    volumeBarPaint.Color = SKColors.DarkGray;
                }

                // Plot OHLC chart
                canvas.DrawRect(xPointHighLow - 1, yPointHighLow, 2, highLowHeight, volumeBarPaint);
                canvas.DrawRect(xPointOpenClose, yPointOpenClose, openCloseStickWidth, openCloseHeight, openCloseStickPaint);

                // Plot Volume chart
                // The value of volume multiply with scaling to reduce the height of volume to fit half of the screen.
                float volume = (float)prop[6] * scaling;
                float yPointVolume = canvasHeight - volume;
                canvas.DrawRect(xPointOpenClose, yPointVolume, volumeStickWidth, volume, volumeBarPaint);

                xPointOpenClose += openCloseStickWidth;
                xPointHighLow += openCloseStickWidth / 2;

                //System.Diagnostics.Debug.WriteLine("__test__" + xPointHighLow);
                //System.Diagnostics.Debug.WriteLine("__test__" + xPointOpenClose);

            }

        }

    }

}
