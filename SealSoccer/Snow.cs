using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SealSoccer
{
    // Joshua Smith
    // 12/20/2025
    //
    // The snow class is literally just a snowflake that falls down the screen. It's used because it looks good, but it also
    // conveys wind direction.
    internal class Snow
    {
        readonly Random rng = new(); // Just a standard C# random number generator.

        readonly Texture2D snowflake; // The texture of the snowflake (duh).
        int fallspeed; // The speed at which the snowflake falls.
        Rectangle drawbox; // The rectangle where we're drawing the snowflake.
        Color color; // The color of the snowflake.

        /// <summary>
        /// The only constructor of a snowflake.
        /// </summary>
        /// <param name="snowflake"></param>
        public Snow(Texture2D snowflake)
        {
            this.snowflake = snowflake;
            drawbox = new Rectangle(0, 0, 10, 10);

            fallspeed = rng.Next(1, 6);
            int temp = rng.Next(180, 251);
            color = new(temp, temp, temp);
            drawbox.X = rng.Next(1, 3830);
            drawbox.Y = rng.Next(1, 2150);
        }

        /// <summary>
        /// Updates the snowflake, which means just moving it, since that really is all they do in this game.
        /// </summary>
        /// <param name="wind"> The current speed and direction of the wind. </param>
        public void Update(int wind)
        {
            drawbox.Y += fallspeed;
            drawbox.X += wind;

            // If the snow falls off the bottom of the screen, reset it.
            if(drawbox.Y > 2070)
            {
                Reset();
            }

            // If the snowflake goes off the left side of the screen, move it to the right side.
            if(drawbox.X < -10)
            {
                drawbox.X = 3840;
            }

            // If the snowflake goes off the right side of the screen, move it to the left side.
            if(drawbox.X > 3840)
            {
                drawbox.X = -10;
            }
        }

        /// <summary>
        /// Draws the snowflake to the screen.
        /// </summary>
        /// <param name="sb"> The SpriteBatch we're using to draw the snowflake. </param>
        public void Draw(SpriteBatch sb)
        {
            sb.Draw(snowflake, drawbox, color);
        }

        /// <summary>
        /// Re-randomizes the fallspeed, color, and position of the snowflake.
        /// </summary>
        public void Reset()
        {
            fallspeed = rng.Next(1, 6);
            int temp = rng.Next(180, 251);
            color = new(temp, temp, temp);
            drawbox.X = rng.Next(1, 3830);
            drawbox.Y = rng.Next(1, 401) - 400;
        }
    }
}
