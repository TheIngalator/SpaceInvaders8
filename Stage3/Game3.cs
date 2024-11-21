/***********************************************************************************************************************************
 * 
 * In the third prototype of the SpaceInvaders program the cannon has been made interactive. The cannon is moved left, right, top and
 * bottom using  the arrow keys. There is no collision detection in the program so the cannon goes through the aliens. This will have
 * to be rectified in the next prototypes.
 * 
 * I. Palmer (18/08/2022)
 * 
 **********************************************************************************************************************************/
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Stage3
{
    public class Game3 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;



        Texture2D charaset;  // The Texture2D of the charaset.

        int sprite_w = 50;   // sprite width 
        int sprite_h = 50;   // sprite height

        float timer;            // A timer that stores milliseconds.
        int threshold;          // An int that is the threshold for the timer.

        int xFactor, yfactor, fireFactor;   // x & y distance increment values across the screen in one time frame
        int leftBoundary, rightBoundary, bottomBoundary; // boundary values for alien movement

        bool moveRight;

        // Rectangle arrays to store different alien images for animations.
        Rectangle[] spriteCol1;
        Rectangle[] spriteCol2;
        Rectangle[] spriteCol3;
        Rectangle[] currCol;

        // Rectangle arrays to store different images for the houses in different states of damage.
        Rectangle[] House;

        Rectangle cannon, cannonFire;
        Vector2 cannonPosition; // The position to draw the charaset.
        float cannonSpeed;
        bool cannonFired = false;



        // These bytes tell the spriteBatch.Draw() which sprite rows to display.
        byte row1, row2, row3;
        // These bytes tell the spriteBatch.Draw() which sprite columns to display.
        byte previousAnimationIndex;
        byte currentAnimationIndex;

        public Game3()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            cannonSpeed = 100f;
            base.Initialize();
        }

        protected override void LoadContent()
        {// TODO: use this.Content to load your game content here
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            // Load the charaset.
            charaset = Content.Load<Texture2D>("aliens256");
            // Set a default timer value.
            timer = 0;
            // Set an initial threshold of 500ms, you can change this to alter the speed of the animation (lower number = faster animation).
            threshold = 500;

            xFactor = yfactor = fireFactor = 0;
            leftBoundary = _graphics.PreferredBackBufferWidth / 3; //position aliens 1/3 across the screen
            rightBoundary = _graphics.PreferredBackBufferWidth;
            bottomBoundary = _graphics.PreferredBackBufferHeight;

            cannonPosition = new Vector2(leftBoundary + 200, bottomBoundary - 50);// Set the canon position
            cannon = new Rectangle(50, 150, sprite_w, sprite_h);
            cannonFire = new Rectangle(150, 150, sprite_w, sprite_h);
            moveRight = true;

            //Box coordindates for images of houses on the ground
            /*leftRoofDamage = new Rectangle[2];
            rightRoofDamage = new Rectangle[2];*/
            House = new Rectangle[4];

            //Store the spritesheet positions for the houses
            /* leftRoofDamage[0] = new Rectangle(32, 96, sprite_w, sprite_h);
             leftRoofDamage[1] = new Rectangle(64, 96, sprite_w, sprite_h);
             rightRoofDamage[0] = new Rectangle(32, 128, sprite_w, sprite_h);
             rightRoofDamage[1] = new Rectangle(64, 128, sprite_w, sprite_h); */
            House[0] = new Rectangle(0, 150, sprite_w, sprite_h);
            /*House[1] = new Rectangle(0, 250, sprite_w, sprite_h);
            House[2] = new Rectangle(0, 300, sprite_w, sprite_h);
            House[3] = new Rectangle(50, 150, sprite_w, sprite_h);*/


            // Store spritesheet positions for the first column of three aliens using an array
            spriteCol1 = new Rectangle[3];
            spriteCol1[0] = new Rectangle(0, 0, sprite_w, sprite_h);
            spriteCol1[1] = new Rectangle(0, 50, sprite_w, sprite_h);
            spriteCol1[2] = new Rectangle(0, 100, sprite_w, sprite_h);

            // Store spritesheet positions for the second column of three aliens using an array
            spriteCol2 = new Rectangle[3];
            spriteCol2[0] = new Rectangle(50, 0, sprite_w, sprite_h);
            spriteCol2[1] = new Rectangle(50, 50, sprite_w, sprite_h);
            spriteCol2[2] = new Rectangle(50, 100, sprite_w, sprite_h);

            // Store spritesheet positions for the third column of three aliens using an array
            spriteCol3 = new Rectangle[3];
            spriteCol3[0] = new Rectangle(100, 0, sprite_w, sprite_h);
            spriteCol3[1] = new Rectangle(100, 50, sprite_w, sprite_h);
            spriteCol3[2] = new Rectangle(100, 100, sprite_w, sprite_h);

            currCol = new Rectangle[3];

            // set sprite rows.
            row1 = 0;
            row2 = 1;
            row3 = 2;

            // This tells the animation to start with the first column of sprites.
            previousAnimationIndex = 2;
            currentAnimationIndex = 0;
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            var kstate = Keyboard.GetState();

            if (kstate.IsKeyDown(Keys.Up))
                cannonPosition.Y -= cannonSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (kstate.IsKeyDown(Keys.Down))
                cannonPosition.Y += cannonSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (kstate.IsKeyDown(Keys.Left))
                cannonPosition.X -= cannonSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (kstate.IsKeyDown(Keys.Right))
                cannonPosition.X += cannonSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (kstate.IsKeyDown(Keys.Space))
                cannonFired = true;

            // to confine the cannon within the screen width & height
            if (cannonPosition.X > _graphics.PreferredBackBufferWidth - sprite_w / 2)
                cannonPosition.X = _graphics.PreferredBackBufferWidth - sprite_w / 2;
            else if (cannonPosition.X < sprite_w / 2)
                cannonPosition.X = sprite_w / 2;

            if (cannonPosition.Y > _graphics.PreferredBackBufferHeight - sprite_h / 2)
                cannonPosition.Y = _graphics.PreferredBackBufferHeight - sprite_h / 2;
            else if (cannonPosition.Y < sprite_h / 2)
                cannonPosition.Y = sprite_h / 2;


            // TODO: Add your update logic here
            // Check if the timer has exceeded the threshold.
            if (timer > threshold)
            {
                // Track the animation.
                previousAnimationIndex = currentAnimationIndex;
                if (previousAnimationIndex == 2)
                    currentAnimationIndex = 0;
                else
                    currentAnimationIndex++;

                // Reset the timer.
                timer = 0;
                xFactor = xFactor + 10;   //used to change the x position of the aliens within one time frame
                fireFactor = fireFactor + 50;
            }
            // If the timer has not reached the threshold, then add the milliseconds that have past since the last Update() to the timer.
            else
            {
                timer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {// TODO: Add your drawing code here
            _spriteBatch.Begin();

            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            int x, y;

            // Draw the ground objects .
            drawGroundObjects();

            if (moveRight == true)
                x = leftBoundary - sprite_w + xFactor;   // change the x position of the leftmost alien
            else
                x = rightBoundary - sprite_w - xFactor;


            if ((x + (50 * 4) >= rightBoundary) && (moveRight == true))  // check if the fifth alien in a row is at the right boundary to change direction
            {
                moveRight = false;
                xFactor = 0;
                //yfactor = yfactor + 10;
            }

            if ((x <= leftBoundary) && (moveRight == false))  // check if the first alien in a row is at the left boundary to change direction
            {
                moveRight = true;
                xFactor = 0;
                //yfactor = yfactor + 10;
            }

            y = 20 + yfactor;

            switch (currentAnimationIndex)
            {
                case 0:
                    currCol = spriteCol1;
                    drawAliens(x, y);
                    break;

                case 1:
                    currCol = spriteCol2;
                    drawAliens(x, y);
                    break;

                case 2:
                    currCol = spriteCol3;
                    drawAliens(x, y);
                    break;
            }

            try
            {
                base.Draw(gameTime);
            }
            finally
            {
                _spriteBatch.End();
            }
        }

        void drawGroundObjects()
        {
            int x, y;
            x = leftBoundary;
            y = bottomBoundary;


            _spriteBatch.Draw(charaset, cannonPosition, cannon, Color.White); // draw the cannon

            if (cannonFired == true)
            {
                _spriteBatch.Draw(charaset, new Vector2(cannonPosition.X, cannonPosition.Y - fireFactor), cannonFire, Color.White); // draw the cannonfire
            }


            for (int i = 0; i < 3; i++)  //draw three houses
            {
                _spriteBatch.Draw(charaset, new Vector2(x + 80 * i, y - 60), House[0], Color.White); //draw 3 houses a few pixels apart
            }
        }
        void drawAliens(int x1, int y1)
        {
            int xWidth = 50;

            if (moveRight == false)
                xWidth = -50;

            // Draw the sprite rows based on the sourceRectangle and currentAnimationIndex .
            for (int i = 0; i < 5; i++)
            {
                _spriteBatch.Draw(charaset, new Vector2(x1 + xWidth * i, y1), currCol[row1], Color.White); // row 1: draw 5 aliens 50 pixels apart
            }

            y1 = 68 + yfactor;    //set row 2 y position
            for (int i = 0; i < 5; i++)
            {
                _spriteBatch.Draw(charaset, new Vector2(x1 + xWidth * i, y1), currCol[row2], Color.White); // row 2: draw 5 aliens 50 pixels apart
            }

            y1 = 118 + yfactor;        //set row 3 y position
            for (int i = 0; i < 5; i++)
            {
                _spriteBatch.Draw(charaset, new Vector2(x1 + xWidth * i, y1), currCol[row3], Color.White); // row 3: draw 5 aliens 50 pixels apart
            }

        }

        void fireCannon()
        {

        }
    }
}
