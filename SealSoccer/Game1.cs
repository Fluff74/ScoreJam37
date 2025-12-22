using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

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
            GameOver
        }
        GameState gameState;

        // Used to store and update all of the snowflakes.
        readonly List<Snow> snowManager = [];
        readonly Random rng = new();

        #endregion

        #region Objects

        Seal seal; // Player character.
        SoccerBall soccerBall; // The ball that the player tries to hit.

        #endregion

        #region Assets

        Texture2D snowflake;
        Texture2D sealSpritesheet;
        Texture2D soccerBallSprite;
        Texture2D ground;
        Texture2D backdrop;
        Texture2D logo;
        Texture2D gameOverMenu;
        SpriteFont mediumJersey10;

        #endregion

        #region Variables

        int score; // How many consecutive balls the seal has bumped.
        int highscore; // The highest score earned by the player.
        float wind; // The current speed at which the wind is blowing.
        int windParameter; // The amount of points needed for the wind to change direction.
        int xMod; // The value by which the ball's X velocity can be modified.
        int xModParameter; // The amount of points needed for xMod to increase.
        int gravParamOne; // The amount of points needed to increase gravity once.
        int gravParamTwo; // The amount of points needed to increase gravity twice.
        bool newHighscore; // Whether or not the player just beat their highscore.

        #endregion

        #region Draw Locations

        Rectangle groundDrawbox; // Where we draw the ground.
        Rectangle backdropDrawbox; // Where we draw the backdrop.
        Rectangle logoDrawbox; // Where we draw the logo.
        Rectangle gameOverMenuDrawbox; // Where we draw the game over menu.
        Vector2 highscoreWriteVector; // Where we write the highscore.
        Vector2 gameScoreWriteVector; // Where we write the current score during the game.
        Vector2 gameOverTextWriteVector; // Where we write game over.
        Vector2 gameOverScoreWriteVector; // Where we display the player's end score.
        Vector2 gameOverHighscoreWriteVector; // Where we display the player's current highscore.
        Vector2 gameOverNewScoreWriteVector; // Where we display whether or not they beat their highscore.

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
            gameState = GameState.MainMenu; // Make sure we start off on the main menu.

            groundDrawbox = new(0, 2070, 3840, 90); // The place we're drawing the ground.
            backdropDrawbox = new(0, 0, 3840, 2070); // The place we're drawing the backdrop.
            logoDrawbox = new(50, 50, 1200, 1200); // The logo is drawn here.
            gameOverMenuDrawbox = new(1320, 480, 1200, 1200); // The game over menu is drawn here.
            highscoreWriteVector = new(400, 1800); // The place we're writing the highscore.
            gameScoreWriteVector = new(80, 30); // The place we're writing the player's score during the game.
            gameOverTextWriteVector = new(1550, 510); // Where we write "Game Over!" on the menu.
            gameOverScoreWriteVector = new(1510, 740); // Where we write the player's current score on the menu.
            gameOverHighscoreWriteVector = new(1510, 900); // Where we write the player's current highscore on the menu.
            gameOverNewScoreWriteVector = new(1420, 1120); // Where we write "New Highscore!" on the game over menu.

            #region Variables

            wind = 0; // There is no wind at the start.
            windParameter = 5; // Sets the wind parameter to an initial value of five.
            xMod = 0; // There is no xMod at the start.
            xModParameter = 10; // Sets the xModParameter to an initial value of ten.
            gravParamOne = 25; // Sets the first gravity increment to happen at 25 points.
            gravParamTwo = 50; // Sets the second gravity increment to happen at 50 points.
            newHighscore = false; // Sets whether or not they've already beaten their score to false.

            #endregion

            LoadHighscore(); // Load in the player's highscore.

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
            backdrop = Content.Load<Texture2D>($"Backdrop");
            logo = Content.Load<Texture2D>($"Snowflake");
            gameOverMenu = Content.Load<Texture2D>($"GameOverMenu");
            mediumJersey10 = Content.Load<SpriteFont>($"Jersey10");

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

            // Snowflakes are updated every frame, in every gamestate.
            foreach (Snow snowflake in snowManager)
            {
                snowflake.Update((int)wind);
            }

            switch (gameState)
            {
                case GameState.MainMenu:

                    if(SingleKeyPress(Keys.Enter))
                    {
                        gameState = GameState.Game;
                    }

                    break;

                // Everything that must be updated during gameplay.
                case GameState.Game:

                    // Update the soccerball every frame.
                    soccerBall.Update(wind / 50);

                    // Handle seal's collision with the ball.
                    switch(seal.CheckCollision(soccerBall.Hitbox))
                    {
                        // Launches the ball.
                        case Seal.BumpType.Launch:

                            soccerBall.Launch(xMod);
                            score++; // Add a point to score.
                            windParameter--; // Decrement wind parameter.
                            xModParameter--; // Decrement xMod parameter;
                            gravParamOne--; // Decrement gravity parameter one.
                            gravParamTwo--; // Decrement gravity parameter two.

                            break;

                        // Fixes the ball.
                        case Seal.BumpType.Side:

                            seal.HandleOverlap(soccerBall);

                            break;
                    }

                    // Allows the player to move left.
                    if((kb.IsKeyDown(Keys.A) && kb.IsKeyUp(Keys.D)) || (kb.IsKeyDown(Keys.Left) && kb.IsKeyUp(Keys.Right)))
                    {
                        seal.Move(false);
                        seal.UpdateAnimation(gameTime);
                    }

                    // Allows the player to move right.
                    else if((kb.IsKeyDown(Keys.D) && kb.IsKeyUp(Keys.A)) || (kb.IsKeyDown(Keys.Right) && kb.IsKeyUp(Keys.Left)))
                    {
                        seal.Move(true);
                        seal.UpdateAnimation(gameTime);
                    }

                    // Resets the seal's animation if he's not moving.
                    else
                    {
                        seal.ResetAnimation();
                    }

                    // Changes the wind every five points.
                    if(windParameter == 0)
                    {
                        // Keeps the wind between -10 and 10.
                        wind = rng.Next(10, 31) - 20;
                        windParameter = 5;
                    }

                    // Increments xMod every ten points.
                    if(xModParameter == 0)
                    {
                        xMod++;
                        xModParameter = 10;
                    }

                    // Increments gravity as needed.
                    if(gravParamOne == 0)
                    {
                        soccerBall.Gravity = 0.4f;
                        gravParamOne = -1;
                    }
                    if(gravParamTwo == 0)
                    {
                        soccerBall.Gravity = 0.5f;
                        gravParamTwo = -1;
                    }

                    // Handles when the game ends.
                    if(CheckGroundCollision(soccerBall.Hitbox))
                    {
                        // Checks to see if they've beaten their highscore.
                        if(score > highscore)
                        {
                            newHighscore = true;
                        }

                        gameState = GameState.GameOver;
                    }

                    break;

                case GameState.GameOver:

                    if(SingleKeyPress(Keys.Enter))
                    {

                        // If they've beaten their highscore, save it before resetting their score.
                        if(newHighscore)
                        {
                            SaveHighscore(score);
                            highscore = score;
                            newHighscore = false;
                        }

                        ResetGame(); // Reset for the next game.

                        gameState = GameState.MainMenu;
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
            _spriteBatch.Draw(backdrop, backdropDrawbox, Color.White);
            seal.Draw(_spriteBatch);
            soccerBall.Draw(_spriteBatch);

            switch (gameState)
            {
                case GameState.MainMenu:

                    _spriteBatch.DrawString(mediumJersey10, $"HIGHSCORE: {highscore}", highscoreWriteVector, Color.Gold);

                    // Draw each of the snowflakes underneath the logo.
                    foreach (Snow snowflake in snowManager)
                    {
                        snowflake.Draw(_spriteBatch);
                    }

                    // Draw our logo to the main menu screen.
                    _spriteBatch.Draw(logo, logoDrawbox, Color.Red);

                    break;

                case GameState.Game:

                    _spriteBatch.DrawString(mediumJersey10, $"Score: {score}", gameScoreWriteVector, Color.LightBlue);

                    // Draw each of the snowflakes.
                    foreach (Snow snowflake in snowManager)
                    {
                        snowflake.Draw(_spriteBatch);
                    }

                    break;

                case GameState.GameOver:

                    // Draw each of the snowflakes underneath the menu.
                    foreach (Snow snowflake in snowManager)
                    {
                        snowflake.Draw(_spriteBatch);
                    }

                    // Draws the game over menu.
                    _spriteBatch.Draw(gameOverMenu, gameOverMenuDrawbox, Color.White);

                    // Writes all text on the game over menu.
                    _spriteBatch.DrawString(mediumJersey10, $"Game Over!", gameOverTextWriteVector, Color.PaleVioletRed);
                    _spriteBatch.DrawString(mediumJersey10, $"Score: {score}", gameOverScoreWriteVector, Color.AntiqueWhite);
                    _spriteBatch.DrawString(mediumJersey10, $"Highscore: {highscore}", gameOverHighscoreWriteVector, Color.AntiqueWhite);
                    if(newHighscore)
                    {
                        _spriteBatch.DrawString(mediumJersey10, $"New Highscore!", gameOverNewScoreWriteVector, Color.LightGreen);
                    }

                    break;
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
        /// Loads the player's highscore into the game.
        /// </summary>
        public void LoadHighscore()
        {
            // Check if file exists.
            if(File.Exists($"Highscore"))
            {
                // Create access to the file, and means of reading it.
                FileStream file = File.OpenRead($"Highscore");
                BinaryReader reader = new(file);

                // Attempt to read the highscore.
                try
                {
                    highscore = reader.ReadInt32();
                }

                // If the data is corrupted, change their highscore to zero.
                catch
                {
                    highscore = 0;
                }

                // Close the reader when we're done with it.
                finally
                {
                    reader.Close();
                }
            }

            // It doesn't exist, so create it and load their highscore as zero.
            else
            {
                File.Create($"Highscore");
                highscore = 0;
                SaveHighscore(0);
            }
        }

        /// <summary>
        /// Saves the highscore to the highscore file.
        /// </summary>
        /// <param name="newScore"> The new highscore of the player. </param>
        public static void SaveHighscore(int newScore)
        {
            // Create access to the file, and means of writing to it.
            FileStream file = File.OpenWrite($"Highscore");
            BinaryWriter writer = new(file);

            // Write their highscore.
            writer.Write(newScore);

            // Close the writer.
            writer.Close();
        }

        /// <summary>
        /// Resets the game.
        /// </summary>
        public void ResetGame()
        {
            seal.Reset();
            soccerBall.Reset();
            score = 0;
            wind = 0;
            windParameter = 5;
            xMod = 0;
            xModParameter = 10;
            gravParamOne = 25;
            gravParamTwo = 50;
        }
    }
}
