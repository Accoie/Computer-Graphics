using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace Task2.Tools;

public static class DrawingTools
{
    public static void DrawEllipse(float x, float y, float radiusX, float radiusY, Color4 color, PrimitiveType primitiveType =  PrimitiveType.Polygon)
    {
        GL.Begin(primitiveType);
        GL.Color4(color);
        int segments = 360;
        
        for (int i = 0; i < segments; i++)
        {
            float angle = i * (float)Math.PI / 180;
            GL.Vertex2(x + Math.Cos(angle) * radiusX, y + Math.Sin(angle) * radiusY);
        }
        
        GL.End();
    }
}