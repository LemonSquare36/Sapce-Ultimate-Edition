using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;


namespace Ultimate_Sapce
{
    public class Camera
    {

        public float Zoom { get; set; }
        public Vector2 Position { get; set; }
        public float Rotation { get; set; }
        public Vector2 Origin { get; set; }
        //Constructor
        public Camera()
        {
            Zoom = 1f;
            Position = Vector2.Zero;
            Rotation = 0;
            Origin = Vector2.Zero;
        }
        //MOves the Camera with arrow keys
        public void Move(KeyboardState CurrentKeyBoardState)
        {
            if (CurrentKeyBoardState.IsKeyDown(Keys.Right))
            {
                Position = new Vector2(Position.X - 10, Position.Y);
            }
            if (CurrentKeyBoardState.IsKeyDown(Keys.Left))
            {
                Position = new Vector2(Position.X + 10, Position.Y);
            }
            if (CurrentKeyBoardState.IsKeyDown(Keys.Up))
            {
                Position = new Vector2(Position.X, Position.Y + 10);
            }
            if (CurrentKeyBoardState.IsKeyDown(Keys.Down))
            {
                Position = new Vector2(Position.X, Position.Y - 10);
            }
        }
        //Cerates what the camera actaully sees. This inculdes scale,zoom, rotaion. Done in a matrix
        public Matrix Transform(GraphicsDevice graphicsDevice)
        {
            //game is scaled to these amounts yo
            var scaleX = (float)graphicsDevice.Viewport.Width / 1920;
            var scaleY = (float)graphicsDevice.Viewport.Height / 1080;

            var translationMatrix = Matrix.CreateTranslation(new Vector3(Position.X, Position.Y, 0));
            var rotationMatrix = Matrix.CreateRotationZ(Rotation);
            var scaleMatrix = Matrix.CreateScale(new Vector3(scaleX, scaleY, 1));
            var originMatrix = Matrix.CreateTranslation(new Vector3(Origin.X, Origin.Y, 0));

            return translationMatrix * rotationMatrix * scaleMatrix * originMatrix;
        }
        //Toggle full screen or not
        public void ChangeScreenSize(GraphicsDeviceManager graphics)
        {
            //Super nice funtion :D
            graphics.ToggleFullScreen();

            graphics.ApplyChanges();

        }
        //Camera Follows the position provide (inteneded for character)
        public void Follow(Vector2 characterPosition)
        {
            Position = new Vector2(characterPosition.X + 800f, characterPosition.Y + 400f);
        }
        public void posReset()
        {
            Position = Vector2.Zero;
        }
    }
}
