using System;
using Android.Content;

namespace Flyiing_Hiigh
{
    public class ObjEnemy : GameObject
    {
        private int health;
        public int deathAnimation;

        public ObjEnemy(Context context, String typ, float screen_width_proportion,  int health) : base(context, typ, screen_width_proportion)
        {
            this.health = health;
        }

        public void hitByShot()
        {
            if(health > 1)
            {
                health--;
            }
            else
            {
                health--;
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