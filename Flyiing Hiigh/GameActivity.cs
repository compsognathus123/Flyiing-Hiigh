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

            startTimer();

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
            
            Assembly assembly = GetType().GetTypeInfo().Assembly;
            using (Stream stream = assembly.GetManifestResourceStream("Flyiing_Hiigh.Resources.Drawable.buttonoptions.png"))
            using (SKManagedStream skStream = new SKManagedStream(stream))
            {
                optionsbutton = SKBitmap.Decode(skStream);
            }

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


            //InfoText
            SKPaint textPaint = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                TextAlign = SKTextAlign.Left,
                Typeface = typeface,
                Color = SKColors.White,
                TextSize = 48
            };


            canvas.DrawText(infotext, 20, 48, textPaint);
            canvas.DrawBitmap(optionsbutton, new SKRect(imageInfo.Width*0.93f, 0, imageInfo.Width, imageInfo.Width *0.07f));

        }


        private void OnScreenTouched(object sender, View.TouchEventArgs touchEventArgs)
        {
            if (touchEventArgs.Event.Action == MotionEventActions.Down && !isPaused())
            {
                if (touchEventArgs.Event.GetX() > (imageInfo.Width*0.93f) && touchEventArgs.Event.GetY() < (imageInfo.Width*0.07f) )
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
            }else if (touchEventArgs.Event.Action == MotionEventActions.Down && isPaused())
            {
                setPause(false);
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

            endActivityIntent.PutExtra("score", getScore());
            endActivityIntent.PutExtra("death_reason", death_reason);

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
                    foreach (GameObject potentialIntersect in gameObjects)
                    {
                        ((ObjPlayer)obj).checkIntersect(potentialIntersect);
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
            if (time % 500 == 499) increaseScore();

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

        public void increaseScore()
        {
            score++;
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