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
        Emitter emitter;

        Texture2D texture;
        Vector2 position;
        Vector2 direction;
        Vector2 origin;
        float velocity;
        float initialScale;
        float scale;
        int elapsed;
        int time;
        float alphaDecayTime;
        float alphaDecayStart;
        float scaleDecayTime;
        float scaleDecayStart;
        Color color;

        public Particle(Emitter emitter, Texture2D texture, Vector2 position, Vector2 direction, 
            float velocity, float scale, int time, float alphaDecayFraction, float scaleDecayFraction)
        {
            this.emitter = emitter;
            this.texture = texture;
            this.position = position;
            this.direction = direction;
            this.velocity = velocity;
            this.time = time;
            this.initialScale = scale;
            this.scale = scale;
            origin = new Vector2(texture.Width / 2.0f, texture.Height / 2.0f);
            alphaDecayTime = time * alphaDecayFraction;
            alphaDecayStart = time - alphaDecayTime;
            scaleDecayTime = time * scaleDecayFraction;
            scaleDecayStart = time - scaleDecayTime;
            color = Color.White;
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

            if (elapsed > alphaDecayStart)
            {
                float decayDelta = elapsed - alphaDecayStart;
                float percent = 1.0f - (1.0f / alphaDecayTime) * decayDelta;
                color = Color.FromNonPremultiplied(color.R, color.G, color.B, (byte)(255.0f * percent));
            }

            if (elapsed > scaleDecayStart)
            {
                float decayDelta = elapsed - scaleDecayStart;
                float percent = 1.0f - (1.0f / scaleDecayTime) * decayDelta;
                scale = initialScale * percent;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, null, color, 0.0f, origin, scale, SpriteEffects.None, 0);
        }
    }
}
