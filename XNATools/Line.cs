using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace XNATools
{
    public class Line : Shape
    {
        public Line(Vector2 p1, Vector2 p2, Color color)
        {
            PrimitiveType = PrimitiveType.LineList;
            Vertices = new[]
            {
                new VertexPositionColor(new Vector3(p1, 0), color),
                new VertexPositionColor(new Vector3(p2, 0), color)
            };
        }
    }
}
