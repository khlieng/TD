using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace XNATools
{
    public class Rect : Shape
    {
        public Rect(Rectangle rect, Color color, bool filled)
            : this(new Vector2(rect.X, rect.Y), new Vector2(rect.Width, rect.Height), color, filled)
        {
        }

        public Rect(Vector2 position, Vector2 size, Color color, bool filled)
        {
            if (filled)
            {
                PrimitiveType = PrimitiveType.TriangleList;
                Vertices = new[]
                {
                    new VertexPositionColor(new Vector3(position.X, position.Y, 0), color),
                    new VertexPositionColor(new Vector3(position.X + size.X, position.Y, 0), color),
                    new VertexPositionColor(new Vector3(position.X, position.Y + size.Y, 0), color),
                    new VertexPositionColor(new Vector3(position.X + size.X, position.Y, 0), color),
                    new VertexPositionColor(new Vector3(position.X + size.X, position.Y + size.Y, 0), color),
                    new VertexPositionColor(new Vector3(position.X, position.Y + size.Y, 0), color)
                };
            }
            else
            {
                PrimitiveType = PrimitiveType.LineStrip;
                Vertices = new[]
                {
                    new VertexPositionColor(new Vector3(position.X, position.Y, 0), color),
                    new VertexPositionColor(new Vector3(position.X + size.X, position.Y, 0), color),
                    new VertexPositionColor(new Vector3(position.X + size.X, position.Y + size.Y, 0), color),
                    new VertexPositionColor(new Vector3(position.X, position.Y + size.Y, 0), color),
                    new VertexPositionColor(new Vector3(position.X, position.Y, 0), color)
                };
            }
        }
    }
}
