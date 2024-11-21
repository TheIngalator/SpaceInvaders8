﻿/***********************************************************************************************************************************
 * Prototype 6: Scoreboard
 * In this prototype a player scoreboard will be added
 *
 *  
 * 30/12/2022
 * 
 **********************************************************************************************************************************/
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections;

namespace Stage6
{
    public class Game6 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private SpriteFont font;
        private int score = 0;

        Texture2D charaset;  // The Texture2D of the charaset.

        int sprite_w = 50;   // sprite width 
        int sprite_h = 50;   // sprite height

        float timer;            // A timer that stores milliseconds.
        int timerResetCount;
        int threshold;          // An int that is the threshold for the timer.

        int xFactor, yFactor, fireFactor;   // x & y distance increment values across the screen in one time frame
        int leftBoundary, rightBoundary, bottomBoundary; // boundary values for alien movement

        // Rectangle arrays to store different alien images for animations.
        Rectangle[,] spritesGrid;
        ArrayList randSprites = new ArrayList();    // this will store random sprites
        ArrayList randSpritesPos = new ArrayList();    // this will store the random sprites positions

        int rand_x, rand_y;

        Boolean gameOver;

        Rectangle cannon, cannonFire;
        Vector2 cannonPosition, firePos; // The position to draw the charaset.
        float cannonSpeed;
        bool cannonFired = false;


