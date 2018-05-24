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
    class PlayerShip : Polygons
    {        
        int shields, gain, downtime, deadTimeGoal, deadTime;
        bool dead = false, effectHasGone = false;
        SpriteSheet deathAnimation;
        SoundManager explosionSound;

        public PlayerShip(List<Vector2> numbers) : base(numbers)
        {
            HP = 200;
            shields = 20;
            gain = 0;
            deadTimeGoal = 180;
            deadTime = 0;
            explosionSound = new SoundManager();

        }
        public override void LoadContent(float X, float Y, string shapeFile)
        {          
            base.LoadContent(X, Y, shapeFile);
            explosionSound.Load("ShipExplosion", true);
            deathAnimation = new SpriteSheet("PixelExplosion", 9, 9);
        }
        public void AsteroidHit()
        {

            shields -= 20;
            if (shields < 0)
            {
                HP += shields;
                shields = 0;
                downtime = 0;
            }
        }
        public void SmallBulletHit()
        {
            shields -= 10;
            if (shields < 0)
            {
                HP += shields;
                shields = 0;
                downtime = 0;
            }
        }
        public void EnemyShipHit()
        {
            shields -= 40;
            if (shields < 0)
            {
                HP += shields;
                shields = 0;
                downtime = 0;
            }
        }
        public void CustomHit(int Damage)
        {
            shields -= Damage;
            if (shields < 0)
            {
                HP += shields;
                shields = 0;
                downtime = 0;
            }
        }
        public bool getDead()
        {
            return dead;
        }
        public void Update(KeyboardState CurrentKeyBoardState)
        {

            if (!dead)
            {
                RestoreShields();
                MoveShape(CurrentKeyBoardState);
                if (HP <= 0)
                {
                    dead = true;
                }
            }
            else if (dead)
            {
                DeadTimeEvent();
                if (!effectHasGone)
                {
                    effectHasGone = true;
                    explosionSound.Play();
                }
            }
        }
        public void DeathAnimation(SpriteBatch spriteBatch)
        {
            deathAnimation.Update(2, false);
            deathAnimation.Draw(spriteBatch, Placement - new Vector2(150,130), 3);
        }
        public override void MoveShape(KeyboardState CurrentKeyBoardState)
        {
            Movement = Vector2.Zero;

            if (CurrentKeyBoardState.IsKeyDown(Keys.D))
            {
                Movement = new Vector2(Movement.X + 7f, Movement.Y);
            }
            if (CurrentKeyBoardState.IsKeyDown(Keys.S))
            {
                Movement = new Vector2(Movement.X, Movement.Y + 6f);
            }
            if (CurrentKeyBoardState.IsKeyDown(Keys.A))
            {
                Movement = new Vector2(Movement.X - 7f, Movement.Y);
            }
            if (CurrentKeyBoardState.IsKeyDown(Keys.W))
            {
                Movement = new Vector2(Movement.X, Movement.Y - 6f);
            }
            OldPosition = Placement;
            Placement += Movement;

        }
        public event EventHandler ChangeScreen;
        private void DeadTimeEvent()
        {
            deadTime++;
            if (deadTime >= deadTimeGoal)
            {
                ChangeScreen?.Invoke(this, EventArgs.Empty);
            }
        }

        public void Burn()
        {
            HP--;
        }
        public int getShields()
        {
            return shields;
        }
        public void RestoreShields()
        {
            if (shields != 20)
            {
                downtime++;
            }
            else
            {
                downtime = 0;
            }
            if (downtime > 180)
            {
                gain++;
                if (shields < 20 && gain >= 3)
                {
                    shields += 1;
                    gain = 0;
                }
            }
        }
    }
}
