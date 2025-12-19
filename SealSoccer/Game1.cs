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

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
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
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin(transformMatrix: windowScaler);



            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
