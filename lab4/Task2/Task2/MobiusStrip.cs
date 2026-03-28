using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace Task2;

public class MobiusStrip
{
    private const double StripRadius = 1;
    private const double StripHalfWidth = 0.5;
    private readonly Vector3 _gridColor = new(0.2f, 0.2f, 0.2f);

    public void Draw()
    {
        DrawShape();

        DrawGrid();
    }

    private void DrawShape()
    {
        GL.Begin(PrimitiveType.QuadStrip);

        const double step = 0.05;
        for (double s = 0; s <= 1; s += step)
        {
            double t;
            for (t = 0; t < 2 * Math.PI + step; t += step)
            {
                SetVertexWithColor(t, s);
                SetVertexWithColor(t, s + step);
            }
        }

        GL.End();
    }

    private void DrawGrid()
    {
        GL.LineWidth(2);
        GL.Color3(_gridColor);

        DrawHorizontalLines();
        DrawVerticalLines();
    }

    private void DrawVerticalLines()
    {
        GL.Begin(PrimitiveType.Lines);

        const double horizontalStep = 0.4;

        for (double t = 0; t <= 2 * Math.PI; t += horizontalStep)
        {
            const double verticalStep = 0.02;

            for (double s = 0; s < 1.0; s += verticalStep)
            {
                SetVertex(t, s);
                SetVertex(t, s + verticalStep);
            }
        }

        GL.End();
    }

    private void DrawHorizontalLines()
    {
        GL.Begin(PrimitiveType.Lines);

        const double verticalStep = 0.05;
        
        for (double s = 0; s <= 1; s += verticalStep)
        {
            const double horizontalStep = 0.05;
                
            for (double t = 0; t <= 2 * Math.PI; t += horizontalStep)
            {
                SetVertex(t, s);
                SetVertex(t + horizontalStep, s);
            }
        }

        GL.End();
    }

    private void SetColorByCoords(Vector3 p)
    {
        p.Normalize();

        float r = Math.Abs(p.X);
        float g = Math.Abs(p.Y);
        float b = Math.Abs(p.Z);

        GL.Color3(r, g, b);
    }

    private void SetVertexWithColor(double t, double s)
    {
        Vector3 vertex = CalculateVertex(t, s);

        SetColorByCoords(vertex);
        GL.Vertex3(vertex);
    }

    private void SetVertex(double t, double s)
    {
        Vector3 vertex = CalculateVertex(t, s);

        GL.Vertex3(vertex);
    }

    private static Vector3 CalculateVertex(double t, double s)
    {
        double alphaX = (StripRadius + StripHalfWidth * Math.Cos(t / 2)) * Math.Cos(t);
        double alphaY = (StripRadius + StripHalfWidth * Math.Cos(t / 2)) * Math.Sin(t);
        double alphaZ = StripHalfWidth * Math.Sin(t / 2);

        double betaX = (StripRadius + StripHalfWidth * Math.Cos(Math.PI + t / 2)) * Math.Cos(t);
        double betaY = (StripRadius + StripHalfWidth * Math.Cos(Math.PI + t / 2)) * Math.Sin(t);
        double betaZ = StripHalfWidth * Math.Sin(Math.PI + t / 2);

        float x = (float)((1 - s) * alphaX + s * betaX);
        float y = (float)((1 - s) * alphaY + s * betaY);
        float z = (float)((1 - s) * alphaZ + s * betaZ);

        return new Vector3(x, y, z);
    }
}