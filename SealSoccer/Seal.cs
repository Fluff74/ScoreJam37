using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

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

        Texture2D galumphSpritesheet; // Used while the seal is moving.
        Rectangle hitbox; // This is where the seal is drawn from.

        public Seal(Texture2D galumphSpritesheet)
        {
            facing = Facing.Right; // Start the seal facing to the right.
            Speed = 5;

            this.galumphSpritesheet = galumphSpritesheet;
            hitbox = new Rectangle(1645, 1750, 550, 320);
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
                hitbox.X += Speed;

                // Make the seal face right, if it is currently facing left.
                if(facing == Facing.Left)
                {
                    facing = Facing.Right;
                }
            }

            // Move the seal to the left.
            else
            {
                hitbox.X -= Speed;

                // Make the seal face left, if it is currently facing right.
                if(facing == Facing.Right)
                {
                    facing = Facing.Left;
                }
            }
        }

        public void Draw(SpriteBatch sb)
        {
            sb.Draw(galumphSpritesheet, hitbox, Color.White);
        }

        public void Reset()
        {
            hitbox.X = 1645;
            hitbox.Y = 1750;
            facing = Facing.Right;
        }
    }
}
