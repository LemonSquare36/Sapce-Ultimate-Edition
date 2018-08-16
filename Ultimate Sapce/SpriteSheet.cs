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
    class SpriteSheet
    {
        public Texture2D Texture { get; set; }
        public int Rows { get; set; }
        public int Columns { get; set; }
        private int currentFrame;
        private int totalFrames;
        int often = 0;
        float realLocation;
        private bool done = false;

        public SpriteSheet(string tex, int rows, int col)
        {
            Texture = Main.GameContent.Load<Texture2D>("Animation/"+ tex);
            Rows = rows;
            Columns = col;
            currentFrame = 0;
            totalFrames = Rows * Columns;

        }
        public void Update(int ofteness, bool repeat)
        {

            if (currentFrame != totalFrames)
            {
                often++;
                if (often == ofteness)
                {
                    currentFrame++;
                    often = 0;
                }
            }

            if (currentFrame == totalFrames)
            {
                 done = true;
                if (repeat)
                {
                    currentFrame = 0;
                    done = false;
                }
            }
               
        }
        public void Draw(SpriteBatch spriteBatch, Vector2 location, float scale)
        {
            int width = Texture.Width / Columns;
            int height = Texture.Height / Rows;
            int row = (int)((float)currentFrame / (float)Columns);
            int column = currentFrame % Columns;

            Rectangle sourceRectangle = new Rectangle(width * column, height * row, width, height);
            Rectangle destinationRectangle = new Rectangle((int)location.X, (int)location.Y, width, height);

            spriteBatch.Draw(Texture, location, sourceRectangle, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0);
        }
        public bool getDone()
        {
            return done;
        }

    }
}
