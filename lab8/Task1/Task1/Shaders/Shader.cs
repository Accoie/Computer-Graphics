using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Task1.Shaders;

public class Shader
{
    private readonly int _handle;

    public Shader(
        string vertexPath = "Shaders/shader.vert",
        string fragmentPath = "Shaders/shader.frag"
    )
    {
        try
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            
            string vertexFullPath = Path.Combine(baseDir, vertexPath);
            string fragmentFullPath = Path.Combine(baseDir, fragmentPath);
            
            string vertexSource = File.ReadAllText(vertexFullPath);
            string fragmentSource = File.ReadAllText(fragmentFullPath);
            
            int vertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShader, vertexSource);
            GL.CompileShader(vertexShader);
            CheckShader(vertexShader, "Vertex");
            
            int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShader, fragmentSource);
            GL.CompileShader(fragmentShader);
            CheckShader(fragmentShader, "Fragment");
            
            _handle = GL.CreateProgram();
            GL.AttachShader(_handle, vertexShader);
            GL.AttachShader(_handle, fragmentShader);
            GL.LinkProgram(_handle);
            
            GL.GetProgram(_handle, GetProgramParameterName.LinkStatus, out int success);
            if (success == 0)
            {
                string info = GL.GetProgramInfoLog(_handle);
                throw new Exception($"Shader linking failed:\n{info}");
            }
            
            GL.DetachShader(_handle, vertexShader);
            GL.DetachShader(_handle, fragmentShader);
            GL.DeleteShader(vertexShader);
            GL.DeleteShader(fragmentShader);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Shader loading error: {ex.Message}");
            throw;
        }
    }

    public void Use() => GL.UseProgram(_handle);

    public void SetMatrix4(string name, Matrix4 matrix)
    {
        int loc = GetUniformLocation(name);
        if (loc != -1) GL.UniformMatrix4(loc, false, ref matrix);
    }

    public void SetVector3(string name, Vector3 vector)
    {
        int loc = GetUniformLocation(name);
        if (loc != -1) GL.Uniform3(loc, vector);
    }

    public void SetFloat(string name, float value)
    {
        int loc = GetUniformLocation(name);
        if (loc != -1) GL.Uniform1(loc, value);
    }
    
    private void CheckShader(int shader, string type)
    {
        GL.GetShader(shader, ShaderParameter.CompileStatus, out int success);
        if (success == 0)
        {
            string info = GL.GetShaderInfoLog(shader);

            throw new Exception($"{type} shader compilation failed:\n{info}");
        }

        Console.WriteLine($"{type} shader compiled successfully!");
    }
    
    private int GetUniformLocation(string name) => GL.GetUniformLocation(_handle, name);
}
