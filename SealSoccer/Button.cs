using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SealSoccer
{
    // Joshua Smith
    // 12/22/2025
    //
    // This is the button class. It functions as a button for players to click, in our game it really just allows them to move
    // between menu states.
    internal class Button
    {
        readonly string label;
        readonly SpriteFont font;
        readonly Vector2 location;
        readonly Rectangle hitbox;
        bool hovering;
        bool holding;

        public Button(string label, SpriteFont font, Vector2 location, Rectangle hitbox)
        {
            this.label = label;
            this.font = font;
            this.location = location;
            this.hitbox = hitbox;

            // Initialize these as false.
            hovering = false;
            holding = false;
        }

        /// <summary>
        /// Updates the internal values of the mouse that are used for drawing the button, but also checks to see if it has
        /// been clicked.
        /// </summary>
        /// <param name="ms"> The current state of the mouse. </param>
        /// <param name="pms"> The previous state of the mouse. </param>
        /// <param name="adjustedMouseLocation"> The adjusted location of the mouse, based on the screen resolution. </param>
        /// <returns> Whether or not the button has been clicked. </returns>
        public bool Update(MouseState ms, MouseState pms, Point adjustedMouseLocation)
        {
            // Check to see if their mouse is over the button.
            hovering = hitbox.Contains(adjustedMouseLocation);
            
            // Check to see if they're currently holding down their left mouse button.
            holding = ms.LeftButton == ButtonState.Pressed;

            // They clicked the button if they are hovering over it, they're not holding their left mouse button down anymore,
            // but their left mouse button WAS down in the previous frame.
            return hovering && !holding && pms.LeftButton == ButtonState.Pressed;
        }

        public void Draw(SpriteBatch sb)
        {
            // If the button is being hovered over, change colors accordingly.
            if(hovering)
            {
                // If the player is holding down on the button, draw it with a specific color.
                if(holding)
                {
                    sb.DrawString(font, label, location, Color.DarkBlue);
                }

                // If the player is not holding down on the button, draw it with a specific color.
                else
                {
                    sb.DrawString(font, label, location, Color.SteelBlue);
                }
            }

            // If the button is not being hovered over, draw it with a specific color.
            else
            {
                sb.DrawString(font, label, location, Color.LightSteelBlue);
            }
        }
    }
}
