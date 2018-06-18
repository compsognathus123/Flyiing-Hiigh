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
        double SPAWNTIME_SPIDERS = 7650;
        double SPAWNTIME_BEES = 1500;
        double SPAWNTIME_BIRD = 50000;

        int nth_bird;

        private double next_bird_spawn_delta = 30000;

        private double next_bee_spawn_time = 2500;
        private double next_spider_spawn_time = 3000;

        Random rnd;


        public GameEventHandler(GameActivity activity, List<gameevent> archievedEvents)
        {
            this.activity = activity;
            this.archievedEvents = archievedEvents;

            time = activity.getTime();
        }

        public void OnBirdDied()
        {
            nth_bird++;

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
                rnd = new Random();
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
                int x = activity.getImageInfo().Width;
                int y = rnd.Next(0, rnd.Next(activity.getImageInfo().Height - 300));

                activity.RunOnUiThread(() =>
                {
                    activity.getGameObjects().Add(new ObjEnemySpider(activity,x,y));
                });

                next_spider_spawn_time = time + 7200 - (time / (SPAWNTIME_BIRD - 2000) * 6700);
            }
        }

        private void spawnBird()
        {
            if(!archievedEvents.Contains(gameevent.BIRD_SPAWNED) && time > SPAWNTIME_BIRD)
            {                              
                archievedEvents.Add(gameevent.BIRD_SPAWNED);

                activity.getGameObjects().Add(new ObjEnemyBird(activity, nth_bird));
            }
        }

        private void spawnWeapon()
        {
            if (!archievedEvents.Contains(gameevent.WEAPON_SPAWNED) && time > SPAWNTIME_WEAPON)
            {
                archievedEvents.Add(gameevent.WEAPON_SPAWNED);
                
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
                int x = activity.getImageInfo().Width;
                int y = rnd.Next(0, rnd.Next(activity.getImageInfo().Height - 100));

                activity.getGameObjects().Add(new EnemyBee(activity, x, y, false));

                next_bee_spawn_time = time + 5000 - (time / (SPAWNTIME_BIRD - 2000) * 4600);
            }
        }
    }
}