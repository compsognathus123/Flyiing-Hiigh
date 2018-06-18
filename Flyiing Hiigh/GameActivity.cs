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
using Android.Transitions;

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
        //++++++++++++++++++++Variables+++++++++++++++++++++
        private Timer timer;
        private double tick_duration_ms;
        private double time;
        
        public Boolean godmode;

        private int theme;

        private SKCanvasView canvasView;
        private SKImageInfo imageInfo;

        private GameEventHandler eventHandler;
        private List<gameevent> archievedEvents;

        private List<GameObject> gameObjects;

        private ObjPlayer player;
        private ObjBackground background;
        private ObjOptions options;
        
        public String infotext = "no Text";
        private SKTypeface typeface;

        private int score;
        

        //************************************BUTTONS/FRAMEWORK******************************
        public override void OnBackPressed()
        {
            if (isPaused())
            {
                setPause(false);
            }
            else
            {
                background.getAudiomanger().stopPlaying();
                timer.Stop();
                archievedEvents.Add(gameevent.GAME_FINISHED);

                Intent startActivityIntent = new Intent(this, typeof(StartActivity));
                startActivityIntent.AddFlags(ActivityFlags.SingleTop);
                StartActivity(startActivityIntent);

                Finish();
            }
        }

                
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            RequestWindowFeature(WindowFeatures.NoTitle);
            Window.SetFlags(WindowManagerFlags.Fullscreen, WindowManagerFlags.Fullscreen);

            Window.AllowEnterTransitionOverlap = true;
            Window.EnterTransition = new Fade();
            Window.SharedElementEnterTransition = new Fade();
            Window.RequestFeature(WindowFeatures.ContentTransitions);
            
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
            

        }


        protected override void OnPause()
        {
            base.OnPause();
            if (!archievedEvents.Contains(gameevent.GAME_FINISHED))
            {
                setPause(true);
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
                Color = 0x96FFFFFF,
                TextSize = 48,
            };

            canvas.DrawText(infotext, 20, 48, textPaint);
            canvas.DrawRoundRect(imageInfo.Width * 0.945f, imageInfo.Width * 0.01f, imageInfo.Width*0.015f, imageInfo.Width * 0.05f, 5, 5, textPaint);
            canvas.DrawRoundRect(imageInfo.Width * 0.97f, imageInfo.Width * 0.01f, imageInfo.Width * 0.015f, imageInfo.Width * 0.05f, 5, 5, textPaint);

        }

       
        //>>>>>>>>>>>>>>>>>>>>>>TOUCHEVENTS<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<

        private void OnScreenTouched(object sender, View.TouchEventArgs touchEventArgs)
        {
            int pointerIndex = touchEventArgs.Event.ActionIndex;

            if(touchEventArgs.Event.Action == MotionEventActions.Up || touchEventArgs.Event.Action == MotionEventActions.Pointer2Up)
            {
                if (!isPaused())
                {
                    if ((touchEventArgs.Event.GetX(pointerIndex) > imageInfo.Width / 4) || player.getWeapon() == null)
                    {
                        player.accelerate(false);

                    }
                }
            }


            if (touchEventArgs.Event.Action == MotionEventActions.Down || touchEventArgs.Event.Action == MotionEventActions.Pointer2Down)
            {
                if (!isPaused())
                {
                    if (touchEventArgs.Event.GetX(pointerIndex) > (imageInfo.Width * 0.93f) && touchEventArgs.Event.GetY(pointerIndex) < (imageInfo.Width * 0.07f))
                    {
                        setPause(true);
                    }
                    else if ((touchEventArgs.Event.GetX(pointerIndex) > imageInfo.Width / 4) || player.getWeapon() == null)
                    {
                        player.accelerate(true);

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
                    if (touchEventArgs.Event.GetX(pointerIndex) > (imageInfo.Width * 0.438f) && touchEventArgs.Event.GetX(pointerIndex) < (imageInfo.Width * 0.4876f) && touchEventArgs.Event.GetY(pointerIndex) > (imageInfo.Height * 0.5652f) && touchEventArgs.Event.GetY(pointerIndex) < (imageInfo.Height * 0.6522f))
                    {
                        activateGodmode();
                    }
                    else if(touchEventArgs.Event.GetX(pointerIndex) > (imageInfo.Width * 0.5041f) && touchEventArgs.Event.GetX(pointerIndex) < (imageInfo.Width * 0.5537f) && touchEventArgs.Event.GetY(pointerIndex) > (imageInfo.Height * 0.5652f) && touchEventArgs.Event.GetY(pointerIndex) < (imageInfo.Height * 0.6522f))
                    {
                        toggleMuted();
                    }
                    else if (touchEventArgs.Event.GetX(pointerIndex) > (imageInfo.Width * 0.3719f) && touchEventArgs.Event.GetX(pointerIndex) < (imageInfo.Width * 0.4876f) && touchEventArgs.Event.GetY(pointerIndex) > (imageInfo.Height * 0.4493f) && touchEventArgs.Event.GetY(pointerIndex) < (imageInfo.Height * 0.5217f))
                    {
                        setPause(false);
                    }
                    else if (touchEventArgs.Event.GetX(pointerIndex) > (imageInfo.Width * 0.5041f) && touchEventArgs.Event.GetX(pointerIndex) < (imageInfo.Width * 0.6281f) && touchEventArgs.Event.GetY(pointerIndex) > (imageInfo.Height * 0.4493f) && touchEventArgs.Event.GetY(pointerIndex) < (imageInfo.Height * 0.5217f))
                    {
                        OnBackPressed();
                    }
                    else if (touchEventArgs.Event.GetX(pointerIndex) > (imageInfo.Width * 0.93f) && touchEventArgs.Event.GetY(pointerIndex) < (imageInfo.Width * 0.07f))
                    {
                        setPause(false);
                    }

                }
            }
        }
               
        //>>>>>>>>>>>>>>>>>>>>>>>>>>>><<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<


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
            background.getAudiomanger().stopPlaying();
            timer.Stop();
            
            Intent endActivityIntent = new Intent(this, typeof(EndActivity));

            endActivityIntent.PutExtra("death_reason", death_reason);
            endActivityIntent.PutExtra("score", score);
            endActivityIntent.SetFlags(ActivityFlags.SingleTop);

            StartActivity(endActivityIntent);
            Finish();
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
                    if (godmode == false) { 
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

            if ((time % 5000) == 4999) increaseScore();

            RunOnUiThread(() =>
            {
                canvasView.Invalidate();
            });
                      
            checkForIntersects();
            eventHandler.checkEvents();
            moveGameObjects();

        }

       
        public void toggleMuted()
        {
            if (isMuted())
            {
                if(!isPaused()) background.getAudiomanger().Resume();

                RunOnUiThread(() =>
                {
                    archievedEvents.Remove(gameevent.GAME_MUTED);
                });
            }
            else
            {
                background.getAudiomanger().Pause();
                RunOnUiThread(() =>
                {
                    archievedEvents.Add(gameevent.GAME_MUTED);
                });
            }
        }

        public Boolean isMuted()
        {
            return archievedEvents.Contains(gameevent.GAME_MUTED);
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
                    background.getAudiomanger().Pause();
                });
            }
            else
            {
                RunOnUiThread(() =>
                {
                    gameObjects.Remove(options);
                    timer.Enabled = true;
                    archievedEvents.Remove(gameevent.GAME_PAUSED);
                    if (!isMuted()) background.getAudiomanger().Resume();
                });
            }
        }

        public Boolean isPaused()
        {
            return archievedEvents.Contains(gameevent.GAME_PAUSED);
        }

        public int getScore()
        {
            return score;
        }

        public Boolean getGodmode()
        {
            return godmode;
        }

        public void activateGodmode()
        {
            LayoutInflater layoutInflater = LayoutInflater.From(this);
            View view = layoutInflater.Inflate(Resource.Layout.GiftCode, null);

            Android.Support.V7.App.AlertDialog.Builder alertbuilder = new Android.Support.V7.App.AlertDialog.Builder(this);
            alertbuilder.SetView(view);

            String code = "";

            var input = view.FindViewById<EditText>(Resource.Id.editText);
            alertbuilder.SetCancelable(false)
            .SetPositiveButton("Choose", delegate
                 {
                     code = input.Text;
                     if (code == "godmode") godmode = !godmode;
                     Console.WriteLine(code + " " + godmode);
                     canvasView.Invalidate();
                  })
            .SetNegativeButton("Cancel", delegate
                   {
                        alertbuilder.Dispose();
                   });
             Android.Support.V7.App.AlertDialog dialog = alertbuilder.Create();
            dialog.Show();
        }

        public void setGodmode(Boolean gm)
        {
            godmode = gm;
        }

        public void toggleGodmode()
        {
            setGodmode(!getGodmode());
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
        
        public GameEventHandler GetGameEventHandler()
        {
            return eventHandler;
        }

        public List<GameObject> getGameObjects()
        {
            return gameObjects;
        }

    }
}