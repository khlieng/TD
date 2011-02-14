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

        public Particle(Emitter emitter, Texture2D texture, Vector2 position, Vector2 direction, float velocity, float scale, int time)
        {
            this.emitter = emitter;
            this.texture = texture;
            this.position = position;
            this.direction = direction;
            this.velocity = velocity;
            this.time = time;
            this.scale = scale;
            origin = new Vector2(texture.Width / 2.0f, texture.Height / 2.0f);

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
            spriteBatch.Draw(texture, position, null, Color.White, 0.0f, origin, scale, SpriteEffects.None, 0);
        }
    }
}
