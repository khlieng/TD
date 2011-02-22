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
        public Circle(Vector2 center, float radius, int subdivs, Color color, bool filled, float thickness = 1.0f)
        {
            VertexPositionColor[] verts = CreateVertices(center, radius, subdivs, color, filled, thickness);

            if (UseVertexBuffer)
            {
                VertexBuffer = new VertexBuffer(GameHelper.Game.GraphicsDevice,
                    typeof(VertexPositionColor), verts.Length, BufferUsage.WriteOnly);
                VertexBuffer.SetData<VertexPositionColor>(verts);
            }
            else
            {
                Vertices = verts;
            }
        }

        private VertexPositionColor[] CreateVertices(Vector2 center, float radius, 
            int subdivs, Color color, bool filled, float thickness)
        {
            if (filled)
            {
                PrimitiveType = PrimitiveType.TriangleList;
                VertexPositionColor[] verts = new VertexPositionColor[subdivs * 3];

                Vector2 direction = new Vector2(0, -1);
                for (int i = 0; i < verts.Length; i += 3)
                {
                    verts[i] = new VertexPositionColor(new Vector3(center, 0f), color);
                    verts[i + 1] = new VertexPositionColor(new Vector3(center + direction * radius, 0), color);
                    direction = Vector2.Transform(direction, Matrix.CreateRotationZ(MathHelper.TwoPi / subdivs));
                    verts[i + 2] = new VertexPositionColor(new Vector3(center + direction * radius, 0), color);
                }

                return verts;
            }
            else
            {
                if (thickness > 1.0f)
                {
                    PrimitiveType = PrimitiveType.TriangleList;
                    VertexPositionColor[] verts = new VertexPositionColor[subdivs * 6];

                    float halfThickness = 0.5f * thickness;
                    float innerRadius = radius - halfThickness;
                    float outerRadius = radius + halfThickness;

                    Vector2 direction = new Vector2(0, -1);
                    for (int i = 0; i < verts.Length; i += 6)
                    {
                        Vector2 rotatedDirection = Vector2.Transform(direction, Matrix.CreateRotationZ(MathHelper.TwoPi / subdivs));

                        verts[i] = new VertexPositionColor(new Vector3(center + direction * innerRadius, 0f), color);
                        verts[i + 1] = new VertexPositionColor(new Vector3(center + direction * outerRadius, 0f), color);
                        verts[i + 2] = new VertexPositionColor(new Vector3(center + rotatedDirection * innerRadius, 0f), color);

                        verts[i + 3] = new VertexPositionColor(new Vector3(center + direction * outerRadius, 0f), color);
                        verts[i + 4] = new VertexPositionColor(new Vector3(center + rotatedDirection * outerRadius, 0f), color);
                        verts[i + 5] = new VertexPositionColor(new Vector3(center + rotatedDirection * innerRadius, 0f), color);

                        direction = rotatedDirection;
                    }

                    return verts;
                }
                else
                {
                    PrimitiveType = PrimitiveType.LineStrip;
                    VertexPositionColor[] verts = new VertexPositionColor[subdivs + 1];

                    Vector2 direction = new Vector2(0, -1);
                    for (int i = 0; i < Vertices.Length; i++)
                    {
                        verts[i] = new VertexPositionColor(new Vector3(center + direction * radius, 0f), color);
                        direction = Vector2.Transform(direction, Matrix.CreateRotationZ(MathHelper.TwoPi / subdivs));
                    }

                    return verts;
                }
            }
        }
    }
}
