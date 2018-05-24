using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Ultimate_Sapce
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Main : Game
    {
        GameState gameState = new GameState();

        SpriteBatch spriteBatch;
        GraphicsDeviceManager graphicsManager;


        private FrameCounter framC = new FrameCounter();
        SpriteFont spritefont;

        //Allows other classes to load code from content manager - Convient
        private static ContentManager content;
        public static ContentManager GameContent
        {
            get { return content; }
            set { content = value; }
        }
        //
        private static GameWindow window;
        public static GameWindow gameWindow
        {
            get { return window; }
            set { window = value; }
        }
        private static GraphicsDevice graphics;
        public static GraphicsDevice graphicsDevice
        {
            get { return graphics; }
            set { graphics = value; }
        }


        //Constructor
        public Main()
        {
            Content.RootDirectory = "Content";
            content = Content;
            window = Window;
            IsMouseVisible = true;
            graphicsManager = new GraphicsDeviceManager(this);

            graphicsManager.PreferredBackBufferWidth = 1344;
            graphicsManager.PreferredBackBufferHeight = 756;
            graphicsManager.ApplyChanges();

        }
        protected override void Initialize()
        {
            graphics = GraphicsDevice;
            spriteBatch = new SpriteBatch(GraphicsDevice);

            gameState.Initialize();
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            gameState.LoadContent(spriteBatch, graphicsManager);
            spritefont = GameContent.Load<SpriteFont>("myFont");
        }


        protected override void UnloadContent()
        {
        }


        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            gameState.Update();

            base.Update(gameTime);
        }


        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            gameState.Draw();

            var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            framC.Update(deltaTime);

            var fps = string.Format("FPS: {0}", framC.AverageFramesPerSecond);

            spriteBatch.Begin();
            spriteBatch.DrawString(spritefont, fps, new Vector2(1, 1), Color.Green);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
