﻿using System;
using Android.Content;
using SkiaSharp;

namespace Flyiing_Hiigh
{
    public class ObjPlayer : GameObject
    {
        double g = -425;

        public Boolean accelerating;

        ObjWeapon weapon;
        private float animationXOffset;

        public ObjPlayer(Context context) : base(context, "ObjPlayer", 0.1f)
        {
            this.setResourceID("Flyiing_Hiigh.Resources.Drawable.butterfly.png");
            this.rect = new SKRect(125, 200, 200, 275);
            this.ySpeed = 100;
                       
        }

        public override void move()
        {
            rect.Size = new SKSize(getWidth(), getHeight());
            rect.Location = new SKPoint(125, rect.Location.Y);

            if ( ySpeed > -450) ySpeed += g * activity.getTickDurationMs() * Math.Pow(10, -3);
            float yOffset = (float)(ySpeed * activity.getTickDurationMs() * Math.Pow(10, -3));

            if (accelerating && ySpeed < 450)
            {
                ySpeed += 3;
            }

            if(rect.Top < 0)
            {
                accelerating = false;
                if (rect.Bottom < 0) ySpeed = -50;
            }

            if (animationXOffset > 0)
            {
                animationXOffset -= 0.5f;
            }

            if(rect.Top > activity.getImageInfo().Height && activity.getTime() > 500)
            {
                activity.onPlayerDied(this);
            }

            rect.Offset(animationXOffset, -yOffset);            

        }

        public void checkIntersect(GameObject obj)
        {
            if(((obj is ObjEnemy && !((ObjEnemy)obj).isDead()) || obj is ObjNut) && obj.getRectangle().IntersectsWith(this.getRectangle()))
            {
                activity.onPlayerDied(obj);
            }
   
        }

        public void accelerate(Boolean accelerated)
        {
            if(!accelerating && accelerated)
            {
                if (rect.Top > 0)
                {
                    ySpeed = 275;
                }
            }

            accelerating = accelerated;

        }

        public void setWeapon(ObjWeapon weapon)
        {
            if (!activity.isMuted())
                Audiomanager.Play("gunload.mp3", 20);

            this.weapon = weapon;
        }

        public ObjWeapon getWeapon()
        {
            return weapon;
        }

        public void performShootingAnimation()
        {
            animationXOffset = 5;

        }



    }
}