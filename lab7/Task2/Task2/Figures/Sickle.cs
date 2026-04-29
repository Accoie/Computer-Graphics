using OpenTK.Graphics.OpenGL;

namespace Task2.Figures;

public class Sickle : Figure
{
    public override void CreatePoints()
    {
        float r = 1f;
        float g = 1f;
        float b = 0f;

        CreateBlade(r, g, b);
        CreateSickleHandle(r, g, b);
    }

    private void CreateBlade(float r, float g, float b)
    {
        float cx = -0.70f;
        float cy = 0.25f;
        float radius = 0.1f;
        double startAngle = 225;
        double endAngle = 450;
        double angleWithMaxWidth = 270;
        
        float x1, y1, x2, y2;
        List<float> points = [];

        for (double angle = startAngle; angle <= endAngle; angle++)
        {
            float offset = (float)(1 - Math.Abs(angleWithMaxWidth - angle) / 180) * 0.01f;
            float angleRad = (float)(Math.Round(angle) * Math.PI / 180);

            x1 = cx + (radius + offset) * (float)Math.Cos(angleRad);
            y1 = cy + (radius + offset) * (float)Math.Sin(angleRad);
            points.AddRange([x1, y1, 0f, r, g, b]);

            x2 = cx + (radius - offset) * (float)Math.Cos(angleRad);
            y2 = cy + (radius - offset) * (float)Math.Sin(angleRad);
            points.AddRange([x2, y2, 0f, r, g, b]);
        }

        CreateAndSetUpBuffer(points.ToArray(), PrimitiveType.TriangleStrip);
    }

    private void CreateSickleHandle(float r, float g, float b)
    {
        float x1 = -0.775f, y1 = 0.175f;
        float x2 = -0.765f, y2 = 0.165f;
        float x3 = -0.807f, y3 = 0.115f;
        float x4 = -0.825f, y4 = 0.13f;
        
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