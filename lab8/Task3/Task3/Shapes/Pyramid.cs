using OpenTK.Mathematics;
using Task3.Shaders;

namespace Task3.Shapes;

public class Pyramid
{
    readonly List<Torus> _toruses =
    [
        new(new TorusData(new Vector3(0, -0.8f, 0), 0.9f, 0.3f), new Vector3(1, 0, 0)),
        new(new TorusData(new Vector3(0, -0.3f, 0), 0.7f, 0.25f), new Vector3(0, 1, 0)),
        new(new TorusData(new Vector3(0, 0.1f, 0), 0.5f, 0.2f), new Vector3(0, 0, 1)),
        new(new TorusData(new Vector3(0, 0.4f, 0), 0.3f, 0.15f), new Vector3(0.95f, 0.08f, 0.73f)),
    ];

    public IReadOnlyList<Torus> Toruses => _toruses;

    public void Paint(Shader shader)
    {
        foreach (Torus torus in _toruses)
        {
            torus.Paint(shader);
        }
    }

    public void CreateBuffers()
    {
        foreach (Torus torus in _toruses)
        {
            torus.CreateBuffers();
        }
    }
}

