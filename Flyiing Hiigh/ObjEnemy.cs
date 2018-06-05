using System;
using Android.Content;

namespace Flyiing_Hiigh
{
    public class ObjEnemy : GameObject
    {
        private int health;
        public int deathAnimation;

        public ObjEnemy(Context context, String typ, int health) : base(context, typ)
        {
            this.health = health;
        }

        public void hitByShot()
        {
            if(health != 0)
            {
                health--;
            }
            else
            {
                onDeath();
                ySpeed = 0;
                activity.increaseScore();
            }
        }

        public virtual void onDeath()
        {

        }

        public int getHealth()
        {
            return health;
        }

        public Boolean isDead()
        {
            if(health == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}