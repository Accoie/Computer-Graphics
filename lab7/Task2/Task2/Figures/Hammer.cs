using OpenTK.Graphics.OpenGL;

namespace Task2.Figures;

public class Hammer : Figure
{
    public override void CreatePoints()
    {
        float r = 1f;
        float g = 1f;
        float b = 0f;

        CreateHammerHead(r, g, b);
        CreateHammerHandle(r, g, b);
    }

    private void CreateHammerHead(float r, float g, float b)
    {
        float x1 = -0.812f, y1 = 0.297f;
        float x2 = -0.790f, y2 = 0.265f;
        float x3 = -0.750f, y3 = 0.295f;
        float x4 = -0.710f, y4 = 0.337f;
        
        float[] points =
        [
            x1, y1, 0f, r, g, b,
            x2, y2, 0f, r, g, b,
            x3, y3, 0f, r, g, b,
            x4, y4, 0f, r, g, b,
        ];

        CreateAndSetUpBuffer(points, PrimitiveType.TriangleFan);
    }

    private void CreateHammerHandle(float r, float g, float b)
    {
        float x1 = -0.580f, y1 = 0.150f;
        float x2 = -0.595f, y2 = 0.135f;
        float x3 = -0.760f, y3 = 0.295f;
        float x4 = -0.750f, y4 = 0.307f;
        
        float[] points =
        [
            x1, y1, 0f, r, g, b,
            x2, y2, 0f, r, g, b,
            x3, y3, 0f, r, g, b,
            x4, y4, 0f, r, g, b,
        ];

        CreateAndSetUpBuffer(points, PrimitiveType.TriangleFan);
    }
}