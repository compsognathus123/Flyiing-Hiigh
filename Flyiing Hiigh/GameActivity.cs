using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Timers;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using SkiaSharp;
using SkiaSharp.Views.Android;
using Android.Widget;


namespace Flyiing_Hiigh 
{
    public enum gameevent
    {
        GAME_STARTED,
        GAME_FINISHED,
        GAME_PAUSED,
        GAME_MUTED,
        WEAPON_SPAWNED,
        BIRD_SPAWNED,
        SPAWNING_BEES,
        SPAWNING_SPIDERS
    }

    [Activity(Label = "Game", ScreenOrientation = Android.Content.PM.ScreenOrientation.Landscape)]
    public class GameActivity : Activity
    {
        private Timer timer;
        private double tick_duration_ms;
        private double time;

        public Boolean muted;
        private Boolean godmode;

        private int theme;

        private SKCanvasView canvasView;
        private SKImageInfo imageInfo;

        private GameEventHandler eventHandler;
        private List<gameevent> archievedEvents;

        private List<GameObject> gameObjects;

        private ObjPlayer player;
        private ObjBackground background;
        private ObjOptions options;

        SKBitmap optionsbutton;

        public String infotext = "no Text";
        private SKTypeface typeface;

        private int score;

        public override void OnBackPressed()
        {
            Intent startActivityIntent = new Intent(this, typeof(StartActivity));
            StartActivity(startActivityIntent);
        }
        
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            RequestWindowFeature(WindowFeatures.NoTitle);
            Window.SetFlags(WindowManagerFlags.Fullscreen, WindowManagerFlags.Fullscreen);

            SetContentView(Resource.Layout.GameScreen);
            
            FindViewById<SKCanvasView>(Resource.Id.canvasView).Touch += OnScreenTouched;

            Random rnd = new Random();
            theme = rnd.Next(0, 4);

            tick_duration_ms = 5;

            godmode = false;

            startTimer();

            score = 0;

            canvasView = FindViewById<SKCanvasView>(Resource.Id.canvasView);
            canvasView.PaintSurface += OnCanvasViewPaintSurface;

            background = new ObjBackground(this, theme);
            player = new ObjPlayer(this);

            gameObjects = new List<GameObject>();
            gameObjects.Add(background);
            gameObjects.Add(player);

            archievedEvents = new List<gameevent>((int)gameevent.GAME_STARTED);
            eventHandler = new GameEventHandler(this, archievedEvents);

            using (var asset = Assets.Open("LittleBird.ttf"))
            {
                var fontStream = new MemoryStream();
                asset.CopyTo(fontStream);
                fontStream.Flush();
                fontStream.Position = 0;
                typeface = SKTypeface.FromStream(fontStream);
            }
            
            /*Assembly assembly = GetType().GetTypeInfo().Assembly;
            using (Stream stream = assembly.GetManifestResourceStream("Flyiing_Hiigh.Resources.Drawable.buttonoptions.png"))
            using (SKManagedStream skStream = new SKManagedStream(stream))
            {
                optionsbutton = SKBitmap.Decode(skStream);
            }*/

        }


        protected override void OnPause()
        {
            base.OnPause();
            timer.Enabled = false;
            background.stopBackgroundMusic();
        }

        public void setPause(Boolean pause)
        {
            if (pause)
            {
                RunOnUiThread(() =>
                {
                    options = new ObjOptions(this);
                    gameObjects.Add(options);
                    timer.Enabled = false;
                    archievedEvents.Add(gameevent.GAME_PAUSED);
                });

            }
            else
            {
                RunOnUiThread(() =>
                {
                    gameObjects.Remove(options);
                    timer.Enabled = true;
                    archievedEvents.Remove(gameevent.GAME_PAUSED);
                });
            }
        }

       
        private void OnCanvasViewPaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            imageInfo = e.Info;
            SKSurface surface = e.Surface;
            SKCanvas canvas = surface.Canvas;

            canvas.Clear();
                       
            foreach (GameObject obj in gameObjects)
            {
               obj.OnCanvasViewPaintSurface(e);
               // infotext += obj.getTyp() + " ";
            }


