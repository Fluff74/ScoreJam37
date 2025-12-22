using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SealSoccer
{
    // Joshua Smith
    // 12/19/2025
    //
    // This is the soccerball that the seal will be trying to keep in the air. It won't be animated, I will just rotate it
    // with code.
    /// <summary>
    /// This is the only constructor for a soccerball. It only needs a texture, everything else is handled internally.
    /// </summary>
    /// <param name="sprite"> The sprite of the soccerball. </param>
    internal class SoccerBall(Texture2D sprite)
    {
        /// <summary>
        /// A property to access the hitbox of the soccer ball.
        /// </summary>
        public Rectangle Hitbox { get { return hitbox; } }

        /// <summary>
        /// It's gravity, it moves the ball downwards.
        /// </summary>
        public float Gravity { get; set; } = 0.3f;

        readonly Texture2D sprite = sprite; // This is the sprite of the soccerball.
        Rectangle hitbox = new(1830, 600, 180, 180); // This is both the hitbox of the soccerball, but also where it'll be drawn from.
        Rectangle drawbox = new(1900, 660, 180, 180); // The drawbox of the soccerball.
        Rectangle source = new(0, 0, 60, 60); // This is the source rectangle from which the soccerball is drawn.
        Vector2 velocity = new(0.0f, -10.0f); // This is the current velocity of the ball.
        float rotation = 0; // The rotation of the ball.
        readonly Random rng = new(); // This is a standard C# Random Number Generator.

        /// <summary>
        /// Updates the position of the soccerball based off of the wind and its velocity.
        /// </summary>
        /// <param name="wind"> The speed of the wind right now. </param>
        public void Update(float wind)
        {
            // Move the ball.
            hitbox.X += (int)velocity.X;
            hitbox.Y += (int)velocity.Y;

            // Adjust for wind.
            velocity.X += wind;

            // Updates the rotation so the ball looks animated and cool.
            rotation += velocity.X / 100;

            // Make the ball bounce off the sides of the screen.
            if(hitbox.X < 0)
            {
                hitbox.X = 0;
                velocity.X *= -1;
            }
            if(hitbox.X > 3660)
            {
                hitbox.X = 3660;
                velocity.X *= -1;
            }

            // Account for gravity.
            velocity.Y += Gravity;

            // Update the drawbox accordingly.
            drawbox.X = hitbox.X + 90;
            drawbox.Y = hitbox.Y + 90;
        }

        /// <summary>
        /// Launches the soccerball.
        /// </summary>
        /// <param name="xMod"> The amount that the X velocity of the ball can be modified. </param>
        public void Launch(int xMod)
        {
            hitbox.Y = 1679; // Puts the ball immediately above the seal.

            // Maximum Y-Velocity can be -31.0f, minimum is -17.0f
            velocity.Y = (float)(rng.Next(170, 311) / 10) * -1;

            // Maximum X-Velocity can be -15.0f, minimum is 15.0f
            velocity.X = (float)(rng.Next(150, 451) / 10);
            velocity.X -= 30;

            // When the score is higher, then start giving the ball some CRAZY velocity.
            if(velocity.X < 0)
            {
                velocity.X -= rng.Next(xMod, xMod * 3 + 1) / 10;
                velocity.X -= xMod * 2;
            }
        }

        /// <summary>
        /// Translates the hitbox of the soccerball.
        /// </summary>
        /// <param name="value"> The value by which we're translating the soccerball's hitbox. </param>
        public void Translate(int value)
        {
            hitbox.X += value;
        }

        /// <summary>
        /// Draws the soccerball, even accounting for it's rotation!!!
        /// </summary>
        /// <param name="sb"> The SpriteBatch with which we're drawing the ball. </param>
        public void Draw(SpriteBatch sb)
        {
            sb.Draw(sprite, drawbox, source, Color.White, rotation, source.Center.ToVector2(), SpriteEffects.None, 0.0f);
        }

        /// <summary>
        /// Resets all of the ball-related variables.
        /// </summary>
        public void Reset()
        {
            hitbox.X = 1830;
            hitbox.Y = 600;
            velocity.X = 0.0f;
            velocity.Y = 0.0f;
            Gravity = 0.3f;
        }
    }
}
