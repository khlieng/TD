using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace TD
{
    class Input : GameComponent
    {
        private KeyboardState prevKeyState;
        private MouseState prevMouseState;

        private Keys[] prevKeysDown;

        public event Action<Keys> KeyPressed;
        public event Action<Keys> KeyReleased;

        public Input(Game game)
            : base(game)
        {
            prevKeysDown = new Keys[0];
        }

        public override void Update(GameTime gameTime)
        {
            KeyboardState keyState = Keyboard.GetState();
            Keys[] keysDown = keyState.GetPressedKeys();
            MouseState mouseState = Mouse.GetState();

            foreach (Keys key in keysDown)
            {
                if (prevKeyState.IsKeyUp(key))
                {
                    OnKeyPressed(key);
                }
            }

            foreach (Keys key in prevKeysDown)
            {
                if (keyState.IsKeyUp(key))
                {
                    OnKeyReleased(key);
                }
            }

            prevKeyState = keyState;
            prevKeysDown = keysDown;
            prevMouseState = mouseState;

            base.Update(gameTime);
        }

        protected virtual void OnKeyPressed(Keys key)
        {
            if (KeyPressed != null)
            {
                KeyPressed(key);
            }
        }

        protected virtual void OnKeyReleased(Keys key)
        {
            if (KeyReleased != null)
            {
                KeyReleased(key);
            }
        }
    }
}
