
using Android.Content;
using SkiaSharp;
using SkiaSharp.Views.Android;

namespace Flyiing_Hiigh
{
    public class ObjOptions : GameObject
    {

        public ObjOptions(Context context) : base(context, "Options")
        {
        }

        public override void OnCanvasViewPaintSurface(SKPaintSurfaceEventArgs e)
        {
            SKImageInfo imageInfo = e.Info;
            SKSurface surface = e.Surface;
            SKCanvas canvas = surface.Canvas;

            setPosition(0, 0);

            if (activity.isMuted()) setResourceID("Flyiing_Hiigh.Resources.Drawable.optionsmuted.png");
            else setResourceID("Flyiing_Hiigh.Resources.Drawable.options.png");
            
            SKPaint paint = new SKPaint();
            paint.Color = (SKColor)0x3Fffffff;

            canvas.DrawRect(0,0,imageInfo.Width,imageInfo.Height, paint);
            canvas.DrawBitmap(getBitmap(), getRectangle());
        }
    }
}