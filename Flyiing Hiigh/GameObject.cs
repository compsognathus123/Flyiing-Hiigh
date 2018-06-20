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

        public float aspect_ratio;
        public float screen_width_proportion;


        public GameObject(Context context, String typ, float screen_width_proportion)
        {
            this.typ = typ;
            this.screen_width_proportion = screen_width_proportion;

            activity = (GameActivity)context;
        }
        
        protected void setAdaptedOffset(float x, float y)
        {
          
            //getRatio()
           // rect.Offset();
        }
          
        protected void setResourceID(String resourceID)
        {
            Assembly assembly = GetType().GetTypeInfo().Assembly;
            using (Stream stream = assembly.GetManifestResourceStream(resourceID))
            using (SKManagedStream skStream = new SKManagedStream(stream))
            {
                bitmap = SKBitmap.Decode(skStream);
            }
            aspect_ratio = (float)bitmap.Width / (float)bitmap.Height;
        }

        public float getWidth()
        {
            return screen_width_proportion * activity.getImageInfo().Width;
        }

        public float getHeight()
        {
            return getWidth() / aspect_ratio;
        }

        protected void setPosition(int x, int y)
        {
            rect = new SKRect(x, y, x + getWidth(), y + getHeight());
            if(this is ObjPlayer)
            {
                activity.infotext = getWidth() + " " + getHeight() + "  " + activity.getImageInfo().Width;
            }
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
