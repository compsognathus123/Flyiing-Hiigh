using Android.App;
using Android.OS;
using Android.Views;
using Android.Content;
using System;
using System.IO;
using SkiaSharp;
using System.Reflection;
using SkiaSharp.Views.Android;
namespace Flyiing_Hiigh
{
    [Activity(ScreenOrientation = Android.Content.PM.ScreenOrientation.Landscape)]
    public class EndActivity : Activity
    {
        private SKCanvasView canvasView;

        private int score;
        private String death_reason;

        public override void OnBackPressed()
        {
            Intent startActivityIntent = new Intent(this, typeof(StartActivity));
            StartActivity(startActivityIntent);
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            Window.SetFlags(WindowManagerFlags.Fullscreen, WindowManagerFlags.Fullscreen);

            base.OnCreate(savedInstanceState);
            RequestWindowFeature(WindowFeatures.NoTitle);
            SetContentView(Resource.Layout.EndScreen);
            
            score = Intent.Extras.GetInt("score");
            death_reason = Intent.Extras.GetString("death_reason");

            canvasView = FindViewById<SKCanvasView>(Resource.Id.canvasViewEndScreen);
            canvasView.PaintSurface += OnPaintCanvas;
            canvasView.Click += OnCanvasClicked;

        }

        private SKBitmap getBitmapFromID(String res)
        {
            Assembly assembly = GetType().GetTypeInfo().Assembly;
            SKBitmap bitmap;
            string resourceID = res;
            using (Stream stream = assembly.GetManifestResourceStream(resourceID))
            using (SKManagedStream skStream = new SKManagedStream(stream))
            {
                bitmap = SKBitmap.Decode(skStream);
            }

            return bitmap;
        }
            
       

        private void OnPaintCanvas(object sender, SKPaintSurfaceEventArgs e)
        {
            SKImageInfo imageInfo = e.Info;
            SKSurface surface = e.Surface;
            SKCanvas canvas = surface.Canvas;

            canvas.Clear();

            canvas.DrawBitmap(getBitmapFromID("Flyiing_Hiigh.Resources.Drawable.StartScreen.background.png"), new SKRect(0, 0, imageInfo.Width, imageInfo.Height));
            canvas.DrawBitmap(getBitmapFromID("Flyiing_Hiigh.Resources.Drawable.StartScreen.paper.png"), new SKRect(0, 0, imageInfo.Width, imageInfo.Height));

            drawText(canvas, imageInfo);

        }

        private void drawText(SKCanvas canvas, SKImageInfo imageInfo)
        {
            SKTypeface typeface;
            using (var asset = Assets.Open("LittleBird.ttf"))
            {
                var fontStream = new MemoryStream();
                asset.CopyTo(fontStream);
                fontStream.Flush();
                fontStream.Position = 0;
                typeface = SKTypeface.FromStream(fontStream);
            }

            String[] death_quote = { "Gone to a better place...", "Dropped dead!", "You bit the dust.", "Deleted!" , "Erased!", "REKT!" };
            Random rnd = new Random();


            using (var paint = new SKPaint())
            {
                paint.TextSize = 64.0f;
                paint.IsAntialias = true;
                paint.Color = (SKColor)0xFF000000;
                paint.IsStroke = false;
                paint.Typeface = typeface;
                paint.TextAlign = SKTextAlign.Center;

                canvas.DrawText(death_quote[rnd.Next(0,death_quote.Length - 1)], imageInfo.Width/2, imageInfo.Height/2 - 70, paint);
            }

            using (var paint = new SKPaint())
            {
                paint.TextSize = 48.0f;
                paint.IsAntialias = true;
                paint.Color = (SKColor)0xFF000000;
                paint.IsStroke = false;
                paint.Typeface = typeface;
                paint.TextAlign = SKTextAlign.Center;

                canvas.DrawText("Cause of death:", imageInfo.Width / 2, imageInfo.Height / 2 + 20, paint);
                canvas.DrawText("Score: ", imageInfo.Width / 2, imageInfo.Height / 2 + 120, paint);
            }

            using (var paint = new SKPaint())
            {
                paint.TextSize = 48.0f;
                paint.IsAntialias = true;
                paint.Color = (SKColor)0xFFbf0000;
                paint.IsStroke = false;
                paint.Typeface = typeface;
                paint.TextAlign = SKTextAlign.Center;

                String ScoreText = "error";
                ScoreText = score + " ECTS";
                canvas.DrawText(death_reason, imageInfo.Width / 2, imageInfo.Height / 2 + 72, paint);
                canvas.DrawText(ScoreText, imageInfo.Width / 2, imageInfo.Height / 2 + 172, paint);
            }
        }

        public void OnCanvasClicked(object sender, EventArgs e)
        {
            Intent mainActivityIntent = new Intent(this, typeof(StartActivity));
            StartActivity(mainActivityIntent);

        }
        

    }
}

