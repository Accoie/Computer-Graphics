using OpenTK.Mathematics;

namespace Task3;

public static class GameConfig
{
    public static class Lighting
    {
        public static readonly float[] LightAmbient = [0.10f, 0.10f, 0.10f, 1.0f];
        public static readonly float[] LightDiffuse = [0.8f, 0.8f, 0.8f, 1.0f];
        public static readonly float[] LightSpecular = [0.8f, 0.8f, 0.8f, 1.0f];
            
        public const float ConstantAttenuation = 1.0f;
        public const float LinearAttenuation = 0.05f;
        public const float QuadraticAttenuation = 0.012f;
    }

    public static class Player
    {
        public const float MoveSpeed = 4.0f;
        public const float MouseSensitivity = 0.12f;
        public const float Radius = 0.38f;
        public static readonly Vector3 StartPosition = new(8.5f, 1.6f, 8.5f);
        public static readonly Vector2 StartRotation = new(0f, 0f);
    }

    public static class World
    {
        public const float WallHeight = 3.0f;
        
        /// <summary>
        /// 0 - пол, 1 - стена
        /// </summary>
        public static readonly int[,] LabyrinthMap = new[,] 
        {
            { 1, 1, 1, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1 },
            { 1, 0, 1, 0, 0, 0, 1, 1, 0, 1, 1, 0, 0, 1, 0, 1 },
            { 1, 0, 1, 1, 0, 1, 0, 0, 0, 1, 1, 0, 1, 1, 0, 1 },
            { 1, 0, 1, 0, 0, 1, 1, 1, 0, 1, 1, 0, 1, 1, 0, 1 },
            { 1, 0, 1, 0, 1, 1, 0, 0, 0, 1, 0, 1, 0, 1, 1, 1 },
            { 1, 0, 1, 0, 1, 1, 0, 1, 1, 1, 1, 0, 1, 0, 1, 1 },
            { 1, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 1, 1, 1, 0, 1 },
            { 1, 0, 1, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 0, 0, 1 },
            { 1, 0, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1 },
            { 1, 1, 0, 1, 0, 0, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1 },
            { 1, 0, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 1, 1 },
            { 1, 0, 1, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 0, 0, 1 },
            { 1, 0, 0, 0, 0, 1, 0, 1, 0, 1, 0, 1, 1, 1, 0, 1 },
            { 1, 1, 1, 0, 1, 1, 0, 1, 0, 1, 0, 0, 0, 0, 0, 1 },
            { 1, 0, 0, 0, 0, 1, 0, 0, 0, 1, 1, 1, 1, 1, 0, 1 },
            { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 }
        };
    }

    public static class Camera
    {
        public const float FovDegrees = 70f;
        public const float NearPlane = 0.1f;
        public const float FarPlane = 30f;
    }
}
