using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;

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
        MouseState ms; // The current state of the mouse.
        MouseState pms; // The previous state of the mouse.
        Point adjustedMousePosition; // The adjusted location of the mouse, based on the resolution of the screen.

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

        // Buttons
        Button playButton; // The button players need to click on to play the game.
        Button quitButton; // The button players need to click on to close the game.
        Button menuButton; // The button players need to click on to return to the main menu.

        #endregion

        #region Assets

        Texture2D snowflake; // The texture used for the snowflakes, without a color tint.
        Texture2D sealSpritesheet; // The spritesheet for the seal's galumphing animation.
        Texture2D soccerBallSprite; // The sprite used for the soccerball, which is rotated in code.
        Texture2D ground; // The texture used for the ground.
        Texture2D backdrop; // The texture used for the backdrop.
        Texture2D logo; // The game's official logo.
        Texture2D gameOverMenu; // The texture used for the background of the text in the game over menu.
        SpriteFont mediumJersey10; // The font used for all text in the game.
        Song backgroundMusic; // The free background music from Pixabay, made by VibeHorn.
        SoundEffect bump; // The bump sound effect that plays when the seal hits the ball.
        SoundEffect windBlow; // The wind sound effect that plays when the wind direction changes.

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

        /// <summary>
        /// The constructor of the game.
        /// </summary>
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

        /// <summary>
        /// Initializes everything that doesn't need an asset of some kind.
        /// </summary>
        protected override void Initialize()
        {
            #region Utilities

            // Set the scales to render at 4K, and then create the scale matrix.
            xScale = (float)_graphics.PreferredBackBufferWidth / 3840;
            yScale = (float)_graphics.PreferredBackBufferHeight / 2160;
            windowScaler = Matrix.CreateScale(xScale, yScale, 1.0f);
            gameState = GameState.MainMenu; // Make sure we start off on the main menu.

            #endregion

            #region Draw Locations

            groundDrawbox = new(0, 2070, 3840, 90); // The place we're drawing the ground.
            backdropDrawbox = new(0, 0, 3840, 2070); // The place we're drawing the backdrop.
            logoDrawbox = new(50, 50, 1200, 1200); // The logo is drawn here.
            gameOverMenuDrawbox = new(1320, 480, 1200, 1200); // The game over menu is drawn here.
            highscoreWriteVector = new(400, 1800); // The place we're writing the highscore.
            gameScoreWriteVector = new(80, 30); // The place we're writing the player's score during the game.
            gameOverTextWriteVector = new(1550, 510); // Where we write "Game Over!" on the menu.
            gameOverScoreWriteVector = new(1510, 740); // Where we write the player's current score on the menu.
            gameOverHighscoreWriteVector = new(1510, 900); // Where we write the player's current highscore on the menu.
            gameOverNewScoreWriteVector = new(1420, 1150); // Where we write "New Highscore!" on the game over menu.

            #endregion

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

        /// <summary>
        /// Loads all of the assets into the game, and initializes objects that utilize assets.
        /// </summary>
        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            #region Assets

            // Load all of the assets.
            snowflake = Content.Load<Texture2D>($"Snowflake");
            sealSpritesheet = Content.Load<Texture2D>($"Seal");
            soccerBallSprite = Content.Load<Texture2D>($"Soccerball");
            ground = Content.Load<Texture2D>($"Ground");
            backdrop = Content.Load<Texture2D>($"Backdrop");
            logo = Content.Load<Texture2D>($"Logo");
            gameOverMenu = Content.Load<Texture2D>($"GameOverMenu");
            mediumJersey10 = Content.Load<SpriteFont>($"Jersey10");
            backgroundMusic = Content.Load<Song>($"BackgroundMusic");
            bump = Content.Load<SoundEffect>($"Bump");
            windBlow = Content.Load<SoundEffect>($"Wind");

            #endregion

            #region Objects

            seal = new(sealSpritesheet);
            soccerBall = new(soccerBallSprite);

            // Buttons
            playButton = new($"Play", mediumJersey10, new(2600, 1700), new(2580, 1735, 295, 155));
            quitButton = new($"Quit", mediumJersey10, new(3200, 1700), new(3180, 1735, 490, 155));
            menuButton = new($"Menu", mediumJersey10, new(1750, 1420), new(1730, 1455, 380, 140));

            #endregion

            #region Utilities

            // Populate the snow manager with 150 snowflakes.
            for (int i = 0; i < 1500; i++)
            {
                snowManager.Add(new(snowflake));
            }

            #endregion

            // Play the background music.
            MediaPlayer.Volume = 0.03f;
            MediaPlayer.Play(backgroundMusic);
            MediaPlayer.IsRepeating = true;
        }

        /// <summary>
        /// Updates everything in the game.
        /// </summary>
        /// <param name="gameTime"> The elapsed time in the game. </param>
        protected override void Update(GameTime gameTime)
        {
            // Get the current input states.
            kb = Keyboard.GetState();
            ms = Mouse.GetState();

            // Calculate the adjusted mouse position based on the maximum supported resolution, and the current resolution.
            adjustedMousePosition = new((int)(ms.X * (3840.0 / _graphics.PreferredBackBufferWidth)), (int)(ms.Y * (2160.0 / _graphics.PreferredBackBufferHeight)));

            // Snowflakes are updated every frame, in every gamestate.
            foreach (Snow snowflake in snowManager)
            {
                snowflake.Update((int)wind);
            }

            // Handle everything specific to the different game states.
            switch (gameState)
            {
                // Everything that must be updated on the main menu.
                case GameState.MainMenu:

                    // If they click the play button, or press the 'Enter' key, start the game!
                    if(playButton.Update(ms, pms, adjustedMousePosition) || SingleKeyPress(Keys.Enter))
                    {
                        IsMouseVisible = false;
                        gameState = GameState.Game;
                    }

                    // If they click the quit button, or press the 'Escape' key, close the app.
                    if(quitButton.Update(ms, pms, adjustedMousePosition) || SingleKeyPress(Keys.Escape))
                    {
                        Exit();
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
                            bump.Play();
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
                        windBlow.Play();
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

                        IsMouseVisible = true;
                        gameState = GameState.GameOver;
                    }

                    break;

                // Everything that must be updated on the game over menu.
                case GameState.GameOver:

                    // If they click the menu button or press the 'Enter' key, go back to the main menu.
                    if(menuButton.Update(ms, pms, adjustedMousePosition) || SingleKeyPress(Keys.Enter))
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

            // Set the previous input states to the current input states at the end of the frame.
            pkb = kb;
            pms = ms;
            base.Update(gameTime);
        }

        /// <summary>
        /// Draws everything in the game to the screen.
        /// </summary>
        /// <param name="gameTime"> The elapsed time in the game. </param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin(transformMatrix: windowScaler, samplerState: SamplerState.PointClamp);

            // The seal, soccerball, ground, and backdrop need to be drawn every frame, regardless of the game state.
            _spriteBatch.Draw(ground, groundDrawbox, Color.White);
            _spriteBatch.Draw(backdrop, backdropDrawbox, Color.White);
            seal.Draw(_spriteBatch);
            soccerBall.Draw(_spriteBatch);

            // Handle everything specific to the different game states.
            switch (gameState)
            {
                // Everything that needs to be drawn on the main menu.
                case GameState.MainMenu:

                    // Display the player's highscore on the main menu.
                    _spriteBatch.DrawString(mediumJersey10, $"HIGHSCORE: {highscore}", highscoreWriteVector, Color.Gold);

                    // Draw the buttons.
                    playButton.Draw(_spriteBatch);
                    quitButton.Draw(_spriteBatch);

                    // Draw each of the snowflakes underneath the logo.
                    foreach (Snow snowflake in snowManager)
                    {
                        snowflake.Draw(_spriteBatch);
                    }

                    // Draw our logo to the main menu screen.
                    _spriteBatch.Draw(logo, logoDrawbox, Color.White);

                    break;

                // Everything that needs to be drawn during the game.
                case GameState.Game:

                    _spriteBatch.DrawString(mediumJersey10, $"Score: {score}", gameScoreWriteVector, Color.LightBlue);

                    // Draw each of the snowflakes.
                    foreach (Snow snowflake in snowManager)
                    {
                        snowflake.Draw(_spriteBatch);
                    }

                    break;

                // Everything that needs to be drawn on the game over menu.
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
                    menuButton.Draw(_spriteBatch);

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
