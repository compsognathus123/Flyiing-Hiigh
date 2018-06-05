using System;
using Android.Content;
using SkiaSharp;

namespace Flyiing_Hiigh
{
    public class ObjPlayer : GameObject
    {
        double g = -350;

        ObjWeapon weapon;
        private float animationXOffset;

        public ObjPlayer(Context context) : base(context, "ObjPlayer")
        {
            this.setResourceID("Flyiing_Hiigh.Resources.Drawable.butterfly.png");
            this.setRectangle(125,200, 300, 375);
            this.ySpeed = 0;
                       
        }

        public override void move()
        {
            rect.Location = new SKPoint(125, rect.Location.Y);

            ySpeed += g * activity.getTickDurationMs() * Math.Pow(10, -3);
            float yOffset = (float)(ySpeed * activity.getTickDurationMs() * Math.Pow(10, -3));

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

        public void accelerate()
        {
            if(rect.Top > 0)
            {
                ySpeed = 300;
            }
        }

        public void setWeapon(ObjWeapon weapon)
        {
            if (!activity.muted)
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