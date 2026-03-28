using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Task3.Handlers;

public class InputHandler
{
    public Vector2 GetMovementDirection(KeyboardState keyboard)
    {
        Vector2 direction = Vector2.Zero;
        
        if (keyboard.IsKeyDown(Keys.W))
        {
            direction.Y += 1f;
        }

        if (keyboard.IsKeyDown(Keys.S))
        {
            direction.Y -= 1f;
        }

        if (keyboard.IsKeyDown(Keys.D))
        {
            direction.X += 1f;
        }

        if (keyboard.IsKeyDown(Keys.A))
        {
            direction.X -= 1f;
        }

        if (direction.LengthSquared > 1f)
        {
            direction = direction.Normalized();
        }

        return direction;
    }
}