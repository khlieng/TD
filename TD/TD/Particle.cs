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
        Vector2 origin;
        float velocity;
        float scale;
        int elapsed;
        int time;
        Emitter emitter;

        float decayTime;
        float decayStart;
        Color color = Color.White;

        public Particle(Emitter emitter, Texture2D texture, Vector2 position, Vector2 direction, 
            float velocity, float scale, int time, float decayFraction)
        {
            this.emitter = emitter;
            this.texture = texture;
            this.position = position;
            this.direction = direction;
            this.velocity = velocity;
            this.time = time;
            this.scale = scale;
            origin = new Vector2(texture.Width / 2.0f, texture.Height / 2.0f);
            decayTime = time * decayFraction;
            decayStart = time - decayTime;

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

            if (elapsed > decayStart)
            {
                float decayDelta = elapsed - decayStart;
                float percent = 1.0f - (1.0f / decayTime) * decayDelta;
                color.A = (byte)(255.0f * percent);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, null, color, 0.0f, origin, scale, SpriteEffects.None, 0);
        }
    }
}
