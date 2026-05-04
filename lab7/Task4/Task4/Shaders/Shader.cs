using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace Task4.Shaders;

public sealed class Shader
{
    private readonly int _handle;

    public Shader(
        string vertexPath = "../../../Shaders/shader.vert",
        string fragmentPath = "../../../Shaders/shader.frag"
        )
    {
        string vertexShaderSource = File.ReadAllText(vertexPath);
        string fragmentShaderSource = File.ReadAllText(fragmentPath);

        int vertexShader = CompileShader(vertexShaderSource, ShaderType.VertexShader);
        int fragmentShader = CompileShader(fragmentShaderSource, ShaderType.FragmentShader);

        _handle = LinkProgram(vertexShader, fragmentShader);
    }

    private int CompileShader(string shaderSource, ShaderType shaderType)
    {
        int shader = GL.CreateShader(shaderType);
        GL.ShaderSource(shader, shaderSource);

        GL.CompileShader(shader);
        GL.GetShader(shader, ShaderParameter.CompileStatus, out int success);
        if (success == 0)
        {
            string infoLog = GL.GetShaderInfoLog(shader);
            throw new ArgumentException($"Shader compilation failed: {infoLog}");
        }

        return shader;
    }

    private int LinkProgram(int vertexShader, int fragmentShader)
    {
        int program = GL.CreateProgram();
        GL.AttachShader(program, vertexShader);
        GL.AttachShader(program, fragmentShader);

        GL.LinkProgram(program);
        GL.GetProgram(program, GetProgramParameterName.LinkStatus, out int success);
        if (success == 0)
        {
            string infoLog = GL.GetProgramInfoLog(program);
            throw new ArgumentException($"Program linking failed: {infoLog}");
        }

        GL.DetachShader(program, vertexShader);
        GL.DetachShader(program, fragmentShader);
        GL.DeleteShader(vertexShader);
        GL.DeleteShader(fragmentShader);

        return program;
    }

    public void Use()
    {
        GL.UseProgram(_handle);
    }
    public void SetMatrix4(string name, Matrix4 matrix)
    {
        int location = GetUniformLocation(name);
        GL.UniformMatrix4(location, false, ref matrix);
    }

    public void SetVector2(string name, Vector2 vector)
    {
        int location = GetUniformLocation(name);
        GL.Uniform2(location, vector);
    }

    public void SetInt(string name, int value)
    {
        int location = GetUniformLocation(name);
        GL.Uniform1(location, value);
    }

    public void SetFloat(string name, float value)
    {
        int location = GetUniformLocation(name);
        GL.Uniform1(location, value);
    }

    private int GetUniformLocation(string name)
    {
        return GL.GetUniformLocation(_handle, name);
    }
}
