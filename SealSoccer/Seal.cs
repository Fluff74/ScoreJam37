using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SealSoccer
{
    // Joshua Smith
    // 12/19/2025
    //
    // This is the seal that the player will control in the game! It walks left and right. It'll have an
    // animation for galumphing.
    internal class Seal
    {
        /// <summary>
        /// This is the direction that the seal is facing in.
        /// </summary>
        enum Facing
        {
            Left,
            Right
        }
        Facing facing;

        /// <summary>
        /// The speed at which the seal moves.
        /// </summary>
        public int Speed { get; set; }

        Texture2D spritesheet; // Used while the seal is moving.
        readonly Vector2 origin; // The origin we're drawing the seal from.
        Rectangle drawbox; // This is where the seal is drawn from.
        Rectangle hitbox; // This is the seal's hitbox, where it can hit the ball from.
        Rectangle source; // The source rectangle on the spritesheet that we're drawing the seal from.

        public Seal(Texture2D spritesheet)
        {
            facing = Facing.Right; // Start the seal facing to the right.
            Speed = 20; // Always between 20 and 30.

            this.spritesheet = spritesheet;
            origin = new(0.0f, 0.0f);
            drawbox = new(1645, 1740, 555, 330);
            hitbox = new(1645, 1780, 555, 290);
            source = new(0, 0, 183, 107);
        }

        public void Update(GameTime gameTime)
        {

        }

        /// <summary>
        /// Moves the seal, and adjusts the direction it is facing accordingly.
        /// </summary>
        /// <param name="rightward"> Whether or not the seal is moving to the right. </param>
        public void Move(bool rightward)
        {
            // Move the seal to the right.
            if(rightward)
            {
                drawbox.X += Speed;

                // If the seal leaves the right bound of the screen, stop it from moving further.
                if(drawbox.X > 3290)
                {
                    drawbox.X = 3290;
                }

                // Make the seal face right, if it is currently facing left.
                if(facing == Facing.Left)
                {
                    facing = Facing.Right;
                }
            }

            // Move the seal to the left.
            else
            {
                drawbox.X -= Speed;

                // If the seal leaves the left bound of the screen, stop it from moving.
                if(drawbox.X < 0)
                {
                    drawbox.X = 0;
                }

                // Make the seal face left, if it is currently facing right.
                if(facing == Facing.Right)
                {
                    facing = Facing.Left;
                }
            }

            // Update the hitbox's coordinates to match the drawbox's coordinates.
            hitbox.X = drawbox.X;
        }

        /// <summary>
        /// Draws the seal onto the screen, facing the proper direction.
        /// </summary>
        /// <param name="sb"> The spritebatch we're using to draw the seal. </param>
        public void Draw(SpriteBatch sb)
        {
            switch (facing)
            {
                case Facing.Left:

                    sb.Draw(spritesheet, drawbox, source, Color.White, 0.0f, origin, SpriteEffects.FlipHorizontally, 0.0f);

                    break;

                case Facing.Right:

                    sb.Draw(spritesheet, drawbox, source, Color.White, 0.0f, origin, SpriteEffects.None, 0.0f);

                    break;
            }
        }

        /// <summary>
        /// Checks to see whether or not the soccerball is colliding with the seal.
        /// </summary>
        /// <param name="soccerball"> The hitbox of the soccerball. </param>
        /// <returns> Whether or not they're colliding. </returns>
        public bool CheckCollision(Rectangle soccerball)
        {
            return hitbox.Intersects(soccerball);
        }

        public void Reset()
        {
            drawbox.X = 1645;
            drawbox.Y = 1750;
            facing = Facing.Right;
            Speed = 20;
        }
    }
}
