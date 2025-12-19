using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SealSoccer
{
    // Joshua Smith
    // 12/19/2025
    //
    // This is the seal that the player will control in the game! It walks left and right, and headbutts the ball. So, it needs
    // three controls. It'll have two animations, one for galumphing, and one for the headbutt.
    internal class Seal
    {
        Texture2D galumphSpritesheet; // Used while the seal is moving.
        Texture2D headbuttSpritesheet; // Used while the seal is headbutting.

        Rectangle drawbox; // This is where the seal is drawn from.
        Rectangle headbuttBox; // This is the hitbox of the seal's headbutt move.

        public Seal(Texture2D galumphSpritesheet, Texture2D headbuttSpritesheet)
        {
            this.galumphSpritesheet = galumphSpritesheet;
            this.headbuttSpritesheet = headbuttSpritesheet;
        }

        public void Update(GameTime gameTime)
        {

        }

        public void Move()
        {

        }

        public bool Headbutt(Rectangle soccerball)
        {
            return false;
        }

        public void Draw(SpriteBatch sb)
        {

        }
    }
}
