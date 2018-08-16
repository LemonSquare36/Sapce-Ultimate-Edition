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
        bool dead = false, effectHasGone = false, stop = false;
        SpriteSheet deathAnimation;
        SoundManager explosionSound;
        Texture2D HPpix, hpBorder, hpBorder2, pixel, rockets;
        Rectangle currentHP, staticHP, shield;
        int score, finalscore, shieldwidth;
        int rocketNum;

        public PlayerShip(List<Vector2> numbers) : base(numbers)
        {
            HP = 200;
            shields = 20;
            gain = 0;
            deadTimeGoal = 180;
            deadTime = 0;
            explosionSound = new SoundManager();
            rocketNum = 30;
        }
        public override void LoadContent(float X, float Y, string shapeFile)
        {          
            base.LoadContent(X, Y, shapeFile);
            explosionSound.Load("ShipExplosion", true);
            deathAnimation = new SpriteSheet("PixelExplosion", 9, 9);
            HPpix = Main.GameContent.Load<Texture2D>("Sprites/HP");
            pixel = Main.GameContent.Load<Texture2D>("Sprites/Pixel");
            hpBorder = Main.GameContent.Load<Texture2D>("Sprites/hpBorder");
            hpBorder2 = Main.GameContent.Load<Texture2D>("Sprites/hpBorder2");
            rockets = Main.GameContent.Load<Texture2D>("Sprites/rocketcounter");

            currentHP = new Rectangle(100, 50, HP * 2, 50);
            staticHP = currentHP;
            shieldwidth = staticHP.Width / 20;
            shield = new Rectangle(100, 100, shields * shieldwidth, 5);
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
            currentHP.Width = HP * 2;
            shield.Width = shields * shieldwidth;

            if (!dead)
            {
                score++;
                RestoreShields();
                MoveShape(CurrentKeyBoardState);
                if (HP <= 0)
                {
                    dead = true;
                }
            }
            else if (dead)
            {
                if (!stop)
                {
                    finalscore = score;
                    stop = true;
                }
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
        public void DrawHUD(bool pause, SpriteBatch spriteBatch, SpriteFont font)
        {
            if (!dead)
            { spriteBatch.DrawString(font, "Score: " + score, new Vector2(1, 100), Color.Red, 0, Vector2.Zero, 1.6f, SpriteEffects.None, 0); }
            else if (dead)
            {
                spriteBatch.DrawString(font, "Score: " + finalscore, new Vector2(1, 100), Color.Red, 0, Vector2.Zero, 1.6f, SpriteEffects.None, 0);
            }
            spriteBatch.Draw(rockets, new Vector2(1800,10), Color.White);
            spriteBatch.DrawString(font, Convert.ToString(rocketNum), new Vector2(1750, 50), Color.White);
            spriteBatch.DrawString(font, "HP", new Vector2(20, 50), Color.Red, 0, Vector2.Zero, 1.7f, SpriteEffects.None, 0);
            spriteBatch.Draw(HPpix, currentHP, Color.White);
            spriteBatch.Draw(hpBorder, staticHP, Color.White);
            spriteBatch.Draw(hpBorder2, new Vector2(100, 55), Color.White);
            spriteBatch.Draw(hpBorder2, new Vector2(95 + staticHP.Width, 55), Color.White);
            spriteBatch.Draw(pixel, shield, Color.LightBlue);
            if (pause)
            {
                spriteBatch.DrawString(font, "PAUSED", new Vector2(720, 540), Color.GreenYellow, 0, Vector2.Zero, 4f, SpriteEffects.None, 0);
            }
            if (dead)
            {
                spriteBatch.DrawString(font, "YOU HAVE DIED", new Vector2(720, 540), Color.Red, 0, Vector2.Zero, 3, SpriteEffects.None, 0);
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
        public int addscore(int addScore)
        {
            score += addScore;
            return score;
        }
    }
}
