using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TD
{
    class Animation : GameComponent
    {        
        public Texture2D Texture { get; private set; }

        private Texture2D[] textures;
        private int currentFrame;
        private int frameTime;
        private int elapsed;

        public Animation(Game game, Texture2D[] textures, int frameTime)
            : base(game)
        {
            this.textures = textures;
            this.frameTime = frameTime;
            Texture = textures[currentFrame];

            game.Components.Add(this);
        }

        public override void Update(GameTime gameTime)
        {
            elapsed += gameTime.ElapsedGameTime.Milliseconds;
            if (elapsed >= frameTime)
            {
                elapsed -= frameTime;

                currentFrame = ++currentFrame % textures.Length;
                Texture = textures[currentFrame];
            }

            base.Update(gameTime);
        }
    }
}
