using System;
using Android.Content;
using SkiaSharp;
using SkiaSharp.Views.Android;

namespace Flyiing_Hiigh
{
    public class ObjBackground : GameObject
    {
        SKRect rect1;
        SKRect rect2;

        Boolean screen_init;

        Audiomanager audiomanager;        

        public ObjBackground(Context context, int theme) : base(context, "Background", 1)
        {
            String resID;
            audiomanager = new Audiomanager();

            switch (theme)
            {
                case 0:
                    resID = "Flyiing_Hiigh.Resources.Drawable.backshroom.png";
                    audiomanager.play("backshroom.wav");
                    break;
                case 1:
                    resID = "Flyiing_Hiigh.Resources.Drawable.backdark.png";
                    audiomanager.play("backdark.mp3");
                    break;
                case 2:
                    resID = "Flyiing_Hiigh.Resources.Drawable.backjungle.png";
                    audiomanager.play("backjungle.mp3");
                    break;
                case 3:
                    resID = "Flyiing_Hiigh.Resources.Drawable.backbutterfly.png";
                    audiomanager.play("backbutterlfy.mp3");
                    break;
                default:
                    resID = "Flyiing_Hiigh.Resources.Drawable.backshroom.png";
                    audiomanager.play("backshroom.wav");
                    break;
            }
            setResourceID(resID);
            this.xSpeed = -1;

        }
        
        public SKRect[] getRectangles(int width, int height)
        {
            if (!screen_init)
            {
                float aspect_ratio = bitmap.Width / bitmap.Height;
                rect1 = new SKRect(0, 0, aspect_ratio * width, height);
                rect2 = new SKRect(aspect_ratio * width - 1, 0, aspect_ratio * width * 2 - 1, height);
                screen_init = true;
            }
            
            return new SKRect[] {rect1, rect2};
        }


        public override void move()
        {
            if(rect1.Right < 0)
            {
                rect1.Location = new SKPoint(rect2.Location.X + rect2.Width - 1, rect2.Location.Y);
            }

            if (rect2.Right < 0)
            {
                rect2.Location = new SKPoint(rect1.Location.X + rect1.Width - 1, rect1.Location.Y);
            }
           
            rect1.Offset((float)xSpeed, 0);
            rect2.Offset((float)xSpeed, 0);

        }

        public override void OnCanvasViewPaintSurface(SKPaintSurfaceEventArgs e)
        {
            SKImageInfo imageInfo = e.Info;
            SKSurface surface = e.Surface;
            SKCanvas canvas = surface.Canvas;

            SKRect[] rects = getRectangles(imageInfo.Width, imageInfo.Height);

            canvas.DrawBitmap(getBitmap(), rects[0]);
            canvas.DrawBitmap(getBitmap(), rects[1]);
        }

        public void stopBackgroundMusic()
        {
            audiomanager.stopPlaying();
        }
        
    }
}