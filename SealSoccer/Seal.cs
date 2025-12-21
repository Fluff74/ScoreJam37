using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SealSoccer
{
    // Joshua Smith
    // 12/19/2025
    //
    // This is the seal that the player will control in the game! It walks left and right. It'll have an
    // animation for galumphing.
    /// <summary>
    /// This is the seal object's constructor. All it takes in is one spritesheet.
    /// </summary>
    /// <param name="spritesheet"> The spritesheet containing the seal's galumphing animation sprites. </param>
    internal class Seal(Texture2D spritesheet)
    {
        /// <summary>
        /// This is the direction that the seal is facing in.
        /// </summary>
        enum Facing
        {
            Left,
            Right
        }
        Facing facing = Facing.Right;

        /// <summary>
        /// This is the type of collision that the seal has with the ball.
        /// </summary>
        public enum BumpType
        {
            Launch,
            Side
        }

        /// <summary>
        /// The speed at which the seal moves.
        /// </summary>
        public int Speed { get; set; } = 20; // Always between 20 and 30.

        readonly Texture2D spritesheet = spritesheet; // Used while the seal is moving.
        readonly Vector2 origin = new(0.0f, 0.0f); // The origin we're drawing the seal from.
        Rectangle drawbox = new(1645, 1780, 555, 330); // This is where the seal is drawn from.
        Rectangle hitbox = new(1645, 1860, 555, 290); // This is the seal's hitbox, where it can hit the ball from.
        Rectangle source = new(0, 0, 185, 110); // The source rectangle on the spritesheet that we're drawing the seal from.
        int frame = 0; // The current frame of the animation we're on.
        float timeElapsed = 0.0f; // The timer for when to increment frames.
        readonly float secondsPerFrame = 0.06f; // How many seconds go by in one frame.

        /// <summary>
        /// Updates the frame data of the seal's animation.
        /// </summary>
        /// <param name="gameTime"> The time elapsed in the game. </param>
        public void UpdateAnimation(GameTime gameTime)
        {
            // Check how much time has elapsed.
            timeElapsed += (float)gameTime.ElapsedGameTime.TotalSeconds;
            
            // If more time has elapsed then the seconds per frame value, increment the frame and reset the timer.
            if(timeElapsed >= secondsPerFrame)
            {
                timeElapsed = 0.0f;
                frame++;

                // Keep the frames bound in the spritesheet.
                if(frame > 5)
                {
                    frame = 0;
                }

                // Move the source rectangle accordingly.
                source.X = 185 * frame;
            }
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
        /// <param name="soccerball"> The soccerball's hitbox. </param>
        /// <returns> The type of collision. </returns>
        public BumpType CheckCollision(Rectangle soccerball)
        {
            Rectangle temp = Rectangle.Intersect(hitbox, soccerball);
            if(temp.Width > temp.Height)
            {
                return BumpType.Launch;
            }
            else
            {
                return BumpType.Side;
            }
        }

        /// <summary>
        /// Handles the overlap with the soccerball, if it's not meant to be launched.
        /// </summary>
        /// <param name="soccerball"></param>
        public void HandleOverlap(SoccerBall soccerball)
        {
            int temp = Rectangle.Intersect(hitbox, soccerball.Hitbox).Width;
            if(hitbox.X > soccerball.Hitbox.X)
            {
                soccerball.Translate(temp * -1);
            }
            else
            {
                soccerball.Translate(temp);
            }
        }

        /// <summary>
        /// Resets all of the seal's game values.
        /// </summary>
        public void Reset()
        {
            drawbox.X = 1645;
            drawbox.Y = 1780;
            hitbox.X = 1645;
            hitbox.Y = 1860;
            facing = Facing.Right;
            Speed = 20;
            ResetAnimation();
        }

        /// <summary>
        /// Resets the seal's animation.
        /// </summary>
        public void ResetAnimation()
        {
            frame = 0;
            timeElapsed = 0;
            source.X = 185 * frame;
        }
    }
}
