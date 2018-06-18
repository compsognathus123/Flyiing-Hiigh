using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Content;
using System;
using System.IO;
using SkiaSharp;
using System.Reflection;
using SkiaSharp.Views.Android;
using Android.Widget;
using Android.Transitions;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Flyiing_Hiigh
{
    [Activity(MainLauncher = true, ScreenOrientation = Android.Content.PM.ScreenOrientation.Landscape, Icon = "@drawable/icon")]
    public class StartActivity : Activity
    {
        String username;
        int score;
        SKCanvasView canvasView;

        SKRect scoreButton;

        ISharedPreferences preferences;

        private SKBitmap backgroundBitmap;

        //Animation
        Stopwatch stopwatch = new Stopwatch();
        bool pageIsActive;
        float scale;

        public override void OnBackPressed()
        {
        }

        protected override void OnPause()
        {
            base.OnPause();
            pageIsActive = false;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            Window.SetFlags(WindowManagerFlags.Fullscreen, WindowManagerFlags.Fullscreen);

            base.OnCreate(savedInstanceState);
            RequestWindowFeature(WindowFeatures.NoTitle);

            Window.SharedElementExitTransition = new Fade();
            Window.ExitTransition = new Fade();
            Window.RequestFeature(WindowFeatures.ActivityTransitions);
            Window.RequestFeature(WindowFeatures.ContentTransitions);

            SetContentView(Resource.Layout.StartScreen);

            preferences = GetSharedPreferences("FlyingHigh", FileCreationMode.Private);

            loadPreferences();
            initializeStartScreen();

            pageIsActive = true;
            AnimationLoop();
        }

        private void loadPreferences()
        {
            username = preferences.GetString("username", "");
            score = preferences.GetInt("score", 0);

            if(username == "")
            {
                showInputDialog();
            }
        }

        private void showInputDialog()
        {
            LayoutInflater layoutInflater = LayoutInflater.From(this);
            View view = layoutInflater.Inflate(Resource.Layout.InputDialog, null);

            Android.Support.V7.App.AlertDialog.Builder alertbuilder = new Android.Support.V7.App.AlertDialog.Builder(this);
            alertbuilder.SetView(view);

            var userdata = view.FindViewById<EditText>(Resource.Id.editText);
            alertbuilder.SetCancelable(false)
            .SetPositiveButton("Choose", delegate
            {
                username = userdata.Text;
                preferences.Edit().PutString("username", username).Apply();
                canvasView.Invalidate();

            })
            .SetNegativeButton("Cancel", delegate
            {
                alertbuilder.Dispose();
            });
            Android.Support.V7.App.AlertDialog dialog = alertbuilder.Create();
            dialog.Show();
        }

        private void initializeStartScreen()
        {

            Assembly assembly = GetType().GetTypeInfo().Assembly;

            string resourceID = "Flyiing_Hiigh.Resources.Drawable.StartScreen.BackgroundName.png";
            using (Stream stream = assembly.GetManifestResourceStream(resourceID))
            using (SKManagedStream skStream = new SKManagedStream(stream))
            {
                backgroundBitmap = SKBitmap.Decode(skStream);
            }
       
            canvasView = FindViewById<SKCanvasView>(Resource.Id.canvasViewStartScreen);
            canvasView.PaintSurface += OnPaintCanvas;
            canvasView.Touch += OnCanvasTouch;
        }

        private void OnCanvasTouch(object sender, View.TouchEventArgs e)
        {
            if (e.Event.Action == MotionEventActions.Down)
            {
                if (scoreButton.IntersectsWith(new SKRect(e.Event.GetX() - 5, e.Event.GetY() - 5, e.Event.GetX() + 5, e.Event.GetY() + 5)))
                {
                    Intent scoreActivityIntent = new Intent(this, typeof(ScoreActivity));
                    scoreActivityIntent.SetFlags(ActivityFlags.SingleTop);
                    StartActivity(scoreActivityIntent);
                }
                else
                {
                    Intent gameActivityIntent = new Intent(this, typeof(GameActivity));
                    gameActivityIntent.SetFlags(ActivityFlags.SingleTop);
                    gameActivityIntent.SetFlags(ActivityFlags.ClearTop);
                    StartActivity(gameActivityIntent, ActivityOptions.MakeSceneTransitionAnimation(this).ToBundle());
                }
            }
        }

        private void OnPaintCanvas(object sender, SKPaintSurfaceEventArgs e)
        {
            SKImageInfo imageInfo = e.Info;
            SKSurface surface = e.Surface;
            SKCanvas canvas = surface.Canvas;
            canvas.Clear();

            canvas.DrawBitmap(backgroundBitmap, new SKRect(0, 0, imageInfo.Width, imageInfo.Height));

            SKTypeface typeface;
            using (var asset = Assets.Open("LittleBird.ttf"))
            {
                var fontStream = new MemoryStream();
                asset.CopyTo(fontStream);
                fontStream.Flush();
                fontStream.Position = 0;
                typeface = SKTypeface.FromStream(fontStream);
            }

            byte alpha = (byte)(100 + 155 * scale);
            SKPaint textPaint = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                Typeface = typeface,
                Color = SKColors.White.WithAlpha(alpha),
                TextAlign = SKTextAlign.Right,
                TextSize = 64
                
            };
            
            
            canvas.DrawText("tap Screen to start playing...", imageInfo.Width - 20, imageInfo.Height/6*5, textPaint);
                     

            SKPaint rectPaint = new SKPaint{Style = SKPaintStyle.Fill, Color = SKColors.Black.WithAlpha(100)};
            SKPaint scorePaint = new SKPaint {Color = SKColors.White, TextAlign = SKTextAlign.Right, TextSize = 32, Typeface = typeface };

            scoreButton = new SKRect(imageInfo.Width - scorePaint.MeasureText("Ranking") - 30,10, imageInfo.Width -10, 68);

            canvas.DrawRoundRect(scoreButton, 5,5, rectPaint);
            canvas.DrawText("Ranking", imageInfo.Width - 20, 48, scorePaint);

            if (username != "")
            {
                SKPaint usernamePaint = textPaint;
                usernamePaint.TextSize = 32;
                usernamePaint.Color = SKColors.Black;
                canvas.DrawText(username, scoreButton.Left - 20, 48, usernamePaint);
            }

        }

        async Task AnimationLoop()
        {
            stopwatch.Start();

            while (pageIsActive)
            {
                double cycleTime = 3;
                double t = stopwatch.Elapsed.TotalSeconds % cycleTime / cycleTime;
                scale = (1 + (float)Math.Sin(2 * Math.PI * t)) / 2;
                canvasView.Invalidate();
                await Task.Delay(TimeSpan.FromSeconds(1.0 / 30));
            }

            stopwatch.Stop();
        }

    }
}

