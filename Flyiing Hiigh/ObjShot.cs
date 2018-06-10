using Android.Content;
using System;

namespace Flyiing_Hiigh
{
    public class ObjShot : GameObject
    {        
        public Boolean hasHit;

        int resID;

        public ObjShot(Context context, int x, int y) : base(context, "Shot", 0.03f)
        {
            setResourceID("Flyiing_Hiigh.Resources.Drawable.WeaponStuff.shot1.png");
            setPosition(x, y);

            xSpeed = 7;
            resID = 0;

            if (!activity.muted)
                Audiomanager.Play("silencer.wav", 0.5);

        }

        public override void move()
        {
            int newResID = 4 - (int)((activity.getImageInfo().Width - rect.Left) / (activity.getImageInfo().Width - activity.getPlayer().getWeapon().getRectangle().Right) * 4);

            if (resID < newResID)
            {
                resID = newResID;
                setResourceID("Flyiing_Hiigh.Resources.Drawable.WeaponStuff.shot" + newResID + ".png");
            }

            if (newResID > 4)
            {
                activity.removeGameObject(this);
            }

            rect.Offset((float)xSpeed, 0);
        }

        public void checkIntersect(GameObject obj)
        {
           if(obj is ObjEnemy && obj.getRectangle().IntersectsWith(this.getRectangle()) && !hasHit)
            {
                if (!((ObjEnemy)obj).isDead())
                {
                    ((ObjEnemy)obj).hitByShot();
                    activity.removeGameObject(this);
                    hasHit = true;
                }
            }

        }
    }
}