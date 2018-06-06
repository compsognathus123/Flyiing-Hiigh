using System;
using Android.Content;
using SkiaSharp;
using SkiaSharp.Views.Android;

namespace Flyiing_Hiigh
{
    class EnemyBee : ObjEnemy
    {
        private int resource_duration;
        private int resID;

        int updown;
        
        Boolean isAggro;
        
        public EnemyBee(Context context, int x, int y, Boolean aggro) : base(context, "Bee", 1/15, 1)
        {
            this.isAggro = aggro;

            setResourceID("Flyiing_Hiigh.Resources.Drawable.Bee.bee1.png");
            setPosition(x, y);

            resID = 1;

            xSpeed = -2;
            ySpeed = 0;
            updown = 1;            

        }
        
        public override void move()
        {
            if(resource_duration > 10)
            {
                if(resID == 4)
                {
                    resID = 1;
                }
                else
                {
                    resID++;
                }
                setResourceID("Flyiing_Hiigh.Resources.Drawable.Bee.bee" + resID + ".png");

                resource_duration = 0;
            }
            else
            {
                resource_duration++;
            }

            if(ySpeed > 0.4) updown = -1;            
            if(ySpeed < -0.4) updown = 1;
            
            ySpeed += updown * 0.005;

            if (rect.Right < 0)
            {
                activity.removeGameObject(this);
            }

            rect.Offset((float)xSpeed, (float)ySpeed);

            if (isAggro)
            {
                if ((activity.getPlayer().getRectangle().MidY - rect.MidY) > 0){

                    rect.Offset(0, 0.3f);

                }else{
                    rect.Offset(0, -0.3f);
                }    
            }

        }

        public override void OnCanvasViewPaintSurface(SKPaintSurfaceEventArgs e)
        {
            SKImageInfo imageInfo = e.Info;
            SKSurface surface = e.Surface;
            SKCanvas canvas = surface.Canvas;

            if(isDead())
            {
                canvas.RotateDegrees(deathAnimation, rect.MidX, rect.MidY);
                canvas.DrawBitmap(getBitmap(), getRectangle());
                canvas.RotateDegrees(-deathAnimation, rect.MidX, rect.MidY);

                deathAnimation+= 12;
                ySpeed += 0.2;
            }           
            else
            {
                if (isAggro)
                {
                    SKPaint paint = new SKPaint();
                    paint.ColorFilter = SKColorFilter.CreateBlendMode(SKColor.FromHsl(356, 51, 43).WithAlpha(255), SKBlendMode.Modulate);

                    canvas.DrawBitmap(getBitmap(), getRectangle(), paint);
                    
                }
                else
                {
                    canvas.DrawBitmap(getBitmap(), getRectangle());

                }
            }
        }

        public override void onDeath()
        {
            updown = 0;
        }

       
    }
}