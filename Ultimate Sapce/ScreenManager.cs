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
    class ScreenManager
    {
        protected static Hashtable shapeVerts = new Hashtable();

        protected KeyboardState Key;
        protected SpriteBatch spriteBatch;
        protected string nextScreen;
        Random rand = new Random();
        protected bool pause = false;
        protected SoundManager Music;

        //Holds Initialize
        public virtual void Initialize()
        {

        }
        //Holds LoadContent and the font if called
        public virtual void LoadContent(SpriteBatch spriteBatchmain)
        {

        }
        //Holds Update
        public virtual void Update(Camera camera)
        {

        }
        //Holds Draw
        public virtual void Draw()
        {
            
        }
        public void getKey()
        {
            Key = Keyboard.GetState();
        }

        //Holds the Function
        public virtual void ButtonReset()
        {

        }
        //Gets the next screen
        public string getNextScreen()
        {
            return nextScreen;
        }

        #region AreaManager
        ///////////////////////////////AREA MANAGER STUFF//////////////////////////////////

        //Uses the Positions from Shape list to make collision
        protected bool Collision(Polygons Shape, Polygons Shape2)
        {
            bool collision = true;
            bool inrange = false;
            bool notinrange = false;
            double Range = Shape.getRange() + Shape2.getRange();

            if (Math.Abs(Distance(Shape.getRealPos(0), Shape2.getRealPos(0))) < Range)
            {
                inrange = true;
            }

            if (inrange)
            {
                // Y is for the verticies one higher than i; I named it Y since it rhymes with i;
                int Y = 2;
                // Z is the same as Y but for Shape2; Named that since it is after Y;
                int Z = 2;

                for (int i = 1; i < Shape.getNumVerticies(); i++)
                {
                    if (Y == Shape.getNumVerticies())
                    {
                        Y = 1;
                    }
                    if (!Shape.Projection(Shape2, Shape.NormalVector(i, Y)))
                    {
                        collision = false;
                    }
                    Y++;
                }

                for (int i = 1; i < Shape2.getNumVerticies(); i++)
                {
                    if (Z == Shape2.getNumVerticies())
                    {
                        Z = 1;
                    }
                    if (!Shape2.Projection(Shape, Shape2.NormalVector(i, Z)))
                    {
                        collision = false;
                    }
                    Z++;
                }

                return collision;
            }
            return notinrange;
        }
        //Creates the Shapes of Polygon Class
        protected Polygons CreateShape(string shapeName)
        {
            List<Vector2> NewList = (List<Vector2>)shapeVerts[shapeName];
            Polygons myPolygon = new Polygons(NewList);
            return myPolygon;
        }
        protected PlayerShip CreateShip(string shapeName)
        {
            List<Vector2> NewList = (List<Vector2>)shapeVerts[shapeName];
            PlayerShip myPolygon = new PlayerShip(NewList);
            return myPolygon;
        }
        protected Entities CreateEnemies(string shapeName)
        {
            List<Vector2> NewList = (List<Vector2>)shapeVerts[shapeName];
            Entities myPolygon = new Entities(NewList);
            return myPolygon;
        }
        protected Entities CreateEntityInList(string shapeName, int X, int Y)
        {
            List<Vector2> NewList = (List<Vector2>)shapeVerts[shapeName];
            Entities poly = new Entities(NewList);
            poly.RealPos();
            poly.LoadContent(X, Y, shapeName);
            return poly;
        }
        protected Entities CreateEnemyInList(string shapeName, int X, int Y, int X2, int Y2)
        {
            List<Vector2> NewList = (List<Vector2>)shapeVerts[shapeName];
            Entities poly = new Entities(NewList);
            poly.RealPos();
            poly.LoadContent(X, Y, shapeName);
            poly.SetPath(X2, Y2);
            return poly;
        }
        protected Polygons CreatePolyInList(string shapeName, int X, int Y, int HP)
        {
            List<Vector2> NewList = (List<Vector2>)shapeVerts[shapeName];
            Polygons poly = new Polygons(NewList);
            poly.RealPos();
            poly.LoadContent(X, Y, shapeName);
            poly.addHP(HP);
            return poly;
        }

        //Gets the Hit boxes from Shape List or Enemy List
        protected void RetrieveShapes()
        {
            string Resource = "Shapes/shapeList.txt";
            StreamReader shapeConfig = new StreamReader(Path.Combine(Main.GameContent.RootDirectory, Resource));

            string line;
            string key = "";
            List<Vector2> verticies = new List<Vector2>();
            while ((line = shapeConfig.ReadLine()) != null)
            {
                try
                {
                    string[] VertCords = (line.Split(','));
                    float xVert = (float)Convert.ToDouble(VertCords[0]);
                    float yVert = (float)Convert.ToDouble(VertCords[1]);
                    Vector2 myVector2 = new Vector2(xVert, yVert);
                    verticies.Add(myVector2);

                }
                catch
                {

                    if (key != null)
                    {
                        shapeVerts[key] = verticies;
                        verticies = new List<Vector2>();
                    }
                    key = line;
                }
            }
            shapeConfig.Close();
        }

        //distance calc
        protected double Distance(Vector2 point1, Vector2 point2)
        {
            double X = Math.Pow((point2.X - point1.X), 2);
            double Y = Math.Pow((point2.Y - point1.Y), 2);

            double unit = Math.Sqrt(X + Y);
            return unit;
        }

        protected void OnScreenChanged(object sender, EventArgs eventArgs)
        {
            Music.Stop();
            ChangeScreen?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        #region MenuManager
        ///////////////////////////////MENU MANAGER STUFF////////////////////////
        protected MouseState mouse;

        //ButtonCLicked leads Here
        protected void ButtonClicked(object sender, EventArgs e)
        {
            //Sets next screen to button name and calls the event.
            switch (((Button)sender).purpose)
            {
                case 0:
                    nextScreen = ((Button)sender).bName;
                    OnScreenChanged();
                    break;
            }
        }
        //Event for Changing the Screen
        public event EventHandler ChangeScreen;
        public event EventHandler buttonPressed;
        public void OnScreenChanged()
        {
            ChangeScreen?.Invoke(this, EventArgs.Empty);
        }
        public void OnButtonPressed()
        {
            buttonPressed?.Invoke(this, EventArgs.Empty);
        }
        protected Vector2 MousePos()
        {
            Vector2 worldPosition = Vector2.Zero;
            mouse = Mouse.GetState();
            try
            {
                worldPosition.X = mouse.X / (float)(Main.gameWindow.ClientBounds.Width / 1920.0);
                worldPosition.Y = mouse.Y / (float)(Main.gameWindow.ClientBounds.Height / 1080.0);
            }
            catch { }
            return worldPosition;
        }
        #endregion
    }
}
