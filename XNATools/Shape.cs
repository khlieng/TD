using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace XNATools
{
    public abstract class Shape
    {
        public static bool UseVertexBuffer { get; set; }

        public PrimitiveType PrimitiveType;
        public VertexPositionColor[] Vertices;
        public VertexBuffer VertexBuffer;
    }
}
