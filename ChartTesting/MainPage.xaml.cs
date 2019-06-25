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

        const int COUNT = 10;
        Random random = new Random();
        double[] height = { 100, 70, 50, 55, 65, 80, 70, 60, 50, 60 };

        SKPaint openCloseStick = new SKPaint
        {
            Style = SKPaintStyle.StrokeAndFill,
            Color = SKColors.Green,
            StrokeWidth = 2,
            
        };

        SKPaint volume_bar_paint = new SKPaint
        {
            Style = SKPaintStyle.StrokeAndFill,
            Color = SKColors.Blue

        };

        SKPaint line_paint = new SKPaint
        {
            Color = SKColors.Blue,
            StrokeWidth = 2
        };

        SKPaint textPaint = new SKPaint
        {
            Color = SKColors.Black,
            TextSize = 40
        };

        public MainPage()
        {
            InitializeComponent();

            //GetJsonData();
        }

        // Add OHLC chart
        private void canvasView_Ohlc(object sender, SKPaintSurfaceEventArgs e)
        {
            SKImageInfo info = e.Info;
            SKSurface surface = e.Surface;
            SKCanvas canvas = surface.Canvas;

            canvas.Clear(SKColors.White);

            int width = info.Width;
            int height = info.Height;

            //set transform

            canvas.DrawRect(0, 0, 1080, 100, openCloseStick);
            canvas.DrawLine(0, 0, 0, 600, line_paint);

            // draw text testing 
            SKPaint paint = new SKPaint
            {
                Color = SKColors.Black,
                TextSize = 40
            };

            float fontSpacing = paint.FontSpacing;
            float x = 200;               // left margin
            float y = fontSpacing;      // first baseline
            float indent = 100;

            canvas.DrawText("SKCanvasView Height and Width:", x, y, paint);
            y += fontSpacing;
            canvas.DrawText(String.Format("{0:F2} x {1:F2}",
                                          volume_CanvasView.Width, volume_CanvasView.Height),
                            x + indent, y, paint);
            y += fontSpacing * 2;
            canvas.DrawText("SKCanvasView CanvasSize:", x, y, paint);
            y += fontSpacing;
            canvas.DrawText(volume_CanvasView.CanvasSize.ToString(), x + indent, y, paint);
            y += fontSpacing * 2;
            canvas.DrawText("SKImageInfo Size:", x, y, paint);
            y += fontSpacing;
            canvas.DrawText(info.Size.ToString(), x + indent, y, paint);
            // end draw text

        }

        private void canvasView_volume(object sender, SKPaintSurfaceEventArgs e)
        {
            SKImageInfo info = e.Info;
            SKSurface surface = e.Surface;
            SKCanvas canvas = surface.Canvas;

            canvas.Clear(SKColors.White);

            int width = info.Width;
            int height = info.Height;

            // Get JSON data
            string jsonFileName = "chart-legs.json";
            var assembly = typeof(MainPage).GetTypeInfo().Assembly;
            Stream stream = assembly.GetManifestResourceStream($"{assembly.GetName().Name}.{jsonFileName}");
            using (var reader = new System.IO.StreamReader(stream))
            {
                var jsonString = reader.ReadToEnd();
                JObject json = JObject.Parse(jsonString);

                var propLegList = json["LegList"];
                int volume_number = propLegList.Count();
                
                foreach (var prop in propLegList)
                {
                    float volume_canvas_height = volume_CanvasView.CanvasSize.Height;
                    float volume_canvas_width = volume_CanvasView.CanvasSize.Width;

                    float volume_bar_width = volume_canvas_width / volume_number;
                    float volume_bar_height = (float)prop[6] / 100;

                    float xTranslate = volume_bar_width;
                    float yTranslate = volume_canvas_height - volume_bar_height;

                    System.Diagnostics.Debug.WriteLine("__volume__" + volume_bar_height);
                    System.Diagnostics.Debug.WriteLine("__aaa-ytranslte__" + (yTranslate));

                    canvas.Translate(0, yTranslate);
                    canvas.DrawRect(0, 0, volume_bar_width, volume_bar_height, volume_bar_paint);
                    canvas.Translate(xTranslate, -yTranslate);
                    
                }

            }

        }

    }

}
