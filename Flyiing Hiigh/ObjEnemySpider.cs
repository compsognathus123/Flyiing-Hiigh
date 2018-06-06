
using Android.Content;
using SkiaSharp;
using SkiaSharp.Views.Android;

namespace Flyiing_Hiigh
{
    public class ObjEnemySpider : ObjEnemy
    {        
        float midYOnDeath;

        public ObjEnemySpider(Context context, int x, int y) : base(context, "Spider", 1/10, 1)
        {
            this.setResourceID("Flyiing_Hiigh.Resources.Drawable.spider.png");
            setPosition(x, y);

            xSpeed = -1;
            ySpeed = 1;
            
        }

        public override void OnCanvasViewPaintSurface(SKPaintSurfaceEventArgs e)
        {
            SKImageInfo imageInfo = e.Info;
            SKSurface surface = e.Surface;
            SKCanvas canvas = surface.Canvas;

            SKPaint lineCol = new SKPaint { Color = SKColors.White };

            if (isDead())
            {
                canvas.DrawLine(this.rect.Left + 60, 0, this.rect.Right - 60, midYOnDeath, lineCol);

                canvas.RotateDegrees(deathAnimation, rect.MidX, rect.MidY);
                canvas.DrawBitmap(getBitmap(), getRectangle());
                canvas.RotateDegrees(-deathAnimation, rect.MidX, rect.MidY);

                midYOnDeath -= 15;
                deathAnimation += 12;
                ySpeed += 0.2;
            }
            else { 
                
                canvas.DrawLine(this.rect.Left + 60, 0, this.rect.Right - 60, this.rect.MidY, lineCol);
                canvas.DrawBitmap(getBitmap(), getRectangle());
            }
        }

        public override void move()
        {
            if ((this.rect.Bottom > activity.getImageInfo().Height || this.rect.Top < 0) && !isDead()) ySpeed *= -1;

            if(rect.Right < 0 || rect.Top > activity.getImageInfo().Height)
            {
                activity.removeGameObject(this);
            }

            rect.Offset((float)xSpeed, (float)ySpeed);
        }

        public override void onDeath()
        {
            midYOnDeath = this.rect.MidY;
        }

              
    }
}