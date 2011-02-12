using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace XNATools
{
    public static class Draw
    {
        static GraphicsDevice device;
        static BasicEffect effect;

        public static void Init(GraphicsDevice graphicsDevice)
        {
            device = graphicsDevice;
            
            effect = new BasicEffect(device);
            effect.VertexColorEnabled = true;
            effect.Projection = Matrix.CreateOrthographicOffCenter(0, graphicsDevice.Viewport.Width, 
                graphicsDevice.Viewport.Height, 0, 0, 1);
        }

        public static void Line(Vector2 from, Vector2 to, Color color)
        {
            VertexPositionColor[] vertices = 
            { 
                new VertexPositionColor(new Vector3(from, 0), color), 
                new VertexPositionColor(new Vector3(to, 0), color) 
            };

            DrawVertices(vertices, PrimitiveType.LineList);
        }

        public static void Rect(Rectangle rect, Color color)
        {
            Rect(new Vector2(rect.X, rect.Y), new Vector2(rect.Width, rect.Height), color);
        }

        public static void Rect(Vector2 position, Vector2 size, Color color)
        {
            VertexPositionColor[] vertices =
            {
                new VertexPositionColor(new Vector3(position.X, position.Y, 0), color),
                new VertexPositionColor(new Vector3(position.X + size.X, position.Y, 0), color),
                new VertexPositionColor(new Vector3(position.X + size.X, position.Y + size.Y, 0), color),
                new VertexPositionColor(new Vector3(position.X, position.Y + size.Y, 0), color),
                new VertexPositionColor(new Vector3(position.X, position.Y, 0), color)
            };

            DrawVertices(vertices, PrimitiveType.LineStrip);
        }

        public static void FilledRect(Rectangle rect, Color color)
        {
            FilledRect(new Vector2(rect.X, rect.Y), new Vector2(rect.Width, rect.Height), color);                       
        }

        public static void FilledRect(Vector2 position, Vector2 size, Color color)
        {
            VertexPositionColor[] vertices =
            {
                new VertexPositionColor(new Vector3(position.X, position.Y, 0), color),
                new VertexPositionColor(new Vector3(position.X + size.X, position.Y, 0), color),
                new VertexPositionColor(new Vector3(position.X, position.Y + size.Y, 0), color),
                new VertexPositionColor(new Vector3(position.X + size.X, position.Y, 0), color),
                new VertexPositionColor(new Vector3(position.X + size.X, position.Y + size.Y, 0), color),
                new VertexPositionColor(new Vector3(position.X, position.Y + size.Y, 0), color)
            };

            DrawVertices(vertices, PrimitiveType.TriangleList);
        }

        private static void DrawVertices(VertexPositionColor[] vertices, PrimitiveType type)
        {
            if (device != null)
            {
                foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                {
                    pass.Apply();

                    switch (type)
                    {
                        case PrimitiveType.LineList:
                            device.DrawUserPrimitives<VertexPositionColor>(type, vertices, 0, vertices.Length / 2);
                            break;

                        case PrimitiveType.TriangleList:
                            device.DrawUserPrimitives<VertexPositionColor>(type, vertices, 0, vertices.Length / 3);
                            break;

                        case PrimitiveType.LineStrip:
                            device.DrawUserPrimitives<VertexPositionColor>(type, vertices, 0, vertices.Length - 1);
                            break;

                        case PrimitiveType.TriangleStrip:
                            device.DrawUserPrimitives<VertexPositionColor>(type, vertices, 0, vertices.Length - 2);
                            break;
                    }
                }
            }
        }
    }
}
