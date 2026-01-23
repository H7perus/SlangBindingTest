using OpenGLTriangle;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace OpenGLTriangle
{
    public class Game : GameWindow
    {
        private int vao;
        private int vbo;
        private int ubo;  // Uniform Buffer Object for the matrix
        private int shaderProgram;
        private string vertexShaderSource;
        private string fragmentShaderSource;
        private float rotation = 0.0f;
        private float aspectRatio = 4/3;

        public Game(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings,
                    string vertexShader, string fragmentShader)
            : base(gameWindowSettings, nativeWindowSettings)
        {
            vertexShaderSource = vertexShader;
            fragmentShaderSource = fragmentShader;
        }

        protected override void OnLoad()
        {
            base.OnLoad();

            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);

            // Triangle vertices with colors (x, y, z, r, g, b)
            float[] vertices = {
                // positions        // colors
                -0.5f, -0.28867513459481287f, 0.0f,  1.0f, 0.0f, 0.0f,  // bottom left (red)
                 0.5f, -0.28867513459481287f, 0.0f,  0.0f, 1.0f, 0.0f,  // bottom right (green)
                 0.0f,  0.5773502691896257f, 0.0f,  0.0f, 0.0f, 1.0f   // top (blue)
            };

            Console.WriteLine("Vertex data being sent:");
            Console.WriteLine("Vertex 0: pos(-0.5, -0.5, 0.0), color(1.0, 0.0, 0.0) RED");
            Console.WriteLine("Vertex 1: pos( 0.5, -0.5, 0.0), color(0.0, 1.0, 0.0) GREEN");
            Console.WriteLine("Vertex 2: pos( 0.0,  0.5, 0.0), color(0.0, 0.0, 1.0) BLUE");
            Console.WriteLine("Position is at location 0, Color is at location 1");
            Console.WriteLine();

            // Create VAO
            vao = GL.GenVertexArray();
            GL.BindVertexArray(vao);

            // Create VBO
            vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

            // Position attribute
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            // Color attribute
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(1);

            // Compile shaders
            int vertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShader, vertexShaderSource);
            GL.CompileShader(vertexShader);

            GL.GetShader(vertexShader, ShaderParameter.CompileStatus, out int vSuccess);
            if (vSuccess == 0)
            {
                string infoLog = GL.GetShaderInfoLog(vertexShader);
                Console.WriteLine($"ERROR: Vertex shader compilation failed:\n{infoLog}");
            }
            else
            {
                Console.WriteLine("Vertex shader compiled successfully!");
            }

            int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShader, fragmentShaderSource);
            GL.CompileShader(fragmentShader);

            GL.GetShader(fragmentShader, ShaderParameter.CompileStatus, out int fSuccess);
            if (fSuccess == 0)
            {
                string infoLog = GL.GetShaderInfoLog(fragmentShader);
                Console.WriteLine($"ERROR: Fragment shader compilation failed:\n{infoLog}");
            }
            else
            {
                Console.WriteLine("Fragment shader compiled successfully!");
            }

            // Link shaders
            shaderProgram = GL.CreateProgram();
            GL.AttachShader(shaderProgram, vertexShader);
            GL.AttachShader(shaderProgram, fragmentShader);
            GL.LinkProgram(shaderProgram);

            GL.GetProgram(shaderProgram, GetProgramParameterName.LinkStatus, out int pSuccess);
            if (pSuccess == 0)
            {
                string infoLog = GL.GetProgramInfoLog(shaderProgram);
                Console.WriteLine($"ERROR: Shader program linking failed:\n{infoLog}");
            }
            else
            {
                Console.WriteLine("Shader program linked successfully!");
            }

            GL.DeleteShader(vertexShader);
            GL.DeleteShader(fragmentShader);

            // Create Uniform Buffer Object (UBO) at binding point 0
            ubo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.UniformBuffer, ubo);
            // Allocate space for a 4x4 matrix (16 floats = 64 bytes)
            GL.BufferData(BufferTarget.UniformBuffer, 64, IntPtr.Zero, BufferUsageHint.DynamicDraw);
            // Bind to binding point 0
            GL.BindBufferBase(BufferRangeTarget.UniformBuffer, 0, ubo);

            Console.WriteLine("Uniform Buffer Object created at binding = 0");
            Console.WriteLine("Buffer holds a 4x4 rotation matrix (64 bytes)");
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);

            // Step 1: Clear the screen with the background color
            GL.Clear(ClearBufferMask.ColorBufferBit);

            // Step 2: Activate our shader program
            GL.UseProgram(shaderProgram);

            // Step 3: Bind the VAO containing our triangle data
            GL.BindVertexArray(vao);

            // Step 4: DRAW THE TRIANGLE!
            // - PrimitiveType.Triangles: interpret vertices as triangles
            // - 0: start at the first vertex
            // - 3: draw 3 vertices (1 triangle)
            GL.DrawArrays(PrimitiveType.Triangles, 0, 3);

            // Step 5: Swap buffers to display the rendered frame
            SwapBuffers();
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);

            if (KeyboardState.IsKeyDown(Keys.Escape))
            {
                Close();
            }

            // Update rotation angle
            rotation += (float)args.Time * 1.0f; // Rotate at 1 radian per second

            // Create rotation matrix around Z axis
            float cos = MathF.Cos(rotation);
            float sin = MathF.Sin(rotation);

            // Aspect ratio correction for 4:3

            // 4x4 rotation matrix (ROW-MAJOR order) with aspect ratio correction
            // Aspect ratio is applied by scaling X coordinates
            float[] rotationMatrix = new float[]
            {
                cos / aspectRatio, -sin / aspectRatio, 0.0f, 0.0f,  // row 0 (scaled by aspect)
                sin,                cos,               0.0f, 0.0f,  // row 1
                0.0f,               0.0f,              1.0f, 0.0f,  // row 2
                0.0f,               0.0f,              0.0f, 1.0f   // row 3
            };

            // Update the UBO with the new rotation matrix
            GL.BindBuffer(BufferTarget.UniformBuffer, ubo);
            GL.BufferSubData(BufferTarget.UniformBuffer, IntPtr.Zero, 64, rotationMatrix);
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(0, 0, e.Width, e.Height);
            aspectRatio = (float)e.Width / e.Height;
        }

        protected override void OnUnload()
        {
            base.OnUnload();

            GL.DeleteBuffer(vbo);
            GL.DeleteBuffer(ubo);
            GL.DeleteVertexArray(vao);
            GL.DeleteProgram(shaderProgram);
        }
    }
}

// CALL THIS FUNCTION FROM YOUR EXISTING Program.cs
public static class TriangleRenderer
{
    public static void OpenWindow(string vertexShaderSource, string fragmentShaderSource)
    {
        var gameWindowSettings = GameWindowSettings.Default;
        var nativeWindowSettings = new NativeWindowSettings()
        {
            ClientSize = new Vector2i(800, 600),
            Title = "OpenGL Triangle",
            Flags = ContextFlags.ForwardCompatible,
        };

        using (var game = new Game(gameWindowSettings, nativeWindowSettings, vertexShaderSource, fragmentShaderSource))
        {
            game.Run(); // Opens window and renders triangle
        }
    }
}