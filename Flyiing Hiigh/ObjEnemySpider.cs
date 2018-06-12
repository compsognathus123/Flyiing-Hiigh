
using Android.Content;
using SkiaSharp;
using SkiaSharp.Views.Android;

namespace Flyiing_Hiigh
{
    public class ObjEnemySpider : ObjEnemy
    {        
        float midYOnDeath;

        public ObjEnemySpider(Context context, int x, int y) : base(context, "Spider", 0.0725f, 1)
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

            SKPaint lineCol = new SKPaint { Color = SKColors.White.WithAlpha(180), StrokeWidth = 3, IsStroke = true};

            if (isDead())
            {
                canvas.DrawLine(this.rect.MidX, 0, this.rect.MidX, midYOnDeath, lineCol);

                SKPoint rotatePoint = new SKPoint(rect.MidX, rect.MidY);

                canvas.RotateDegrees(deathAnimation, rotatePoint.X, rotatePoint.Y);
                canvas.DrawBitmap(getBitmap(), getRectangle());
                canvas.RotateDegrees(-deathAnimation, rotatePoint.X, rotatePoint.Y);
                                
                midYOnDeath -= 25;
                deathAnimation += 12;
                ySpeed += 0.2;
            }
            else { 
                
                canvas.DrawLine(this.rect.MidX, 0, this.rect.MidX, this.rect.MidY, lineCol);
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