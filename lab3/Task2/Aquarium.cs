using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using Task2.Configs;
using Task2.Models;
using Task2.Strategies.FishDrawingStrategies;
using Task2.Strategies.StoneDrawingStrategies;
using Task2.Tools;

namespace Task2;

public class AquariumWindow(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
    : GameWindow(gameWindowSettings, nativeWindowSettings)
{
    private const float WindowWidth = AquariumConfig.WindowWidth;
    private const float WindowHeight = AquariumConfig.WindowHeight;

    private const float PlantSegmentWidth = 11.0f;
    private const float BubbleHighlightMultiplier = 0.6f;

    private static readonly Vector2[] FishBasePositions = AquariumConfig.FishBasePositions;

    private readonly float[] _fishSpeeds = AquariumConfig.FishSpeeds;
    private readonly float[] _fishOffsets = AquariumConfig.FishInitialOffsets;
    private readonly float[] _bubbleTimers = new float[AquariumConfig.NumBubblesArraySize];

    private readonly List<Bubble> _bubbles = [];
    private readonly Random _random = new();

    private readonly List<Fish> _fishes = [];
    private readonly List<Stone> _stones = [];

    protected override void OnLoad()
    {
        base.OnLoad();
        GL.ClearColor(0.4f, 0.7f, 0.5f, 1.0f);
        GL.Enable(EnableCap.Blend);
        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

        InitializeFishes();
        InitializeStones();
    }

    protected override void OnResize(ResizeEventArgs e)
    {
        base.OnResize(e);
        GL.Viewport(0, 0, Size.X, Size.Y);
        AdjustProjection();
        SwapBuffers();
    }

    protected override void OnUpdateFrame(FrameEventArgs args)
    {
        base.OnUpdateFrame(args);
        float deltaTime = (float)args.Time;

        UpdateFishPositions(deltaTime);
        UpdateBubbles(deltaTime);

        for (int i = 0; i < _bubbleTimers.Length; i++)
        {
            _bubbleTimers[i] += deltaTime;
        }

        GenerateBubblesFromFish();
    }

    protected override void OnRenderFrame(FrameEventArgs args)
    {
        GL.Clear(ClearBufferMask.ColorBufferBit);
        DrawAquarium();
        DrawBubbles();
        SwapBuffers();
        base.OnRenderFrame(args);
    }

    protected override void OnUnload()
    {
        base.OnUnload();
        GL.ClearColor(Color4.White);
    }

    private void InitializeFishes()
    {
        _fishes.Add(new Fish(FishBasePositions[0], new Vector2(120.0f, 80.0f), _fishOffsets[0],
            new Fish1DrawingStrategy()));
        _fishes.Add(new Fish(FishBasePositions[1], new Vector2(150.0f, 40.0f), _fishOffsets[1],
            new Fish2DrawingStrategy()));
        _fishes.Add(new Fish(FishBasePositions[2], new Vector2(100.0f, 60.0f), _fishOffsets[2],
            new Fish3DrawingStrategy()));
        _fishes.Add(new Fish(FishBasePositions[3], new Vector2(120.0f, 80.0f), _fishOffsets[3],
            new Fish4DrawingStrategy()));
        _fishes.Add(new Fish(FishBasePositions[4], new Vector2(100.0f, 60.0f), _fishOffsets[4],
            new Fish5DrawingStrategy()));
    }

    private void InitializeStones()
    {
        _stones.Add(new Stone(new Vector2(370.0f, -600.0f), new Vector2(100.0f, 100.0f), new Stone2DrawingStrategy()));
        _stones.Add(new Stone(new Vector2(-240.0f, -600.0f), new Vector2(50.0f, 50.0f), new Stone4DrawingStrategy()));
        _stones.Add(new Stone(new Vector2(100.0f, -600.0f), new Vector2(60.0f, 30.0f), new Stone2DrawingStrategy()));
        _stones.Add(new Stone(new Vector2(600.0f, -600.0f), new Vector2(40.0f, 20.0f), new Stone2DrawingStrategy()));
        _stones.Add(new Stone(new Vector2(60.0f, -600.0f), new Vector2(60.0f, 40.0f), new Stone1DrawingStrategy()));
        _stones.Add(new Stone(new Vector2(-550.0f, -600.0f), new Vector2(50.0f, 50.0f), new Stone3DrawingStrategy()));
        _stones.Add(new Stone(new Vector2(200.0f, -600.0f), new Vector2(40.0f, 40.0f), new Stone4DrawingStrategy()));
        _stones.Add(new Stone(new Vector2(500.0f, -600.0f), new Vector2(70.0f, 50.0f), new Stone1DrawingStrategy()));
        _stones.Add(new Stone(new Vector2(440.0f, -600.0f), new Vector2(40.0f, 40.0f), new Stone5DrawingStrategy()));
        _stones.Add(new Stone(new Vector2(630.0f, -600.0f), new Vector2(40.0f, 40.0f), new Stone5DrawingStrategy()));
    }

    private void GenerateBubblesFromFish()
    {
        float[] bubbleIntervals = AquariumConfig.FishBubbleIntervals;

        for (int i = 0; i < AquariumConfig.NumBubblesArraySize; i++)
        {
            if (_bubbleTimers[i] > bubbleIntervals[i])
            {
                float fishX = FishBasePositions[i].X - _fishOffsets[i];
                float fishY = FishBasePositions[i].Y;
                GenerateFishBubbles(fishX, fishY);
                _bubbleTimers[i] = 0f;
            }
        }
    }

    private void GenerateFishBubbles(float fishX, float fishY)
    {
        for (int i = 0; i < AquariumConfig.NumBubblesPerFish; i++)
        {
            float offsetX = _random.Next(AquariumConfig.MinBubbleOffsetX, AquariumConfig.MaxBubbleOffsetX);
            float offsetY = _random.Next(AquariumConfig.MinBubbleOffsetY, AquariumConfig.MaxBubbleOffsetY);
            float bubbleX = fishX + offsetX;
            float bubbleY = fishY + offsetY;
            float size = _random.Next(AquariumConfig.MinBubbleSize, AquariumConfig.MaxBubbleSize);
            float speedY = _random.Next(AquariumConfig.MinBubbleSpeedY, AquariumConfig.MaxBubbleSpeedY);
            float speedX = _random.Next(AquariumConfig.MinBubbleSpeedX, AquariumConfig.MaxBubbleSpeedX);

            _bubbles.Add(new Bubble(
                new Vector2(bubbleX, bubbleY),
                new Vector2(speedX, speedY),
                new Vector2(0, 1),
                size
            ));
        }
    }

    private void UpdateFishPositions(float deltaTime)
    {
        float aspectRatio = (float)Size.X / Size.Y;
        float visibleHalfWidth = WindowWidth * aspectRatio;
        float leftBound = -visibleHalfWidth - AquariumConfig.FishLeftBoundOffset;
        float rightResetPos = visibleHalfWidth + AquariumConfig.FishRightResetPositionOffset;

        for (int i = 0; i < _fishOffsets.Length; i++)
        {
            _fishOffsets[i] += _fishSpeeds[i] * deltaTime * AquariumConfig.FishMovementSpeedMultiplier;

            if (FishBasePositions[i].X - _fishOffsets[i] < leftBound)
            {
                _fishOffsets[i] = FishBasePositions[i].X - rightResetPos;
            }

            _fishes[i].Offset = _fishOffsets[i];
        }
    }

    private void UpdateBubbles(float deltaTime)
    {
        for (int i = _bubbles.Count - 1; i >= 0; i--)
        {
            Bubble bubble = _bubbles[i];

            bubble.Position += bubble.Speed * bubble.Direction * deltaTime;
            bubble.LifeTime -= deltaTime;

            if (bubble.Position.Y > WindowHeight ||
                bubble.Position.X > WindowWidth ||
                bubble.Position.X < -WindowWidth ||
                bubble.LifeTime <= 0)
            {
                _bubbles.RemoveAt(i);
            }
        }
    }

    private void DrawBubbles()
    {
        foreach (Bubble bubble in _bubbles)
        {
            DrawBubble(bubble.Position.X, bubble.Position.Y, bubble.Size);
        }
    }

    private void DrawBubble(float x, float y, float size)
    {
        GL.PushMatrix();
        GL.Translate(x, y, 0);

        DrawingTools.DrawEllipse(0, 0, size, size, new Color4(1.0f, 1.0f, 1.0f, 0.4f));
        DrawingTools.DrawEllipse(0, 0, size, size, new Color4(1.0f, 1.0f, 1.0f, 0.8f), PrimitiveType.LineLoop);

        GL.Begin(PrimitiveType.Points);
        GL.Color3(1.0f, 1.0f, 1.0f);
        GL.Vertex2(size * BubbleHighlightMultiplier,
            size * BubbleHighlightMultiplier);
        GL.End();

        GL.PopMatrix();
    }

    private void AdjustProjection()
    {
        float aspectRatio = (float)Size.X / Size.Y;

        Matrix4 projection = Matrix4.CreateOrthographic(
            AquariumConfig.ProjectionWidth * aspectRatio,
            AquariumConfig.ProjectionHeight,
            AquariumConfig.ProjectionNearPlane,
            AquariumConfig.ProjectionFarPlane
        );

        GL.MatrixMode(MatrixMode.Projection);
        GL.LoadMatrix(ref projection);
    }

    private void DrawAquarium()
    {
        DrawPlants();
        DrawStones();
        DrawFish();
    }

    private void DrawPlants()
    {
        DrawPlant(-500.0f, -600.0f, 60.0f, 450.0f);
        DrawPlant(-100.0f, -600.0f, 50.0f, 250.0f);
        DrawPlant(100.0f, -600.0f, 60.0f, 300.0f);
        DrawPlant(300.0f, -600.0f, 20.0f, 150.0f);
        DrawPlant(500.0f, -600.0f, 60.0f, 550.0f);
        DrawPlant(370.0f, -600.0f, 100.0f, 750.0f);
        DrawPlant(-700.0f, -600.0f, 80.0f, 350.0f);
    }

    private void DrawStones()
    {
        foreach (Stone stone in _stones)
        {
            stone.Draw();
        }
    }

    private void DrawFish()
    {
        foreach (Fish fish in _fishes)
        {
            fish.Draw();
        }
    }

    private void DrawPlant(float x, float y, float width, float height)
    {
        GL.PushMatrix();
        GL.Translate(x, y, 0);

        GL.Begin(PrimitiveType.Quads);
        GL.Color3(0.0f, 0.5f, 0.0f);

        float segmentWidth = PlantSegmentWidth;

        for (float currentX = 0; currentX < width; currentX += segmentWidth)
        {
            GL.Vertex2(currentX, 0);
            GL.Vertex2(currentX + segmentWidth, 0);
            GL.Vertex2(currentX + segmentWidth, height);
            GL.Vertex2(currentX, height);
        }

        GL.End();

        GL.PopMatrix();
    }
}