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
    class MainMenu : ScreenManager
    {
        Texture2D backTex, titleTex;
        Texture2D startUH, startH;
        Button Start;

        //Holds Initialize
        public override void Initialize()
        {

        }
        //Holds LoadContent and the font if called
        public override void LoadContent(SpriteBatch spriteBatchmain)
        {
            spriteBatch = spriteBatchmain;

            backTex = Main.GameContent.Load<Texture2D>("MenuImages/Starbackground");
            titleTex = Main.GameContent.Load<Texture2D>("MenuImages/SapceUltimate");

            #region ButtonStuff
            startUH = Main.GameContent.Load<Texture2D>("MenuImages/Buttons/StartUH");
            startH = Main.GameContent.Load<Texture2D>("MenuImages/Buttons/StartH");

            Start = new Button(new Vector2(815, 500), 300, 160, startUH, startH, "start", 0);

            Start.ButtonClicked += ButtonClicked;
            #endregion

        }
        //Holds Update
        public override void Update(Camera camera)
        {
            Vector2 worldPosition = MousePos();

            Start.Update(mouse, worldPosition);
        }
        //Holds Draw
        public override void Draw()
        {
            spriteBatch.Draw(backTex, new Vector2(0, 0), Color.White);
            spriteBatch.Draw(titleTex, new Vector2(563,100), Color.White);

            Start.Draw(spriteBatch);
        }

        //Holds the Function
        public override void ButtonReset()
        {
            Start.ButtonReset();
        }
    }
}
