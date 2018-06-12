using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Timers;
using Android.App;
using Android.Content;
using Android.OS;
using SkiaSharp;
using SkiaSharp.Views.Android;

namespace Flyiing_Hiigh
{
    public class ObjOptions : GameObject
    {
        Boolean muted;

        public ObjOptions(Context context) : base(context, "Options", 1)
        {
            if (activity.isMuted()) setResourceID("Flyiing_Hiigh.Resources.Drawable.optionsmuted.png");
            else setResourceID("Flyiing_Hiigh.Resources.Drawable.options.png");
        }

        public override void OnCanvasViewPaintSurface(SKPaintSurfaceEventArgs e)
        {
            SKImageInfo imageInfo = e.Info;
            SKSurface surface = e.Surface;
            SKCanvas canvas = surface.Canvas;

            setPosition(0, 0);

            if(muted != activity.isMuted())
            {
                if (activity.isMuted()) setResourceID("Flyiing_Hiigh.Resources.Drawable.optionsmuted.png");
                else setResourceID("Flyiing_Hiigh.Resources.Drawable.options.png");
                muted = activity.isMuted();
            }

            SKPaint paint = new SKPaint();
            paint.Color = (SKColor)0x3Fffffff;

            activity.RunOnUiThread(() =>
            {
                activity.GetCanvasView().Invalidate();
                canvas.DrawRect(0, 0, imageInfo.Width, imageInfo.Height, paint);
                canvas.DrawBitmap(getBitmap(), getRectangle());
            });
            
        }
    }
}