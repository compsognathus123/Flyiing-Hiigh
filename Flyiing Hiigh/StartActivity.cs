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
using Android.Preferences;
using Android.Widget;

namespace Flyiing_Hiigh
{
    [Activity(MainLauncher = true, ScreenOrientation = Android.Content.PM.ScreenOrientation.Landscape, Icon = "@drawable/icon")]
    public class StartActivity : Activity
    {
        String username;
        int score;
        SKCanvasView canvasView;

        ISharedPreferences preferences;

        private SKBitmap backgroundBitmap;
        //private SKBitmap bgshroomBitmap;
        //private SKBitmap playbuttonBitmap;
        
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

            SetContentView(Resource.Layout.StartScreen);

            preferences  = GetSharedPreferences("FlyingHigh", FileCreationMode.Private);

            initializeStartScreen();
            loadPreferences();
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

           /* string resourceID1 = "Flyiing_Hiigh.Resources.Drawable.StartScreen.buttonplay.png";
            using (Stream stream = assembly.GetManifestResourceStream(resourceID1))
            using (SKManagedStream skStream = new SKManagedStream(stream))
            {
                playbuttonBitmap = SKBitmap.Decode(skStream);
            }

            string resourceID2 = "Flyiing_Hiigh.Resources.Drawable.StartScreen.mainbg.png";
            using (Stream stream = assembly.GetManifestResourceStream(resourceID2))
            using (SKManagedStream skStream = new SKManagedStream(stream))
            {
                bgshroomBitmap = SKBitmap.Decode(skStream);
            }*/

            canvasView = FindViewById<SKCanvasView>(Resource.Id.canvasViewStartScreen);
            canvasView.PaintSurface += OnPaintCanvas;
            canvasView.Click += startButtonClicked;
        }

       

        private void OnPaintCanvas(object sender, SKPaintSurfaceEventArgs e)
        {
            SKImageInfo imageInfo = e.Info;
            SKSurface surface = e.Surface;
            SKCanvas canvas = surface.Canvas;
            canvas.Clear();

            canvas.DrawBitmap(backgroundBitmap, new SKRect(0, 0, imageInfo.Width, imageInfo.Height));
            //canvas.DrawBitmap(bgshroomBitmap, new SKRect(0, 0, imageInfo.Width, imageInfo.Height));

            // canvas.DrawBitmap(playbuttonBitmap, new SKRect(imageInfo.Width/2 - 75, imageInfo.Height/2 - 75, imageInfo.Width/2 + 75, imageInfo.Height/2 + 75));


            SKTypeface typeface;
            using (var asset = Assets.Open("LittleBird.ttf"))
            {
                var fontStream = new MemoryStream();
                asset.CopyTo(fontStream);
                fontStream.Flush();
                fontStream.Position = 0;
                typeface = SKTypeface.FromStream(fontStream);
            }

            SKPaint textPaint = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                Typeface = typeface,
                Color = SKColors.White,
                TextAlign = SKTextAlign.Right,
                TextSize = 64
            };

            String text = "tap Screen to start playing...";

            canvas.DrawText(text, imageInfo.Width - 20, imageInfo.Height/6*5, textPaint);
            if(username != "")
            {
                textPaint.TextSize = 32;
                textPaint.Color = SKColors.Black;
                canvas.DrawText(username, imageInfo.Width - 20, 48, textPaint);
            }

        }

        public void startButtonClicked(object sender, EventArgs e)
        {
            Intent gameActivityIntent = new Intent(this, typeof(GameActivity));
            StartActivity(gameActivityIntent);

        }
        

    }
}

