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

        public static void Circle(Vector2 center, float radius, int subdivs, Color color)
        {
            VertexPositionColor[] vertices = new VertexPositionColor[subdivs + 1];

            Vector2 direction = new Vector2(0, -1);
            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i] = new VertexPositionColor(new Vector3(center + direction * radius, 0f), color);
                direction = Vector2.Transform(direction, Matrix.CreateRotationZ(MathHelper.TwoPi / subdivs));
            }

            DrawVertices(vertices, PrimitiveType.LineStrip);
        }

        public static void FilledCircle(Vector2 center, float radius, int subdivs, Color color)
        {
            VertexPositionColor[] vertices = new VertexPositionColor[subdivs * 3];

            Vector2 direction = new Vector2(0, -1);
            for (int i = 0; i < vertices.Length; i += 3)
            {
                vertices[i] = new VertexPositionColor(new Vector3(center, 0f), color);
                vertices[i + 1] = new VertexPositionColor(new Vector3(center + direction * radius, 0), color);
                direction = Vector2.Transform(direction, Matrix.CreateRotationZ(MathHelper.TwoPi / subdivs));
                vertices[i + 2] = new VertexPositionColor(new Vector3(center + direction * radius, 0), color);
            }

            DrawVertices(vertices, PrimitiveType.TriangleList);
        }

        public static void Shape(Shape shape)
        {
            if (shape.VertexBuffer == null)
            {
                DrawVertices(shape.Vertices, shape.PrimitiveType);
            }
            else
            {
                DrawVertexBuffer(shape.VertexBuffer, shape.PrimitiveType);
            }
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

        private static void DrawVertexBuffer(VertexBuffer buffer, PrimitiveType type)
        {
            if (device != null)
            {
                device.SetVertexBuffer(buffer);

                foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                {
                    pass.Apply();

                    switch (type)
                    {
                        case PrimitiveType.LineList:
                            device.DrawPrimitives(type, 0, buffer.VertexCount / 2);
                            break;

                        case PrimitiveType.TriangleList:
                            device.DrawPrimitives(type, 0, buffer.VertexCount / 3);
                            break;

                        case PrimitiveType.LineStrip:
                            device.DrawPrimitives(type, 0, buffer.VertexCount - 1);
                            break;

                        case PrimitiveType.TriangleStrip:
                            device.DrawPrimitives(type, 0, buffer.VertexCount - 2);
                            break;
                    }
                }
            }
        }
    }
}
