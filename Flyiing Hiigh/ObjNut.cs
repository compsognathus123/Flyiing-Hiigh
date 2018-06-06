
using Android.Content;
using SkiaSharp;
using SkiaSharp.Views.Android;

namespace Flyiing_Hiigh
{
    class ObjNut : GameObject
    {
        public static int initHeight = 50;
        public static int initWidth = 50;

        private int rotate_degree;
                
        public ObjNut(Context context, int x, int y) : base(context, "Nut")
        {
            setResourceID("Flyiing_Hiigh.Resources.Drawable.Bird.walnut.png");
            setPosition(x, y);

            xSpeed = -2.5;
            ySpeed = xSpeed * (y - activity.getPlayer().getRectangle().MidY)/ activity.getImageInfo().Height - 0.3;
            
           // if (!activity.muted)
             //   Audiomanager.Play("silencer.wav", 0.5);
            
        }

        public override void move()
        {
            ySpeed += 0.005;
            if (rect.Right < 0)
            {
                activity.removeGameObject(this);
            }

            rotate_degree += 2;
            rect.Offset((float)xSpeed, (float)ySpeed);
        }

        public override void OnCanvasViewPaintSurface(SKPaintSurfaceEventArgs e)
        {
            SKImageInfo imageInfo = e.Info;
            SKSurface surface = e.Surface;
            SKCanvas canvas = surface.Canvas;

            canvas.RotateDegrees(rotate_degree, getRectangle().MidX, getRectangle().MidY);

            canvas.DrawBitmap(getBitmap(), getRectangle());

            canvas.RotateDegrees(-rotate_degree, getRectangle().MidX, getRectangle().MidY);

        }

       
    }
}