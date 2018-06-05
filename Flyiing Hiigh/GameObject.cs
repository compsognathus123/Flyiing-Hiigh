using System;
using System.IO;
using System.Reflection;
using Android.Content;
using SkiaSharp;
using SkiaSharp.Views.Android;

namespace Flyiing_Hiigh
{
    public class GameObject
    {
        protected GameActivity activity;

        protected String typ;

        protected SKRect rect;
        protected SKBitmap bitmap;

        protected double xSpeed;
        protected double ySpeed;


        public GameObject(Context context, String typ)
        {
            this.typ = typ;
            activity = (GameActivity)context;
        }

        protected void setResourceID(String resourceID)
        {
            Assembly assembly = GetType().GetTypeInfo().Assembly;
            using (Stream stream = assembly.GetManifestResourceStream(resourceID))
            using (SKManagedStream skStream = new SKManagedStream(stream))
            {
                bitmap = SKBitmap.Decode(skStream);
            }
        }

        protected void setRectangle(int x1, int y1, int x2, int y2)
        {
            rect = new SKRect(x1, y1, x2, y2);
        }

        public SKBitmap getBitmap()
        {
            return bitmap;
        }

        public virtual void move()
        {

        }

        public virtual void OnCanvasViewPaintSurface(SKPaintSurfaceEventArgs e)
        {
            e.Surface.Canvas.DrawBitmap(getBitmap(), getRectangle());

        }

        public String getTyp()
        {
            return typ;
        }
               
        public SKRect getRectangle()
        {
            return rect;
        }
        
    }
}