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
        Texture2D backTex, backTex2, backTex3, backTex4;
        PlayerShip Ship;
        Vector2 position, position2, position3, position4;
        SpriteFont font;
        Polygons topwall1, topwall2, topwall3, topwall4, topwall5, topwall6, botwall1, botwall2, botwall3, botwall4, botwall5, botwall6;
        List<Polygons> wallList, asteroidList;
        List<Entities> cannonList, enemyBulletList, zoomerList, bulletList;
        Random rand;
        int wallPath1, wallPath2, wallMove1, wallMove2;
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

            MakeShapes();
            Ship.LoadContent(200, 540, "Base Ship");
            #region wallload
            topwall1.LoadContent(0, 200, "Wall/topwall1");
            topwall2.LoadContent(480, 200, "Wall/topwall2");
            topwall3.LoadContent(960, 200, "Wall/topwall1");
            topwall4.LoadContent(1440, 200, "Wall/topwall2");
            topwall5.LoadContent(1920, 200, "Wall/topwall1");
            topwall6.LoadContent(2400, 200, "Wall/topwall2");
            botwall1.LoadContent(0, 800, "Wall/botwall1");
            botwall2.LoadContent(480, 800, "Wall/botwall2");
            botwall3.LoadContent(960, 800, "Wall/botwall1");
            botwall4.LoadContent(1440, 800, "Wall/botwall2");
            botwall5.LoadContent(1920, 800, "Wall/botwall1");
            botwall6.LoadContent(2400, 800, "Wall/botwall2");
            #endregion  
            #region wallList
            wallList.Add(topwall1);
            wallList.Add(topwall2);
            wallList.Add(topwall3);
            wallList.Add(topwall4);
            wallList.Add(topwall5);
            wallList.Add(topwall6);
            wallList.Add(botwall1);
            wallList.Add(botwall2);
            wallList.Add(botwall3);
            wallList.Add(botwall4);
            wallList.Add(botwall5);
            wallList.Add(botwall6);
            #endregion
            Ship.ChangeScreen += OnScreenChanged;
        }
        //Holds Update
        public override void Update(Camera camera)
        {
                Music.FadeIn();
                Music.Repeat();
                oldState = Key;
                getKey();
                

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

                if (Ship.getDead())
                {
                    Ship.Stop();
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
                    if (!cannon.getDead())
                    {

                        if (!Ship.getDead())
                            CannonFire(cannon);
                        cannon.Update();
                        cannon.RealPos();
                        bool collision = Collision(cannon, Ship);
                        if (collision)
                        {
                            cannon.SetDead();
                            Ship.EnemyShipHit();
                            Ship.addscore(-300);
                        }
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
                        Ship.addscore(-200);
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
                            if (Ship.Placement.Y < 480)
                                Ship.Placement.Y = wall.Placement.Y + 57;

                            if (Ship.Placement.Y > 480)
                                Ship.Placement.Y = wall.Placement.Y - 57;

                            Ship.Burn();
                        }
                    }
                    foreach (Entities cannon in cannonList)
                    {
                        if (!cannon.getDead())
                        {
                            collision = Collision(cannon, wall);
                            if (collision)
                            {
                                if (cannon.Placement.Y < 480)
                                    cannon.Placement.Y = wall.Placement.Y + 59;

                                if (cannon.Placement.Y > 480)
                                    cannon.Placement.Y = wall.Placement.Y - 59;
                            }
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
                        Ship.addscore(-200);
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
                            Ship.addscore(150);
                        }
                    }
                    foreach (Entities cannon in cannonList)
                    {
                        if (!cannon.getDead())
                        {
                            collision = Collision(bullet, cannon);
                            if (collision)
                            {
                                bulletList.Remove(bullet);
                                cannon.addHP(-1);
                                Ship.addscore(150);
                            }
                        }
                    }
                    foreach (Polygons zoomer in zoomerList)
                    {
                        collision = Collision(bullet, zoomer);
                        if (collision)
                        {
                            bulletList.Remove(bullet);
                            zoomer.addHP(-1);
                            Ship.addscore(150);
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
                        Ship.addscore(-50);
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
            
            topwall1.Draw(spriteBatch); topwall2.Draw(spriteBatch); topwall3.Draw(spriteBatch); topwall4.Draw(spriteBatch); topwall5.Draw(spriteBatch); topwall6.Draw(spriteBatch);
            botwall1.Draw(spriteBatch); botwall2.Draw(spriteBatch); botwall3.Draw(spriteBatch); botwall4.Draw(spriteBatch); botwall5.Draw(spriteBatch); botwall6.Draw(spriteBatch);

            if (!Ship.getDead())
            { Ship.Draw(spriteBatch); }
            if (Ship.getDead())
            { Ship.DeathAnimation(spriteBatch); }

            foreach (Entities cannon in cannonList.ToList())
            {
                if(!cannon.getDead())
                cannon.Draw(spriteBatch);
                if(cannon.getDead())
                {
                    cannon.PlayDeathSound();
                    cannon.DeathAnimation(spriteBatch); 
                    if (cannon.getDeathAniDone())
                    {
                        cannonList.Remove(cannon);
                    }
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

            Ship.DrawHUD(pause, spriteBatch, font);

        }

        //Holds the Function
        public override void ButtonReset()
        {
       
        }

        private void MakeShapes()
        {
            RetrieveShapes();
            Ship = CreateShip("Base Ship");

            topwall1 = CreateShape("wall"); topwall2 = CreateShape("wall"); topwall3 = CreateShape("wall"); topwall4 = CreateShape("wall");
            topwall5 = CreateShape("wall"); topwall6 = CreateShape("wall"); 
            botwall1 = CreateShape("wall"); botwall2 = CreateShape("wall"); botwall3 = CreateShape("wall"); botwall4 = CreateShape("wall");
            botwall5 = CreateShape("wall"); botwall6 = CreateShape("wall"); 

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
            topwall1.Placement.X += -5; topwall2.Placement.X += -5; topwall3.Placement.X += -5; topwall4.Placement.X += -5;
            topwall5.Placement.X += -5; topwall6.Placement.X += -5; 
            botwall1.Placement.X += -5; botwall2.Placement.X += -5; botwall3.Placement.X += -5; botwall4.Placement.X += -5;
            botwall5.Placement.X += -5; botwall6.Placement.X += -5;
            #region wallresets
            if (topwall1.Placement.X <= -480)
            {
                topwall1.Placement.X = 2400;
                botwall1.Placement.X = 2400;
            }
            if (topwall2.Placement.X <= -480)
            {
                topwall2.Placement.X = 2400;
                botwall2.Placement.X = 2400;
            }
            if (topwall3.Placement.X <= -480)
            {
                topwall3.Placement.X = 2400;
                botwall3.Placement.X = 2400;
            }
            if (topwall4.Placement.X <= -480)
            {
                topwall4.Placement.X = 2400;
                botwall4.Placement.X = 2400;
            }
            if (topwall5.Placement.X <= -480)
            {
                topwall5.Placement.X = 2400;
                botwall5.Placement.X = 2400;
            }
            if (topwall6.Placement.X <= -480)
            {
                topwall6.Placement.X = 2400;
                botwall6.Placement.X = 2400;
            }
            #endregion
            if (done)
            {
                done = false;
                done1 = false;
                done2 = false;
                wallPath1 = rand.Next(45, 195)*2;
                wallPath2 = rand.Next(335, 485)*2;

                if(topwall1.Placement.Y < wallPath1)
                    wallMove1 = 1;
                else if(topwall1.Placement.Y > wallPath1)
                    wallMove1 = -1;
                else
                    wallMove1 = 0;
                if (botwall1.Placement.Y < wallPath2)
                    wallMove2 = 1;
                else if (botwall1.Placement.Y > wallPath2)
                    wallMove2 = -1;
                else
                    wallMove2 = 0;
            }
            if (!done)
            {
                if (!done1)
                {
                    topwall1.Placement.Y += wallMove1;
                    topwall2.Placement.Y += wallMove1;
                    topwall3.Placement.Y += wallMove1;
                    topwall4.Placement.Y += wallMove1;
                    topwall5.Placement.Y += wallMove1;
                    topwall6.Placement.Y += wallMove1;
                }
                if (!done2)
                {
                    botwall1.Placement.Y += wallMove2;
                    botwall2.Placement.Y += wallMove2;
                    botwall3.Placement.Y += wallMove2;
                    botwall4.Placement.Y += wallMove2;
                    botwall5.Placement.Y += wallMove2;
                    botwall6.Placement.Y += wallMove2;
                }
                if (topwall1.Placement.Y == wallPath1)                
                    done1 = true;
                
                if (botwall1.Placement.Y == wallPath2)
                    done2 = true;

                if (topwall1.Placement.Y == wallPath1 && botwall1.Placement.Y == wallPath2)
                    done = true;

            }
        }
        private void AddAsteroid()
        {
            if (asteroidTimer.ElapsedMilliseconds >= 600) 
            {
                asteroidList.Add(CreatePolyInList("asteroid1", 2000, rand.Next(Convert.ToInt16(topwall1.Placement.Y + 57), Convert.ToInt16(botwall1.Placement.Y - 57)), 1));
                asteroidTimer.Restart();
            }
        }
        private void Shoot()
        {
            if (Key.IsKeyDown(Keys.Right) && !oldState.IsKeyDown(Keys.Right))
            {
                    bulletList.Add(CreateEntityInList("bullet", Convert.ToInt32(Ship.Placement.X + 40), Convert.ToInt32(Ship.Placement.Y)));
                    Ship.addscore(-85);              
            }
        }
        private void AddCannon()
        {
            if (cannonTimer.ElapsedMilliseconds >= 1500)
            {
                cannonList.Add(CreateEntityInList("cannon", 2000, rand.Next(Convert.ToInt16(topwall1.Placement.Y + 57), Convert.ToInt16(botwall1.Placement.Y - 57))));
                cannonTimer.Restart();
            }
        }
        private void AddZoomer()
        {
            if (zoomerTimer.ElapsedMilliseconds >= 2000)
            {
                zoomerList.Add(CreateEntityInList("zoomer", 2000, rand.Next(Convert.ToInt16(topwall1.Placement.Y + 57), Convert.ToInt16(botwall1.Placement.Y - 57))));
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

