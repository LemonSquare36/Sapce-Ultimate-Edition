using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;
using System.Diagnostics;
using System.IO;
using System.Collections;

namespace Ultimate_Sapce
{
    class Entities : Polygons
    {
        private int bVe;
        Stopwatch timer = new Stopwatch();
        int time;
       // bool elapsed = true;
        bool canShoot = false, dead = false, deathsoundplayed = false;
        Vector2 Path, offset;
        string shapetype;
        float accelerate = 0;
        SpriteSheet Deathsheet;
        SoundManager deathsound;

        public int AddHP(int newHP)
        {
            HP += newHP;
            return HP;
        }
        public int GetTime()
        {
            return Convert.ToInt16(timer.ElapsedMilliseconds);
        }
        public void AlterTime()
        {
            if (timer.IsRunning)
                timer.Stop();
            else
                timer.Start();
        }
        public bool CanShoot()
        {
            return canShoot;
        }
        public Entities(List<Vector2> numbers) : base(numbers)
        {
            deathsound = new SoundManager();
        }
        /// <param name="X">Placement X value</param>
        /// <param name="Y">Placement Y value</param>
        /// <param name="BvE">1 for normal enemies, 2 for bullets</param>
        public override void LoadContent(float X, float Y, string shapeFile)
        {
            base.LoadContent(X, Y, shapeFile);

                shapetype = shapeFile;

                switch (shapeFile)
                {
                    case "cannon":
                    time = 1200;
                    HP = 3;
                    timer.Start();
                    deathsound.Load("ShipExplosion", true);
                    Deathsheet = new SpriteSheet("ExplosionSheet1", 1, 32);
                    offset = new Vector2(192, 144);
                    break;
                    case "zoomer":
                        HP = 2;
                        break;
                    case "bullet":
                    accelerate = 2;
                        break;
            }
        }
        public void Update()
        {
            if (HP <= 0)
            {
                dead = true;
            }
            switch (shapetype)
            {
                case "cannon":
                    if (Placement.X > 1300)
                    {
                        Placement.X -= 2;
                    }
                    break;
                case "zoomer":
                    Placement.X -= 35;
                    break;
                case "bullet":
                    if (accelerate < 10)
                    accelerate += accelerate/10;

                    Placement.X += accelerate;
                    break;
            }           
            if (canShoot)
            {
                canShoot = false;
                timer.Restart();
            }
            EnemyTimerElapsed();
        }
        public void SetPath(int X, int Y)
        {
            float Movex = X - Placement.X;
            float Movey = Y - Placement.Y;
            double C = Math.Sqrt((Movex * Movex) + (Movey * Movey));
            Movex /= (float)C;
            Movey /= (float)C;
            Movex *= 10f;
            Movey *= 10f;
            Path = new Vector2(Movex, Movey);
        }
        public Vector2 getPath()
        {
            return Path;
        }
        private void EnemyTimerElapsed()
        {
            if (timer.ElapsedMilliseconds >= time)
            { canShoot = true; }
        }
        public int getBvE()
        {
            return bVe;
        }
        public bool getDead()
        {
            return dead;
        }
        public void SetDead()
        {
            dead = true;
        }
        public void DeathAnimation(SpriteBatch spriteBatch)
        {
            Deathsheet.Update(2, false);
            Deathsheet.Draw(spriteBatch, Placement - offset, 10);
        }
        public void PlayDeathSound()
        {
            if (!deathsoundplayed)
            {
                deathsound.Play();
                deathsoundplayed = true;
            }
        }
        public bool getDeathAniDone()
        {
            return Deathsheet.getDone();
        }
    }
}
