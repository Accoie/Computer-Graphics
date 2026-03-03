using OpenTK.Mathematics;

namespace Task2.Configs;

public static class AquariumConfig
{
    public const float WindowWidth = 800f;
    public const float WindowHeight = 600f;

    public static readonly Vector2[] FishBasePositions =
    [
        new(370.0f, -100.0f),
        new(250.0f, 300.0f),
        new(-400.0f, -200.0f),
        new(-170.0f, 50.0f),
        new(440.0f, -301.0f)
    ];

    public static readonly float[] FishSpeeds = [2.5f, 2.2f, 2.8f, 2.0f, 3.0f];

    public static readonly float[] FishInitialOffsets = [-1450, -850, -1200, -950, -600];

    public static readonly float[] FishBubbleIntervals = [2.9f, 1.7f, 1.7f, 2.2f, 3.9f];

    public const int MinBubbleOffsetX = 10;
    public const int MaxBubbleOffsetX = 30;
    public const int MinBubbleOffsetY = -20;
    public const int MaxBubbleOffsetY = 20;
    public const int MinBubbleSize = 8;
    public const int MaxBubbleSize = 15;
    public const int MinBubbleSpeedY = 40;
    public const int MaxBubbleSpeedY = 100;
    public const int MinBubbleSpeedX = -5;
    public const int MaxBubbleSpeedX = 5;
    public const int NumBubblesPerFish = 1;

    public const int NumBubblesArraySize = 5;

    public const float FishMovementSpeedMultiplier = 100f;
    public const float FishLeftBoundOffset = 190f;
    public const float FishRightResetPositionOffset = 100f;

    public const float ProjectionWidth = 1600;
    public const float ProjectionHeight = 1200;
    public const float ProjectionNearPlane = -1;
    public const float ProjectionFarPlane = 1;

    public const float BubbleHighlightMultiplier = 0.3f;

    public const double BubbleSinWaveFrequency = 3;
    public const double BubbleSinWaveAmplitude = 3;
}