using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace XNATools
{
    public class Circle : Shape
    {
        public Circle(Vector2 center, float radius, int subdivs, Color color, bool filled)
        {
            if (filled)
            {
                PrimitiveType = PrimitiveType.TriangleList;
                Vertices = new VertexPositionColor[subdivs * 3];

                Vector2 direction = new Vector2(0, -1);
                for (int i = 0; i < Vertices.Length; i += 3)
                {
                    Vertices[i] = new VertexPositionColor(new Vector3(center, 0f), color);
                    Vertices[i + 1] = new VertexPositionColor(new Vector3(center + direction * radius, 0), color);
                    direction = Vector2.Transform(direction, Matrix.CreateRotationZ(MathHelper.TwoPi / subdivs));
                    Vertices[i + 2] = new VertexPositionColor(new Vector3(center + direction * radius, 0), color);
                }
            }
            else
            {
                PrimitiveType = PrimitiveType.LineStrip;
                Vertices = new VertexPositionColor[subdivs + 1];

                Vector2 direction = new Vector2(0, -1);
                for (int i = 0; i < Vertices.Length; i++)
                {
                    Vertices[i] = new VertexPositionColor(new Vector3(center + direction * radius, 0f), color);
                    direction = Vector2.Transform(direction, Matrix.CreateRotationZ(MathHelper.TwoPi / subdivs));
                }
            }
        }
    }
}
