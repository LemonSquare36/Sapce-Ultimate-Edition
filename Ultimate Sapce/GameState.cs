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
    class GameState
    {
        Camera camera = new Camera();
        Vector3 screenScale = Vector3.Zero;
        Color color = Color.CornflowerBlue;
        KeyboardState key;
        SpriteBatch spriteBatch;
        SpriteFont font;
        GraphicsDeviceManager graphicsManager;
        RasterizerState r;
        Rectangle cutOff;
        bool loading;

        #region Declare the Screens
        public ScreenManager CurrentScreen;
        public MainMenu mainMenu;
        public IntoTheFray mainGame;
        #endregion
        //Constructor
        public GameState()
        {
            #region Initialize the Screens
            mainMenu = new MainMenu();
            mainGame = new IntoTheFray();
            #endregion
        }
        //Initialize things upon class creation
        public void Initialize()
        {
            loading = true;

            r = new RasterizerState();
            cutOff = new Rectangle(0,0,1344,756);
            if (CurrentScreen == null)
            {
                CurrentScreen = mainMenu;
            }
            CurrentScreen.Initialize();

            mainMenu.ChangeScreen += HandleScreenChanged;
            mainGame.ChangeScreen += PlayerChangeScreen;
        }
        //Loads the Content for The gamestate
        public void LoadContent(SpriteBatch spriteBatchMain, GraphicsDeviceManager graphicsManagerMain)
        {
            spriteBatch = spriteBatchMain;
            font = Main.GameContent.Load<SpriteFont>("myFont");
            graphicsManager = graphicsManagerMain;
          
            r.ScissorTestEnable = true;
            spriteBatch.GraphicsDevice.RasterizerState = r;
            spriteBatch.GraphicsDevice.ScissorRectangle = cutOff;

            CurrentScreen.LoadContent(spriteBatch);
            loading = false;
        }
        //The update function for gamestates and for using functions of the current screens
        public void Update()
        {
            if (!loading)
            {
                key = Keyboard.GetState();
                if (key.IsKeyDown(Keys.F1))
                {
                    camera.ChangeScreenSize(graphicsManager);
                }

                CurrentScreen.Update(camera);
            }
        }
        //Draws the images and textures we use
        public void Draw()
        {
            var viewMatrix = camera.Transform(Main.graphicsDevice);
             
            Main.graphicsDevice.Clear(color);

            if (!loading)
            {
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, null, null, r, null, viewMatrix * Matrix.CreateScale(1));
                CurrentScreen.Draw();
                spriteBatch.End();
            }
        }
        //The Event that Changes the Screens
        public void HandleScreenChanged(object sender, EventArgs eventArgs)
        {
            bool Load = true;
            //Next Screen is Based off the buttons Name (not garenteed to even load a new screen)
            switch (CurrentScreen.getNextScreen())
            {
                case "start":
                        CurrentScreen = mainGame;
                    break;
                case "main":
                    CurrentScreen = mainMenu;
                    break;
                default:
                    Load = false;
                    break;
            }
            //Resets the button on the screen
            CurrentScreen.ButtonReset();
            
            //Loads if a new screen is activated
            if (Load)
            {
                Initialize();
                LoadContent(spriteBatch, graphicsManager);
            }
        }
        public void PlayerChangeScreen(object sender, EventArgs eventArgs)
        {
            CurrentScreen = mainMenu;
            Initialize();
            LoadContent(spriteBatch, graphicsManager);
            CurrentScreen.ButtonReset();
        }
    }
}
