using System;
using System.Collections.Generic;
using Android.Content;
using SkiaSharp;
using SkiaSharp.Views.Android;

namespace Flyiing_Hiigh
{
    class ObjEnemyBird : ObjEnemy
    {
        public static int initWidth = 300;
        public static int initHeight = 300;

        private int resource_duration;
        private int resID;

        private Boolean isFighting;
        private double time_next_shot;

        private SKBitmap[] sprites;
                
        int updown;

        int nth_bird;
        SKPaint paint;

        int shoot_period;

        public ObjEnemyBird(Context context, int nth_bird) : base(context, "Bird", 0.25f, 10)
        {
            paint = new SKPaint();
            this.nth_bird = nth_bird;

            if ((nth_bird * 750) <= 3000)
            {
                shoot_period = 3000 - (nth_bird * 750);

               // paint.ColorFilter = SKColorFilter.CreateBlendMode(SKColors.Blue.WithAlpha(100), SKBlendMode.SrcATop);
            }
            else
            {
                shoot_period = 300;

            }

            sprites = new SKBitmap[8];
            for (int i = 1; i < 8; i++)
            {
                setResourceID("Flyiing_Hiigh.Resources.Drawable.Bird.bird" + i + ".png");
                sprites[i] = getBitmap();
            }

            setPosition(activity.getImageInfo().Width, (int)(activity.getImageInfo().Height / 2 - getHeight()));

            resID = 1;
            xSpeed = -1.5;
            updown = 1;
            
        }

        public override void move()
        {            
            //Check wether to remove
            if (isDead() && rect.Top > activity.getImageInfo().Height)
            {
                activity.removeGameObject(this);
            }

            //Pre-Fight Flight
            if (!isFighting && !isDead())
            {
                if (rect.Left > activity.getImageInfo().Width - rect.Width * 1.25)
                {
                    xSpeed = -1.5 + 1.5 * (activity.getImageInfo().Width - rect.Right) / (activity.getImageInfo().Width - rect.Width * 1.25);
                }
                else
                {
                    isFighting = true;
                    xSpeed = 0;
                    time_next_shot = activity.getTime() + shoot_period;
                }
            }

            //Flight animation
            if (resource_duration > 25 && !isDead())
            {
                if (resID == 7)
                {
                    resID = 1;
                }
                else
                {
                    resID++;
                }
                resource_duration = 0;
            }
            else
            {
                resource_duration++;
            }

            if (ySpeed > 0.6) updown = -1;
            if (ySpeed < -0.6) updown = 1;

            ySpeed += updown * 0.0025;

            //apply movement
            rect.Offset((float)xSpeed, (float)ySpeed);

            handleShooting();

        }

        private void handleShooting()
        {

            if(isFighting && time_next_shot < activity.getTime() && !isDead())
            {
                time_next_shot = activity.getTime() + shoot_period;

                activity.RunOnUiThread(() =>
                {
                    int x = (int)(rect.MidX - rect.Width/3);
                    int y = (int) rect.MidY;
                    activity.getGameObjects().Add(new ObjNut(activity, x, y));
                });

            }
        }

        public override void onDeath()
        {
            activity.GetGameEventHandler().OnBirdDied();
        }

        public override void OnCanvasViewPaintSurface(SKPaintSurfaceEventArgs e)
        {
            SKImageInfo imageInfo = e.Info;
            SKSurface surface = e.Surface;
            SKCanvas canvas = surface.Canvas;

            if (isDead())
            {
                SKPoint rotatePoint = new SKPoint(rect.MidX, rect.MidY);

                canvas.RotateDegrees(deathAnimation, rotatePoint.X, rotatePoint.Y);
                canvas.DrawBitmap(getBitmap(), getRectangle(), paint);
                canvas.RotateDegrees(-deathAnimation, rotatePoint.X, rotatePoint.Y);

                deathAnimation += 15;
                ySpeed += 0.2;
            }
            else
            {
                canvas.DrawBitmap(sprites[resID], getRectangle(), paint);

                //Draw health-bar
                int separate = 20;
                var paintMargin = new SKPaint
                {
                    Style = SKPaintStyle.Stroke,
                    Color = SKColors.Black
                };
                var paintHP = new SKPaint
                {
                    Style = SKPaintStyle.Fill,
                    Color = SKColors.IndianRed
                };
                var paintFillWhite = new SKPaint
                 {
                     Style = SKPaintStyle.Fill,
                     Color = SKColors.White
                };

                float hp = -((activity.getImageInfo().Width - separate) - (activity.getImageInfo().Width / 2 + separate)) * ((10 - (float)getHealth()) / 10);
              
                SKRect margin = new SKRect(activity.getImageInfo().Width / 2 + separate, activity.getImageInfo().Height - separate * 2, activity.getImageInfo().Width - separate, activity.getImageInfo().Height - separate);
                SKRect fill = new SKRect(activity.getImageInfo().Width / 2 + separate, activity.getImageInfo().Height - separate * 2, activity.getImageInfo().Width - separate + (int)hp, activity.getImageInfo().Height - separate);

                canvas.DrawRoundRect(margin, 5, 5, paintFillWhite);
                canvas.DrawRoundRect(fill, 5, 5, paintHP);
                canvas.DrawRoundRect(margin, 5, 5, paintMargin);     

            }


        }
    }
}