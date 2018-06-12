using System;
using System.Collections.Generic;

namespace Flyiing_Hiigh
{
    public class GameEventHandler
    {
        GameActivity activity;

        private double time;

        List<gameevent> archievedEvents;
        
        double SPAWNTIME_WEAPON = 1500;
        double SPAWNTIME_SPIDERS = 7500;
        double SPAWNTIME_BEES = 1500;
        double SPAWNTIME_BIRD = 10000;

        private double next_bird_spawn_delta = 10000;

        private double next_bee_spawn_time = 2500;
        private double next_spider_spawn_time = 3000;


        public GameEventHandler(GameActivity activity, List<gameevent> archievedEvents)
        {
            this.activity = activity;
            this.archievedEvents = archievedEvents;

            time = activity.getTime();
        }

        public void OnBirdDied()
        {
            activity.RunOnUiThread(() =>
            {
                archievedEvents.Remove(gameevent.BIRD_SPAWNED);
                archievedEvents.Add(gameevent.SPAWNING_SPIDERS);
                archievedEvents.Add(gameevent.SPAWNING_BEES);
                SPAWNTIME_BIRD = time + next_bird_spawn_delta;
            });
        }

        public void checkEvents()
        {
            time = activity.getTime();

            if (time > 100000)
            {
                activity.getTimer().Stop();
            }

            activity.RunOnUiThread(() =>
            {
                spawnWeapon();
                spawnBees();
                spawnSpiders();
                spawnBird();
            });
        }

        private void spawnSpiders()
        {
            if (time > SPAWNTIME_SPIDERS && time < SPAWNTIME_BIRD - 2000 && !archievedEvents.Contains(gameevent.SPAWNING_SPIDERS))
            {
                    archievedEvents.Add(gameevent.SPAWNING_SPIDERS);
            }
            else
            {
                    archievedEvents.Remove(gameevent.SPAWNING_SPIDERS);
            }

            if (time > next_spider_spawn_time && archievedEvents.Contains(gameevent.SPAWNING_SPIDERS))
            {
                Random rnd = new Random();
                int x = activity.getImageInfo().Width;
                int y = rnd.Next(0, rnd.Next(activity.getImageInfo().Height - 300));

                activity.RunOnUiThread(() =>
                {
                    activity.getGameObjects().Add(new ObjEnemySpider(activity,x,y));
                });

                next_spider_spawn_time = time + 5000 - (time / (SPAWNTIME_BIRD - 2000) * 4600);
            }
        }

        private void spawnBird()
        {
            if(!archievedEvents.Contains(gameevent.BIRD_SPAWNED) && time > SPAWNTIME_BIRD)
            {                              
                archievedEvents.Add(gameevent.BIRD_SPAWNED);
                activity.getGameObjects().Add(new ObjEnemyBird(activity));   
            }
        }

        private void spawnWeapon()
        {
            if (!archievedEvents.Contains(gameevent.WEAPON_SPAWNED) && time > SPAWNTIME_WEAPON)
            {
                archievedEvents.Add(gameevent.WEAPON_SPAWNED);

                Random rnd = new Random();
                int x = activity.getImageInfo().Width;
                int y = rnd.Next(0, rnd.Next(activity.getImageInfo().Height - 200));

                    activity.getGameObjects().Add(new ObjWeapon(activity, x, y));

            }
        }

        private void spawnBees()
        {
            if(time > SPAWNTIME_BEES &&  time < SPAWNTIME_BIRD - 2000 && !archievedEvents.Contains(gameevent.SPAWNING_BEES))
            {
                archievedEvents.Add(gameevent.SPAWNING_BEES);
            }
            else
            {
                archievedEvents.Remove(gameevent.SPAWNING_BEES);
            }

            if (time > next_bee_spawn_time && archievedEvents.Contains(gameevent.SPAWNING_BEES))
            {
                Random rnd = new Random();
                int x = activity.getImageInfo().Width;
                int y = rnd.Next(0, rnd.Next(activity.getImageInfo().Height - 100));

                    activity.getGameObjects().Add(new EnemyBee(activity, x, y, false));

                next_bee_spawn_time = time + 5000 - (time / (SPAWNTIME_BIRD - 2000) * 4600);
            }
        }
    }
}