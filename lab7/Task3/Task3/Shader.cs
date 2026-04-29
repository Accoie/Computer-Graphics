using OpenTK.Graphics.OpenGL4;

namespace Task3
{
    public class Shader
    {
        public int Handle { get; }

        public Shader(string vertexShaderPath, string fragmentShaderPath)
        {
            string vertexShaderSource = File.ReadAllText(vertexShaderPath);
            string fragmentShaderSource = File.ReadAllText(fragmentShaderPath);

            int vertexShader = CompileShader(ShaderType.VertexShader, vertexShaderSource);
            int fragmentShader = CompileShader(ShaderType.FragmentShader, fragmentShaderSource);

            Handle = GL.CreateProgram();
            GL.AttachShader(Handle, vertexShader);
            GL.AttachShader(Handle, fragmentShader);
            GL.LinkProgram(Handle);

            GL.GetProgram(Handle, GetProgramParameterName.LinkStatus, out int linkStatus);
            if (linkStatus == 0)
            {
                Console.WriteLine($"ERROR: Shader link failed: {GL.GetProgramInfoLog(Handle)}");
            }

            GL.DetachShader(Handle, vertexShader);
            GL.DetachShader(Handle, fragmentShader);
            GL.DeleteShader(vertexShader);
            GL.DeleteShader(fragmentShader);
        }

        public void Use()
        {
            GL.UseProgram(Handle);
        }

        public int GetAttribLocation(string attribName)
        {
            return GL.GetAttribLocation(Handle, attribName);
        }

        public int GetUniformLocation(string uniformName)
        {
            return GL.GetUniformLocation(Handle, uniformName);
        }

        public void SetUniform(string name, float value)
        {
            int location = GetUniformLocation(name);
            if (location != -1)
            {
                GL.Uniform1(location, value);
            }
        }

        public void SetUniform(string name, OpenTK.Mathematics.Vector3 value)
        {
            int location = GetUniformLocation(name);
            if (location != -1)
            {
                GL.Uniform3(location, ref value);
            }
        }

        public void SetUniform(string name, OpenTK.Mathematics.Matrix4 value)
        {
            int location = GetUniformLocation(name);
            if (location != -1)
            {
                GL.UniformMatrix4(location, false, ref value);
            }
        }

        private int CompileShader(ShaderType type, string source)
        {
            int shader = GL.CreateShader(type);
            GL.ShaderSource(shader, source);
            GL.CompileShader(shader);
            GL.GetShader(shader, ShaderParameter.CompileStatus, out int _);
            
            return shader;
        }

        public void Dispose()
        {
            GL.DeleteProgram(Handle);
        }
    }
}
