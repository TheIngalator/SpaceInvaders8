/***********************************************************************************************************************************
 * 
 * This is the first prototype for the SpaceInvaders program. The program displays a sprites sheet showing 9 sprites.
 * 
 * I. Palmer (18/08/2022)
 * 
 **********************************************************************************************************************************/
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Stage1
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;



        Texture2D charaset;  // The Texture2D of the charaset.
        //Vector2 position; // The position to draw the charaset.
        int sprite_w = 32;   // sprite width 
        int sprite_h = 32;   // sprite height

        // A Rectangle array that stores spriteCol1 for animations.
        Rectangle[] spriteCol1;
        Rectangle[] spriteCol2;
        Rectangle[] spriteCol3;
        // These bytes tell the spriteBatch.Draw() what sourceRectangle to display.
        byte row1, row2, row3;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // TODO: use this.Content to load your game content here
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            // Load the charaset.
            charaset = Content.Load<Texture2D>("Aliens");
            // Set the draw position.
            //position = new Vector2(100, 100);

            // Three sprite rectangles containing the coordinates of the first column of sprites
            spriteCol1 = new Rectangle[3];
            spriteCol1[0] = new Rectangle(0, 0, sprite_w, sprite_h);
            spriteCol1[1] = new Rectangle(0, 32, sprite_w, sprite_h);
            spriteCol1[2] = new Rectangle(0, 64, sprite_w, sprite_h);

            // Three sprite rectangles containing the coordinates of the second column of sprites
            spriteCol2 = new Rectangle[3];
            spriteCol2[0] = new Rectangle(32, 0, sprite_w, sprite_h);
            spriteCol2[1] = new Rectangle(32, 32, sprite_w, sprite_h);
            spriteCol2[2] = new Rectangle(32, 64, sprite_w, sprite_h);

            // Three sprite rectangles containing the coordinates of the third column of sprites
            spriteCol3 = new Rectangle[3];
            spriteCol3[0] = new Rectangle(64, 0, sprite_w, sprite_h);
            spriteCol3[1] = new Rectangle(64, 32, sprite_w, sprite_h);
            spriteCol3[2] = new Rectangle(64, 64, sprite_w, sprite_h);

            // set sprite rows.
            row1 = 0;
            row2 = 1;
            row3 = 2;
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            // TODO: Add your drawing code here

            // base.Draw(gameTime);

            _spriteBatch.Begin();

            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            drawAliens();

            try
            {
                base.Draw(gameTime);
            }
            finally
            {
                _spriteBatch.End();
            }
        }

        void drawAliens()
        {
            int x = _graphics.PreferredBackBufferWidth / 3 - sprite_w; //position the sprite quarter way across window width
            int y = 20;

            // Draw the sprite rows based on the sourceRectangle and currentAnimationIndex .
            for (int i = 0; i < 5; i++)
            {
                _spriteBatch.Draw(charaset, new Vector2(x + 32 * i, y), spriteCol1[row1], Color.White);  //Draw five sprites in row 1
            }

            y = 58;
            for (int i = 0; i < 5; i++)
            {
                _spriteBatch.Draw(charaset, new Vector2(x + 32 * i, y), spriteCol1[row2], Color.White); //Draw five sprites in row 2
            }

            y = 96;
            for (int i = 0; i < 5; i++)
            {
                _spriteBatch.Draw(charaset, new Vector2(x + 32 * i, y), spriteCol1[row3], Color.White); //Draw five sprites in row 3
            }

        }


    }
}
