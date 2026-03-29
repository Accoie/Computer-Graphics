using OpenTK.Mathematics;
using Task1.Extensions;

namespace Task1.Models;

public class Player(
    Vector3 startPosition,
    Vector2 startRotation,
    float moveSpeed,
    float rotationSensitivity,
    float radius)
{
    private bool _firstRotation = true;
    private Vector2 _lastRotationPos;
    private Vector3 _position = startPosition;

    public Vector3 Position => _position;

    public float RotationY { get; private set; } = startRotation.Y;

    public float RotationX { get; private set; } = startRotation.X;

    public float Radius { get; } = radius;

    private float MoveSpeed { get; } = moveSpeed;
    private float RotationSensitivity { get; } = rotationSensitivity;

    public void UpdateRotation(float x, float y)
    {
        if (_firstRotation)
        {
            _lastRotationPos = new Vector2(x, y);
            _firstRotation = false;

            return;
        }

        float dx = x - _lastRotationPos.X;
        float dy = y - _lastRotationPos.Y;
        _lastRotationPos = new Vector2(x, y);

        RotationY += dx * RotationSensitivity;
        RotationX += dy * RotationSensitivity;

        RotationX = MathHelper.Clamp(RotationX, -89f, 89f);
    }

    public void UpdateMovement(Vector2 inputDirection, float dt)
    {
        if (inputDirection.LengthSquared <= 0)
        {
            return;
        }

        Vector3 forward = GetDirectionVector(RotationY - 90f);
        Vector3 right = GetDirectionVector(RotationY);

        Vector3 movementDirection = forward * inputDirection.Y + right * inputDirection.X;
        Vector3 movementDelta = movementDirection.Normalized() * MoveSpeed * dt;

        TryMove(movementDelta.X, movementDelta.Z);
    }

    private Vector3 GetDirectionVector(float rotationY)
    {
        float rad = MathHelper.DegreesToRadians(rotationY);

        return new Vector3((float)Math.Cos(rad), 0, (float)Math.Sin(rad)).Normalized();
    }

    private void TryMove(float dx, float dz)
    {
        float newX = _position.X + dx;
        float newZ = _position.Z + dz;

        if (!this.IsColliding(newX, _position.Z))
        {
            _position.X = newX;
        }

        if (!this.IsColliding(_position.X, newZ))
        {
            _position.Z = newZ;
        }
    }
}