using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System;

namespace SealSoccer
{
    // Joshua Smith
    // 12/19/2025
    //
    // This is our game for the 37th ScoreJam. We want to make a game where you play as a seal that is practicing heading a soccer
    // ball, because it does not have feet :)
    public class Game1 : Game
    {
        private readonly GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        #region Utilities

        // Resolution
        float xScale; // Used to support different screen resolutions, up to 4K UHD.
        float yScale; // Used to support different screen resolutions, up to 4K UHD.
        Matrix windowScaler; // Used to scale the entire game based on the above scales.

        // Input
        KeyboardState kb; // The current state of the keyboard.
        KeyboardState pkb; // The previous state of the keyboard.

        /// <summary>
        /// The current state the game is in.
        /// </summary>
        enum GameState
        {
            MainMenu,
            Game,
            Leaderboard
        }
        GameState gameState;

        // Used to store and update all of the snowflakes.
        readonly List<Snow> snowManager = [];
        readonly Random rng = new();

        #endregion

        #region Objects

        Seal seal;
        SoccerBall soccerBall;

        #endregion

        #region Assets

        Texture2D snowflake;
        Texture2D sealSpritesheet;
        Texture2D soccerBallSprite;
        Texture2D ground;

        #endregion

        #region Variables

        int score; // How many consecutive balls the seal has bumped.
        float wind; // The current speed at which the wind is blowing.
        int windParameter; // The amount of points needed for the wind to change direction.

        #endregion

        #region Draw Locations

        Rectangle groundDrawbox;

        #endregion

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            #region Screen Resolution Settings

            _graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            _graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            _graphics.ApplyChanges();

            Window.IsBorderless = true;

            #endregion
        }

        protected override void Initialize()
        {
            // Set the scales to render at 4K, and then create the scale matrix.
            xScale = (float)_graphics.PreferredBackBufferWidth / 3840;
            yScale = (float)_graphics.PreferredBackBufferHeight / 2160;
            windowScaler = Matrix.CreateScale(xScale, yScale, 1.0f);

            groundDrawbox = new(0, 2070, 3840, 90);

            gameState = GameState.Game;

            wind = 0; // There is no wind at the start.
            windParameter = 5; // Sets the wind parameter to an initial value of five.

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            #region Assets

            snowflake = Content.Load<Texture2D>($"Snowflake");
            sealSpritesheet = Content.Load<Texture2D>($"Seal");
            soccerBallSprite = Content.Load<Texture2D>($"Soccerball");
            ground = Content.Load<Texture2D>($"Ground");

            #endregion

            #region Objects

            seal = new(sealSpritesheet);
            soccerBall = new(soccerBallSprite);

            #endregion

            #region Utilities

            // Populate the snow manager with 150 snowflakes.
            for (int i = 0; i < 1500; i++)
            {
                snowManager.Add(new(snowflake));
            }

            #endregion
        }

        protected override void Update(GameTime gameTime)
        {
            if(GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape)) { Exit(); }
            kb = Keyboard.GetState();

            switch(gameState)
            {
                case GameState.Game:

                    // Update the soccerball every frame.
                    soccerBall.Update(wind / 50);

                    // Handle seal's collision with the ball.
                    switch(seal.CheckCollision(soccerBall.Hitbox))
                    {
                        // Launches the ball.
                        case Seal.BumpType.Launch:

                            soccerBall.Launch();
                            score++; // Add a point to score.
                            windParameter--; // Decrement wind parameter.

                            break;

                        // Fixes the ball.
                        case Seal.BumpType.Side:

                            seal.HandleOverlap(soccerBall);

                            break;
                    }

                    // Allows the player to move left.
                    if(kb.IsKeyDown(Keys.A) && kb.IsKeyUp(Keys.D))
                    {
                        seal.Move(false);
                        seal.UpdateAnimation(gameTime);
                    }

                    // Allows the player to move right.
                    else if(kb.IsKeyDown(Keys.D) && kb.IsKeyUp(Keys.A))
                    {
                        seal.Move(true);
                        seal.UpdateAnimation(gameTime);
                    }

                    // Resets the seal's animation if he's not moving.
                    else
                    {
                        seal.ResetAnimation();
                    }

                    // Update each of the snowflakes.
                    foreach (Snow snowflake in snowManager)
                        {
                            snowflake.Update((int)wind);
                        }

                    // Changes the wind every five points.
                    if(windParameter == 0)
                    {
                        // Keeps the wind between -10 and 10.
                        wind = rng.Next(10, 31) - 20;
                        windParameter = 5;
                    }

                    if(CheckGroundCollision(soccerBall.Hitbox))
                    {
                        ResetGame();
                    }

                    break;
            }

            pkb = kb; // Set previous keyboard state to the keyboard state at the end of the frame.
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin(transformMatrix: windowScaler, samplerState: SamplerState.PointClamp);

            _spriteBatch.Draw(ground, groundDrawbox, Color.White);
            seal.Draw(_spriteBatch);
            soccerBall.Draw(_spriteBatch);

            // Draw each of the snowflakes.
            foreach (Snow snowflake in snowManager)
            {
                snowflake.Draw(_spriteBatch);
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        /// <summary>
        /// Checks whether or not the specified key was pressed once.
        /// </summary>
        /// <param name="key"> The key we're checking. </param>
        /// <returns> Whether or not the key has been pressed once. </returns>
        public bool SingleKeyPress(Keys key)
        {
            return kb.IsKeyDown(key) && pkb.IsKeyUp(key);
        }

        /// <summary>
        /// Checks to see if the soccerball hit the ground.
        /// </summary>
        /// <param name="soccerball"> The hitbox of the soccerball. </param>
        /// <returns> Whether or not the soccerball hit the ground. </returns>
        public bool CheckGroundCollision(Rectangle soccerball)
        {
            return groundDrawbox.Intersects(soccerball);
        }

        /// <summary>
        /// Resets the game.
        /// </summary>
        public void ResetGame()
        {
            seal.Reset();
            soccerBall.Reset();
            wind = 0;
            windParameter = 5;
        }
    }
}
