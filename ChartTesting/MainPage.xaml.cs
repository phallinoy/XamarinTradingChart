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
        double[] height = { 60, 70, 50, 55, 65, 80, 70, 60, 50, 60 };

        ChartData[] chartDatas = new ChartData[5];
        //chartDatas[0] = new ChartData("2019 1/1 9:00", 101.12, 102.25, 100.23, 101.87, 245);

        SKPaint openCloseStick = new SKPaint
        {
            Style = SKPaintStyle.StrokeAndFill,
            Color = SKColors.Green,
            StrokeWidth = 2,
        };

        SKPaint textPaint = new SKPaint
        {
            Color = SKColors.Black,
            TextSize = 40
        };

        public MainPage()
        {
            InitializeComponent();

            List<View> views = new List<View>();
            // Create BoxView elements and add to List.
            for (int i = 0; i < COUNT; i++)
            {
                BoxView boxView = new BoxView
                {
                    Color = Color.Gray,
                    HeightRequest = height[i],
                    VerticalOptions = LayoutOptions.End
                };
                views.Add(boxView);
            }
            // Add whole List of BoxView elements to Grid.
            volume.Children.AddHorizontal(views);

            GetJsonData();
        }

        // Add OHLC chart
        private void canvasView_Ohlc(object sender, SKPaintSurfaceEventArgs e)
        {
            SKImageInfo info = e.Info;
            SKSurface surface = e.Surface;
            SKCanvas canvas = surface.Canvas;

            canvas.Clear(SKColors.White);

            canvas.DrawRect(0, 100, 360, 100, openCloseStick);

            
        }

        void GetJsonData()
        {
            string jsonFileName = "chart-legs.json";
            var assembly = typeof(MainPage).GetTypeInfo().Assembly;
            Stream stream = assembly.GetManifestResourceStream($"{assembly.GetName().Name}.{jsonFileName}");
            using (var reader = new System.IO.StreamReader(stream))
            {
                var jsonString = reader.ReadToEnd();
                JObject json = JObject.Parse(jsonString);

                var propLegList = json["LegList"];
                List<View> views = new List<View>();
                foreach (var prop in propLegList)
                {
                    System.Diagnostics.Debug.WriteLine("__subprop__" + prop[6]);

                }
            }
        }

    }

}
