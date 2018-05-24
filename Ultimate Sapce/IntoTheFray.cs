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
    class IntoTheFray : ScreenManager
    {
        Texture2D backTex, backTex2, backTex3, backTex4, HPpix, hpBorder, hpBorder2, pixel;
        PlayerShip Ship;
        Vector2 position, position2, position3, position4;
        Rectangle HP, staticHP, shield;
        SpriteFont font;
        Polygons wall1, wall2, wall3, wall4;
        List<Polygons> wallList, asteroidList;
        List<Entities> cannonList, enemyBulletList, zoomerList, bulletList;
        Random rand;
        int wallPath1, wallPath2, wallMove1, wallMove2, score, finalscore, shieldwidth;
        bool done = true; bool done1, done2;
        Stopwatch asteroidTimer = new Stopwatch(), cannonTimer = new Stopwatch(), zoomerTimer = new Stopwatch();
        KeyboardState oldState;
        bool stop = false;

        public override void Initialize()
        {
            position = new Vector2(0, -99);
            position2 = new Vector2(2097,0);
            position3 = new Vector2(-200, 100);
            position4 = new Vector2(1720, -100);
            rand = new Random();
            score = 0;
            Music = new SoundManager();
            done = true; done1 = false; done2 = false;

            #region Lists
            wallList = new List<Polygons>();
            asteroidList = new List<Polygons>();
            bulletList = new List<Entities>();
            cannonList = new List<Entities>();
            enemyBulletList = new List<Entities>();
            zoomerList = new List<Entities>();
            #endregion
            #region Timer
            asteroidTimer.Start();
            cannonTimer.Start();
            zoomerTimer.Start();
            #endregion 
        }
        //Holds LoadContent and the font if called
        public override void LoadContent(SpriteBatch spriteBatchmain)
        {
            spriteBatch = spriteBatchmain;
            font = Main.GameContent.Load<SpriteFont>("myFont");
            Music.Load("Solar Empress");
            Music.Play();
            
            backTex = Main.GameContent.Load<Texture2D>("MenuImages/Starbackground");
            backTex2 = Main.GameContent.Load<Texture2D>("MenuImages/Starbackground");
            backTex3 = Main.GameContent.Load<Texture2D>("MenuImages/Starbackground2");
            backTex4 = Main.GameContent.Load<Texture2D>("MenuImages/Starbackground2");
            HPpix = Main.GameContent.Load<Texture2D>("Sprites/HP");
            pixel = Main.GameContent.Load<Texture2D>("Sprites/Pixel");
            hpBorder = Main.GameContent.Load<Texture2D>("Sprites/hpBorder");
            hpBorder2 = Main.GameContent.Load<Texture2D>("Sprites/hpBorder2");

            MakeShapes();
            Ship.LoadContent(200, 540, "Base Ship");
            wall1.LoadContent(0, 200, "Wall");
            wall2.LoadContent(1920, 200, "Wall");
            wall3.LoadContent(0, 800, "Wall2");
            wall4.LoadContent(1920, 800, "Wall2");

            #region wallList
            wallList.Add(wall1);
            wallList.Add(wall2);
            wallList.Add(wall3);
            wallList.Add(wall4);
            #endregion
            Ship.ChangeScreen += OnScreenChanged;
            HP = new Rectangle(100, 50, Ship.getHP()*2, 50);           
            staticHP = HP;
            shieldwidth = staticHP.Width / 20;
            shield = new Rectangle(100, 100, Ship.getShields() * shieldwidth, 5);
        }
        //Holds Update
        public override void Update(Camera camera)
        {
                Music.FadeIn();
                Music.Repeat();
                oldState = Key;
                getKey();
                HP.Width = Ship.getHP() * 2;
                shield.Width = Ship.getShields() * shieldwidth;

            if (Key.IsKeyDown(Keys.P) && !oldState.IsKeyDown(Keys.P) && !Ship.getDead())
            {
                pause = !pause;
                if (pause)
                {
                    zoomerTimer.Stop();
                    cannonTimer.Stop();
                    asteroidTimer.Stop();
                    foreach (Entities cannon in cannonList)
                    {
                        cannon.AlterTime();
                    }
                }
                else
                {
                    zoomerTimer.Start();
                    cannonTimer.Start();
                    asteroidTimer.Start();
                    foreach (Entities cannon in cannonList)
                    {
                        cannon.AlterTime();
                    }
                }
            }
            if (!pause)
            {
                #region PreLoops
                Scroll();
                WallScroll();
                Ship.Update(Key);
                score++;

                if (Ship.getDead())
                {
                    Ship.Stop();
                    if(!stop)
                    finalscore = score;
                    stop = true;
                }

                if (!Ship.getDead())
                {
                    AddAsteroid();
                    Shoot();
                    AddCannon();
                    AddZoomer();
                }
                Ship.RealPos();
                #endregion 
                foreach (Entities cannon in cannonList.ToList())
                {
                    if (cannon.getDead())
                    {
                        cannonList.Remove(cannon);
                    }
                    if (!Ship.getDead())
                        CannonFire(cannon);
                    cannon.Update();
                    cannon.RealPos();
                    bool collision = Collision(cannon, Ship);
                    if (collision)
                    {
                        cannon.SetDead();
                        Ship.EnemyShipHit();
                        score -= 300;
                    }
                   
                }
                foreach (Entities zoomer in zoomerList.ToList())
                {
                    if (zoomer.getDead())
                    {
                        zoomerList.Remove(zoomer);
                    }
                    zoomer.Update();
                    zoomer.RealPos();
                    bool collision = Collision(Ship, zoomer);
                    if (collision)
                    {
                        zoomerList.Remove(zoomer);
                        Ship.EnemyShipHit();
                        score -= 200;
                    }
                }
                foreach (Polygons wall in wallList)
                {
                    wall.RealPos();
                    bool collision;
                    if (!Ship.getDead())
                    {
                        collision = Collision(Ship, wall);
                        if (collision)
                        {
                            if (wall == wall1 || wall == wall2)
                                Ship.Placement.Y = wall.Placement.Y + 57;

                            if (wall == wall3 || wall == wall4)
                                Ship.Placement.Y = wall.Placement.Y - 57;

                            Ship.Burn();
                        }
                    }
                    foreach (Polygons cannon in cannonList)
                    {
                        collision = Collision(cannon, wall);
                        if (collision)
                        {
                            if (wall == wall1 || wall == wall2)
                                cannon.Placement.Y = wall.Placement.Y + 59;

                            if (wall == wall3 || wall == wall4)
                                cannon.Placement.Y = wall.Placement.Y - 59;
                        }
                    }
                }
                foreach (Polygons asteroid in asteroidList.ToList())
                {
                    if (asteroid.getHP() <= 0)
                    {
                        asteroidList.Remove(asteroid);
                    }
                    asteroid.RealPos();
                    asteroid.Placement.X -= 10;
                    asteroid.Rotate(-.01f);
                    if (asteroid.Placement.X <= -200)
                    {
                        asteroidList.Remove(asteroid);
                    }

                    bool collision = Collision(Ship, asteroid);
                    if (collision)
                    {
                        Ship.AsteroidHit();
                        score -= 200;
                        asteroidList.Remove(asteroid);
                    }
                }
                foreach (Entities bullet in bulletList.ToList())
                {
                    bullet.Update();
                    bullet.RealPos();
                    bool collision;
                    if (bullet.Placement.X >= 2100)
                    {
                        bulletList.Remove(bullet);
                    }
                    foreach (Polygons asteroid in asteroidList)
                    {
                        collision = Collision(bullet, asteroid);
                        if (collision)
                        {
                            bulletList.Remove(bullet);
                            asteroid.addHP(-1);
                            score += 150;
                        }
                    }
                    foreach (Polygons cannon in cannonList)
                    {
                        collision = Collision(bullet, cannon);
                        if (collision)
                        {
                            bulletList.Remove(bullet);
                            cannon.addHP(-1);
                            score += 150;
                        }
                    }
                    foreach (Polygons zoomer in zoomerList)
                    {
                        collision = Collision(bullet, zoomer);
                        if (collision)
                        {
                            bulletList.Remove(bullet);
                            zoomer.addHP(-1);
                            score += 150;
                        }
                    }
                }
                foreach (Entities eBullet in enemyBulletList.ToList())
                {
                    if (eBullet.Placement.X < -100)
                    {
                        bulletList.Remove(eBullet);
                    }
                    eBullet.Placement += eBullet.getPath();
                    eBullet.RealPos();
                    bool collision = Collision(Ship, eBullet);
                    if (collision)
                    {
                        Ship.SmallBulletHit();
                        enemyBulletList.Remove(eBullet);
                        score -= 50;
                    }
                }
            }
        }
        //Holds Draw
        public override void Draw()
        {
            spriteBatch.Draw(backTex, position, Color.White);
            spriteBatch.Draw(backTex2, position2, Color.White);
            spriteBatch.Draw(backTex3, position3, Color.White);
            spriteBatch.Draw(backTex4, position4, Color.White);
            
            wall1.Draw(spriteBatch);
            wall2.Draw(spriteBatch);
            wall3.Draw(spriteBatch);
            wall4.Draw(spriteBatch);
            if (!Ship.getDead())
            { Ship.Draw(spriteBatch); }
            if (Ship.getDead())
            { Ship.DeathAnimation(spriteBatch); }

            foreach (Entities cannon in cannonList)
            {
                if(!cannon.getDead())
                cannon.Draw(spriteBatch);
                if(cannon.getDead())
                {
                    cannon.DeathAnimation(spriteBatch);
                }
            }
            foreach (Polygons zoomer in zoomerList)
            {
                zoomer.Draw(spriteBatch);
            }
            foreach (Polygons asteroid in asteroidList)
            {
                asteroid.Draw(spriteBatch);
            }
            foreach (Polygons bullet in bulletList)
            {
                bullet.Draw(spriteBatch);
            }
            foreach(Polygons eBullet in enemyBulletList)
            {
                eBullet.Draw(spriteBatch);
            }

            if (!Ship.getDead())
            { spriteBatch.DrawString(font, "Score: " + score, new Vector2(1, 100), Color.Red, 0, Vector2.Zero, 1.6f, SpriteEffects.None, 0); }
            else if (Ship.getDead())
            {
                spriteBatch.DrawString(font, "Score: " + finalscore, new Vector2(1, 100), Color.Red, 0, Vector2.Zero, 1.6f, SpriteEffects.None, 0);
            }
           
            spriteBatch.DrawString(font, "HP", new Vector2(20, 50), Color.Red, 0, Vector2.Zero, 1.7f, SpriteEffects.None, 0);
            spriteBatch.Draw(HPpix, HP, Color.White);
            spriteBatch.Draw(hpBorder, staticHP, Color.White);
            spriteBatch.Draw(hpBorder2, new Vector2(100, 55), Color.White);
            spriteBatch.Draw(hpBorder2, new Vector2(95 + staticHP.Width, 55), Color.White);
            spriteBatch.Draw(pixel, shield, Color.LightBlue);
            if (pause)
            {
                spriteBatch.DrawString(font, "PAUSED", new Vector2(720, 540), Color.GreenYellow, 0, Vector2.Zero, 4f, SpriteEffects.None, 0);
            }
            if (Ship.getDead())
            {
                spriteBatch.DrawString(font, "YOU HAVE DIED", new Vector2(720, 540), Color.Red, 0, Vector2.Zero, 3, SpriteEffects.None, 0);
            }

        }

        //Holds the Function
        public override void ButtonReset()
        {
       
        }

        private void MakeShapes()
        {
            RetrieveShapes();
            Ship = CreateShip("Base Ship");

            wall1 = CreateShape("wall");
            wall2 = CreateShape("wall");
            wall3 = CreateShape("wall");
            wall4 = CreateShape("wall");

        }
        private void Scroll()
        {
            position.X += -3;
            position2.X += -3;
            if (position.X <= -2097)
            {
                position.X = 2097;
            }
            if (position2.X <= -2097)
            {
                position2.X = 2097;
            }
            position3.X += -2;
            position4.X += -2;
            if (position3.X <= -1920)
            {
                position3.X = 1920;
            }
            if (position4.X <= -1920)
            {
                position4.X = 1920;
            }
        }
        private void WallScroll()
        {
            wall1.Placement.X += -5;
            wall2.Placement.X += -5;
            wall3.Placement.X += -5;
            wall4.Placement.X += -5;

            if (wall1.Placement.X <= -960)
            {
                wall1.Placement.X = 2880;
                wall3.Placement.X = 2880;
            }
            if (wall2.Placement.X <= -960)
            {
                wall2.Placement.X = 2880;
                wall4.Placement.X = 2880;
            }
            if (done)
            {
                done = false;
                done1 = false;
                done2 = false;
                wallPath1 = rand.Next(45, 195)*2;
                wallPath2 = rand.Next(335, 485)*2;

                if(wall1.Placement.Y < wallPath1)
                    wallMove1 = 1;
                else if(wall1.Placement.Y > wallPath1)
                    wallMove1 = -1;
                else
                    wallMove1 = 0;
                if (wall3.Placement.Y < wallPath2)
                    wallMove2 = 1;
                else if (wall3.Placement.Y > wallPath2)
                    wallMove2 = -1;
                else
                    wallMove2 = 0;
            }
            if (!done)
            {
                if (!done1)
                {
                    wall1.Placement.Y += wallMove1;
                    wall2.Placement.Y += wallMove1;
                }
                if (!done2)
                {
                    wall3.Placement.Y += wallMove2;
                    wall4.Placement.Y += wallMove2;
                }
                if (wall1.Placement.Y == wallPath1)                
                    done1 = true;
                
                if (wall3.Placement.Y == wallPath2)
                    done2 = true;

                if (wall1.Placement.Y == wallPath1 && wall3.Placement.Y == wallPath2)
                    done = true;

            }
        }
        private void AddAsteroid()
        {
            if (asteroidTimer.ElapsedMilliseconds >= 600) 
            {
                asteroidList.Add(CreatePolyInList("asteroid1", 2000, rand.Next(Convert.ToInt16(wall1.Placement.Y + 57), Convert.ToInt16(wall3.Placement.Y - 57)), 1));
                asteroidTimer.Restart();
            }
        }
        private void Shoot()
        {
            if (Key.IsKeyDown(Keys.Right) && !oldState.IsKeyDown(Keys.Right))
            {
                    bulletList.Add(CreateEntityInList("bullet", Convert.ToInt32(Ship.Placement.X + 40), Convert.ToInt32(Ship.Placement.Y)));
                    score -= 100;              
            }
        }
        private void AddCannon()
        {
            if (cannonTimer.ElapsedMilliseconds >= 1500)
            {
                cannonList.Add(CreateEntityInList("cannon", 2000, rand.Next(Convert.ToInt16(wall1.Placement.Y + 57), Convert.ToInt16(wall3.Placement.Y - 57))));
                cannonTimer.Restart();
            }
        }
        private void AddZoomer()
        {
            if (zoomerTimer.ElapsedMilliseconds >= 2000)
            {
                zoomerList.Add(CreateEntityInList("zoomer", 2000, rand.Next(Convert.ToInt16(wall1.Placement.Y + 57), Convert.ToInt16(wall3.Placement.Y - 57))));
                zoomerTimer.Restart();
            }
        }
        private void CannonFire(Entities cannon)
        {
            if (cannon.CanShoot())
                {
                enemyBulletList.Add(CreateEnemyInList("enemyBullet", Convert.ToInt16(cannon.Placement.X - 44), 
                    Convert.ToInt16(cannon.Placement.Y), Convert.ToInt16(Ship.Placement.X), Convert.ToInt16(Ship.Placement.Y)));
                }
        }
    }
}

