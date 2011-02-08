using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TD
{
    class Particle
    {
        Texture2D texture;
        Vector2 position;
        Vector2 direction;
        float velocity;
        int elapsed;
        int time;
        Emitter emitter;

        public Particle(Emitter emitter, Texture2D texture, Vector2 position, Vector2 direction, float velocity, int time)
        {
            this.emitter = emitter;
            this.texture = texture;
            this.position = position;
            this.direction = direction;
            this.velocity = velocity;
            this.time = time;

            emitter.Add(this);
        }

        public void Update(GameTime gameTime)
        {
            elapsed += gameTime.ElapsedGameTime.Milliseconds;
            float elapsedSeconds = gameTime.ElapsedGameTime.Milliseconds / 1000.0f;
            position += direction * velocity * elapsedSeconds;

            if (elapsed >= time)
            {
                emitter.Remove(this);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {            
            spriteBatch.Draw(texture, new Rectangle((int)position.X - 8, (int)position.Y - 8, 16, 16), Color.White);
        }
    }
}
