﻿using System;
using Android.Content;
using SkiaSharp;
using SkiaSharp.Views.Android;

namespace Flyiing_Hiigh
{
    public class ObjWeapon : GameObject
    {
        private Boolean collected;

        private float animationXOffset;

        public ObjWeapon(Context context, int x, int y) : base(context, "Weapon", 0.0825f)
        {
            setResourceID("Flyiing_Hiigh.Resources.Drawable.WeaponStuff.gun.png");            
            setPosition(x, y);

            xSpeed = -1;

        }

        public override void OnCanvasViewPaintSurface(SKPaintSurfaceEventArgs e)
        {
            SKImageInfo imageInfo = e.Info;
            SKSurface surface = e.Surface;
            SKCanvas canvas = surface.Canvas;

            SKPoint rotatePoint = new SKPoint(getRectangle().Left, getRectangle().MidY);

            canvas.RotateDegrees(-animationXOffset / 2, rotatePoint.X, rotatePoint.Y);
            canvas.DrawBitmap(getBitmap(), getRectangle());
            canvas.RotateDegrees(animationXOffset / 2, rotatePoint.X, rotatePoint.Y);
        }

        public override void move()
        {
            if (!collected)
            {
                rect.Offset((float)xSpeed, 0);
                if(rect.Left + rect.Width < 0)
                {
                    activity.removeGameObject(this);
                }
            }
            else
            {
                if(animationXOffset > 0)
                {
                    animationXOffset -= 0.5f;
                    
                }

                rect.Size = new SKSize(getWidth(), getHeight());
                rect.Location = activity.getPlayer().getRectangle().Location;
                rect.Offset(activity.getPlayer().getRectangle().Width/8*5  + animationXOffset, activity.getPlayer().getRectangle().Height/7*4 );
                
            }
        }

        public void performShootingAnimation()
        {
            animationXOffset = 7;            
        }
               

        public void checkIntersect(GameObject obj)
        {
            if(!collected && obj is ObjPlayer && rect.IntersectsWith(((ObjPlayer)obj).getRectangle()))
            {
                activity.getPlayer().setWeapon(this);
                collected = true;
            }
        }

    }
}