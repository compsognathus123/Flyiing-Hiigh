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
                
        int updown;

        public ObjEnemyBird(Context context) : base(context, "Bird", 0.25f, 10)
        {
            setResourceID("Flyiing_Hiigh.Resources.Drawable.Bird.bird1.png");
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
                    time_next_shot = activity.getTime() + 1000;
                }
            }

            //Flight animation
            if (resource_duration > 30 && !isDead())
            {
                if (resID == 7)
                {
                    resID = 1;
                }
                else
                {
                    resID++;
                }
                setResourceID("Flyiing_Hiigh.Resources.Drawable.Bird.bird" + resID + ".png");
                resource_duration = 0;
            }
            else
            {
                resource_duration++;
            }

            if (ySpeed > 0.5) updown = -1;
            if (ySpeed < -0.5) updown = 1;

            ySpeed += updown * 0.0025;

            //apply movement
            rect.Offset((float)xSpeed, (float)ySpeed);

            handleShooting();

        }

        private void handleShooting()
        {
            if(isFighting && time_next_shot < activity.getTime() && !isDead())
            {
                time_next_shot = activity.getTime() + 3000;

                activity.RunOnUiThread(() =>
                {
                    int x = (int)(rect.MidX - rect.Width/3);
                    int y = (int)rect.MidY;
                    activity.getGameObjects().Add(new ObjNut(activity, x, y));
                });

            }
        }

        public override void onDeath()
        {
            base.onDeath();
        }

        public override void OnCanvasViewPaintSurface(SKPaintSurfaceEventArgs e)
        {
            SKImageInfo imageInfo = e.Info;
            SKSurface surface = e.Surface;
            SKCanvas canvas = surface.Canvas;

            if (isDead())
            {
                canvas.RotateDegrees(deathAnimation, rect.MidX, rect.MidY);
                canvas.DrawBitmap(getBitmap(), getRectangle());
                canvas.RotateDegrees(-deathAnimation, rect.MidX, rect.MidY);

                deathAnimation += 15;
                ySpeed += 0.2;
            }
            else
            {
                canvas.DrawBitmap(getBitmap(), getRectangle());

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

                float hp = -((activity.getImageInfo().Width - separate) - (activity.getImageInfo().Width / 2 + separate)) * ((10 - getHealth()) / 10);

                SKRect margin = new SKRect(activity.getImageInfo().Width / 2 + separate, activity.getImageInfo().Height - separate * 2, activity.getImageInfo().Width - separate, activity.getImageInfo().Height - separate);
                SKRect fill = new SKRect(activity.getImageInfo().Width / 2 + separate, activity.getImageInfo().Height - separate * 2, activity.getImageInfo().Width - separate + (int)hp, activity.getImageInfo().Height - separate);

                canvas.DrawRoundRect(margin, 5, 5, paintFillWhite);
                canvas.DrawRoundRect(fill, 5, 5, paintHP);
                canvas.DrawRoundRect(margin, 5, 5, paintMargin);     

            }


        }
    }
}