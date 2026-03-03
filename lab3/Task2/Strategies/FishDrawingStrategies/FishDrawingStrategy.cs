using OpenTK.Graphics.OpenGL;
using Task2.Models;

namespace Task2.Strategies.FishDrawingStrategies;

public abstract class FishDrawingStrategy : IDrawingStrategy<Fish>
{
    protected Fish Fish = null!;
    public void Draw(Fish fish)
    {
        Fish = fish;
        
        float x = fish.Position.X;
        float y = fish.Position.Y;
        float rx = fish.Size.X;
        float ry = fish.Size.Y;

        GL.PushMatrix();
        GL.Translate(-fish.Offset, 0.0f, 0.0f);

        DrawBody(x, y, rx, ry);
        DrawTail(x, y, rx);
        DrawEye(x, y);

        GL.PopMatrix();
    }

    public abstract void DrawBody(float x, float y, float rx, float ry);
    public abstract void DrawTail(float x, float y, float rx);
    public abstract void DrawEye(float x, float y);
}