        public Game6()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            gameOver = false;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            cannonSpeed = 100f;
            base.Initialize();
        }

        protected override void LoadContent()  // TODO: use this.Content to load your game content here
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            font = Content.Load<SpriteFont>("Score");     // load the font from a file named Score

            // Load the charaset.
            charaset = Content.Load<Texture2D>("aliens256");
            // Set a default timer value.
            timer = 0;
            timerResetCount = 0;
            // Set an initial threshold of 500ms, you can change this to alter the speed of the animation (lower number = faster animation).
            threshold = 500;

            xFactor = fireFactor = 0;
            yFactor = 20;
            leftBoundary = _graphics.PreferredBackBufferWidth / 3; //position aliens 1/3 across the screen
            rightBoundary = _graphics.PreferredBackBufferWidth;
            bottomBoundary = _graphics.PreferredBackBufferHeight;

            cannonPosition = new Vector2(leftBoundary + 200, bottomBoundary - 50);// Set the canon position
            cannon = new Rectangle(50, 150, sprite_w, sprite_h);
            cannonFire = new Rectangle(100, 150, sprite_w, sprite_h);


            spritesGrid = new Rectangle[3, 3];    // 2d array to store 9 sprites

            spritesGrid[0, 0] = new Rectangle(0, 0, sprite_w, sprite_h);
            spritesGrid[1, 0] = new Rectangle(0, 50, sprite_w, sprite_h);
            spritesGrid[2, 0] = new Rectangle(0, 100, sprite_w, sprite_h);
            spritesGrid[0, 1] = new Rectangle(50, 0, sprite_w, sprite_h);
            spritesGrid[1, 1] = new Rectangle(50, 50, sprite_w, sprite_h);
            spritesGrid[2, 1] = new Rectangle(50, 100, sprite_w, sprite_h);
            spritesGrid[0, 2] = new Rectangle(100, 0, sprite_w, sprite_h);
            spritesGrid[1, 2] = new Rectangle(100, 50, sprite_w, sprite_h);
            spritesGrid[2, 2] = new Rectangle(100, 100, sprite_w, sprite_h);

            spawnRandomSprite();
        }

        protected override void Update(GameTime gameTime)   //add your update logic here
        {
            Random rnd = new Random();
            int randomInterval = rnd.Next(2, 8);      //this value will be used to control random interval of spawning aliens

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            var kstate = Keyboard.GetState();

            if (kstate.IsKeyDown(Keys.Left))
                cannonPosition.X -= cannonSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (kstate.IsKeyDown(Keys.Right))
                cannonPosition.X += cannonSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (kstate.IsKeyDown(Keys.Space) && (cannonFired == false))
            {
                firePos.X = cannonPosition.X;
                firePos.Y = cannonPosition.Y - 20;
                cannonFired = true;
            }

            // to confine the cannon within the screen width & height
            if (cannonPosition.X > _graphics.PreferredBackBufferWidth - sprite_w / 2)
                cannonPosition.X = _graphics.PreferredBackBufferWidth - sprite_w / 2;
            else if (cannonPosition.X < sprite_w / 2)
                cannonPosition.X = sprite_w / 2;

            if (cannonPosition.Y > _graphics.PreferredBackBufferHeight - sprite_h / 2)
                cannonPosition.Y = _graphics.PreferredBackBufferHeight - sprite_h / 2;
            else if (cannonPosition.Y < sprite_h / 2)
                cannonPosition.Y = sprite_h / 2;

            if ((timer > threshold / 4) && (cannonFired == true))   //using threshold/4 to set speed of cannon fire
            {
                fireFactor = +10;
                firePos.Y -= fireFactor;

                if ((firePos.Y <= 0) || (alienCollision() == true))  // check for alien collision or fire reaching boundary
                {
                    fireFactor = 0;
                    cannonFired = false;
                }

            }


            if (timer > threshold)  // Check if the timer has exceeded the threshold.
            {
                timer = 0;      // Reset the timer.
                timerResetCount++;          // Track the animation.

                //used to change the y position of the aliens within one time frame
                for (int i = 0; i < randSprites.Count; i++)
                {
                    Vector2 rndPos = (Vector2)randSpritesPos[i];
                    rndPos.Y += yFactor;
                    randSpritesPos[i] = rndPos;
                }

                if ((timerResetCount % randomInterval == 0) && (gameOver == false))
                    spawnRandomSprite();

            }

            // If the timer has not reached the threshold, then add the milliseconds that have past since the last Update() to the timer.
            else
            {
                timer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            }


            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)  // Add your drawing code here
        {
            _spriteBatch.Begin();

            showScores();

            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Draw(charaset, cannonPosition, cannon, Color.White); // draw the cannon

            if (cannonFired == true)
                fireCannon();

            drawAliens();

            for (int i = 0; i < randSprites.Count; i++)
            {
                Vector2 thisSprite = (Vector2)randSpritesPos[i];

                thisSprite.Y += yFactor;
                if (thisSprite.Y >= bottomBoundary)     // if this sprite has reached the bottom boundary
                {
                    gameOver = true;

                }
            }

            if (gameOver == true)
            {
                for (int i = 0; i < randSprites.Count; i++)
                {
                    randSprites.RemoveAt(i);
                    randSpritesPos.RemoveAt(i);
                    //this.Exit();
                }
                displayGameOverMsg();
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

        void fireCannon()
        {
            _spriteBatch.Draw(charaset, firePos, cannonFire, Color.White); // draw the cannonfire   
        }
        void drawAliens()
        {
            for (int i = 0; i < randSprites.Count; i++)
            {
                Rectangle currSprite = new Rectangle();
                currSprite = (Rectangle)randSprites[i];  // get a sprite from the list of sprites
                Vector2 currSpritePos = (Vector2)randSpritesPos[i];

                _spriteBatch.Draw(charaset, new Vector2(currSpritePos.X, currSpritePos.Y + yFactor), currSprite, Color.White);
            }
        }


        void spawnRandomSprite()
        {
            Vector2 rndPos;
            Random rnd1 = new Random();
            Random rnd2 = new Random();
            int r = rnd1.Next(0, 3);
            int c = rnd2.Next(0, 3);
            randSprites.Add(spritesGrid[r, c]);   // Add a random sprite from the spritesheet to the list

            Random rnd = new Random();
            rand_x = rnd.Next(0, rightBoundary);
            rand_y = 0;

            rndPos.X = rand_x;
            rndPos.Y = rand_y;
            randSpritesPos.Add(rndPos);  // Store a position for the random sprite
        }


        public Boolean alienCollision()
        {
            Boolean collided = false;

            for (int i = 0; i < randSprites.Count; i++)
            {
                Vector2 thisSprite = (Vector2)randSpritesPos[i];

                if ((firePos.X + sprite_w) >= thisSprite.X &&    // fireball's right edge is past alien's left
                        firePos.X <= thisSprite.X + sprite_w &&     // fireball's left edge is past alien's right
                        firePos.Y + thisSprite.Y >= thisSprite.Y &&    // fireball's top edge is past alien's bottom
                        firePos.Y <= thisSprite.Y + sprite_h)   // fireball's bottom edge is past alien's top

                {
                    collided = true;
                    randSprites.RemoveAt(i);     //remove the sprite
                    randSpritesPos.RemoveAt(i);
                    score++;
                }
            }

            return collided;
        }

        public void showScores()
        {
            _spriteBatch.DrawString(font, "Score:" + score, new Vector2(rightBoundary - 80, 10), Color.Black);
        }

        public void displayGameOverMsg()
        {
            _spriteBatch.DrawString(font, "Game over!", new Vector2(rightBoundary / 3, bottomBoundary / 3), Color.Black);
        }



    }
}