            //InfoText & heller Button
            SKPaint textPaint = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                TextAlign = SKTextAlign.Left,
                Typeface = typeface,
                Color = 0x7eFFFFFF,
                TextSize = 48,
            };

            // Dunkle Striche in Pausebutton
            SKPaint darkPaint = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                TextAlign = SKTextAlign.Left,
                Typeface = typeface,
                Color = 0x7e000000,
                TextSize = 48
                
            };


            canvas.DrawText(infotext, 20, 48, textPaint);
            canvas.DrawRect(imageInfo.Width * 0.93f, 0, imageInfo.Width*0.07f, imageInfo.Width * 0.07f,textPaint);
            canvas.DrawRect(imageInfo.Width * 0.945f, imageInfo.Width * 0.01f, imageInfo.Width*0.015f, imageInfo.Width * 0.05f, darkPaint);
            canvas.DrawRect(imageInfo.Width * 0.97f, imageInfo.Width * 0.01f, imageInfo.Width * 0.015f, imageInfo.Width * 0.05f, darkPaint);

        }


        private void OnScreenTouched(object sender, View.TouchEventArgs touchEventArgs)
        {
            if (touchEventArgs.Event.Action == MotionEventActions.Down)
            {
                if (!isPaused())
                {
                    if (touchEventArgs.Event.GetX() > (imageInfo.Width * 0.93f) && touchEventArgs.Event.GetY() < (imageInfo.Width * 0.07f))
                    {
                        setPause(true);
                    }
                    else if ((touchEventArgs.Event.GetX() > imageInfo.Width / 4) || player.getWeapon() == null)
                    {
                        player.accelerate();

                    }
                    else
                    {
                        RunOnUiThread(() =>
                        {
                            int x = (int)player.getWeapon().getRectangle().Right - 15;
                            int y = (int)player.getWeapon().getRectangle().MidY - 15;

                            gameObjects.Add(new ObjShot(this, x, y));

                            player.getWeapon().performShootingAnimation();
                            player.performShootingAnimation();

                        });
                    }
                }
                else
                {
                    if (touchEventArgs.Event.GetX() > (imageInfo.Width * 0.438f) && touchEventArgs.Event.GetX() < (imageInfo.Width * 0.4876f) && touchEventArgs.Event.GetY() > (imageInfo.Height * 0.5652f) && touchEventArgs.Event.GetY() < (imageInfo.Height * 0.6522f))
                    {
                        activateGodmode();
                    }
                    else {
                        setPause(false);
                    }
                }
            }
        }
               

        public void onPlayerDied(GameObject obj)
        {
            String death_reason;

            if(obj is ObjPlayer)
            {
                death_reason = "Smashed on ground";
            }
            else
            {
                death_reason = obj.getTyp();
            }

            archievedEvents.Add(gameevent.GAME_FINISHED);
            background.stopBackgroundMusic();
            timer.Stop();

            
                Intent endActivityIntent = new Intent(this, typeof(EndActivity));

                endActivityIntent.PutExtra("death_reason", death_reason);
                endActivityIntent.PutExtra("score", score);

                StartActivity(endActivityIntent);

        }

        private void moveGameObjects()
        {
            foreach (GameObject obj in gameObjects)
            {
                obj.move();
            }
        }

        private void checkForIntersects()
        {
            foreach (GameObject obj in gameObjects)
            {
                if (obj is ObjWeapon)
                {
                    ((ObjWeapon)obj).checkIntersect(player);
                }

                if (obj is ObjPlayer)
                {
                    if (godmode = false) { 
                        foreach (GameObject potentialIntersect in gameObjects)
                        {
                            ((ObjPlayer)obj).checkIntersect(potentialIntersect);
                        }
                    }
                }

                if (obj is ObjShot)
                {
                    foreach (GameObject potentialIntersect in gameObjects)
                    {
                        ((ObjShot)obj).checkIntersect(potentialIntersect);
                    }
                }
            }
        }

        private void startTimer()
        {
            timer = new Timer();
            timer.Interval = tick_duration_ms;
            timer.Elapsed += TimerTick;
            timer.Start();
        }

        private void TimerTick(object sender, ElapsedEventArgs args)
        {
            time += tick_duration_ms;

           infotext = (int)(time/500) + "m";
            // infotext = player.getRectangle().Width + " " + player.getRectangle().Height;

            RunOnUiThread(() =>
            {
                canvasView.Invalidate();
            });
                      
            checkForIntersects();
            eventHandler.checkEvents();
            moveGameObjects();

        }

        

        public Boolean isMuted()
        {
            return archievedEvents.Contains(gameevent.GAME_MUTED);
        }

        public Boolean isPaused()
        {
            return archievedEvents.Contains(gameevent.GAME_PAUSED);
        }

        public int getScore()
        {
            return score;
        }

        public void activateGodmode()
        {

            LayoutInflater layoutInflater = LayoutInflater.From(this);
            View view = layoutInflater.Inflate(Resource.Layout.InputDialog, null);

            Android.Support.V7.App.AlertDialog.Builder alertbuilder = new Android.Support.V7.App.AlertDialog.Builder(this);
            alertbuilder.SetView(view);

            String code = "";

            var input = view.FindViewById<EditText>(Resource.Id.editText);
            alertbuilder.SetCancelable(false)
            .SetPositiveButton("Choose", delegate
                 {
                        code = input.Text;
                        canvasView.Invalidate();
                  })
            .SetNegativeButton("Cancel", delegate
                   {
                        alertbuilder.Dispose();
                   });
             Android.Support.V7.App.AlertDialog dialog = alertbuilder.Create();
            dialog.Show();
            if (String.Compare(code, "godmode", true) == 0) setGodmode();
                        
        }

        public void setGodmode()
        {
            godmode = true;
        }

        public void increaseScore()
        {
            this.score++;
        }

        public Timer getTimer()
        {
            return timer;
        }

        public double getTickDurationMs()
        {
            return tick_duration_ms;
        }

        public SKCanvasView GetCanvasView()
        {
            return canvasView;
        }

        public SKImageInfo getImageInfo()
        {
            return imageInfo;
        }

        public ObjPlayer getPlayer()
        {
            return player;
        }

        public double getTime()
        {
            return time;
        }

        public void removeGameObject(GameObject obj)
        {
            RunOnUiThread(() =>
            {
                gameObjects.Remove(obj);
            });
        }
        

        public List<GameObject> getGameObjects()
        {
            return gameObjects;
        }

    }
}