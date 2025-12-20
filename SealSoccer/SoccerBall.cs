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
    internal class SoccerBall
    {
        /// <summary>
        /// A property to access the hitbox of the soccer ball.
        /// </summary>
        public Rectangle Hitbox { get { return hitbox; } }

        Texture2D sprite; // This is the sprite of the soccerball.
        Rectangle hitbox; // This is both the hitbox of the soccerball, but also where it'll be drawn from.
        Vector2 velocity; // This is the current velocity of the ball.

        readonly float gravity = 0.3f; // It's gravity, it moves the ball downwards.
        readonly Random rng = new(); // This is a standard C# Random Number Generator.

        public SoccerBall(Texture2D sprite)
        {
            this.sprite = sprite;
            hitbox = new(1830, 600, 180, 180);
            velocity = new(0.0f, -10.0f);
        }

        public void Update()
        {
            hitbox.X += (int)velocity.X;
            hitbox.Y += (int)velocity.Y;

            velocity.Y += gravity;
        }

        public void Launch()
        {
            // Maximum Y-Velocity can be -31.0f, minimum is -17.0f
            velocity.Y = (float)(rng.Next(170, 311) / 10) * -1;
        }

        public void Draw(SpriteBatch sb)
        {
            sb.Draw(sprite, hitbox, Color.White);
        }

        public void Reset()
        {
            hitbox.X = 1830;
            hitbox.Y = 600;
            velocity.X = 0.0f;
            velocity.Y = 0.0f;
        }
    }
}
