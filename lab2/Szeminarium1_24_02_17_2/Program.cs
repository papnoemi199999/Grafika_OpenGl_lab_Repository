using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenAL;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;

namespace Szeminarium1_24_02_17_2
{
    internal static class Program
    {
        private static List<GlCube> rubiksCube = new List<GlCube>();

        private static CameraDescriptor cameraDescriptor = new();

        private static CubeArrangementModel cubeArrangementModel = new();

        private static IWindow window;

        private static GL Gl;

        private static uint program;

        private const string ModelMatrixVariableName = "uModel";
        private const string ViewMatrixVariableName = "uView";
        private const string ProjectionMatrixVariableName = "uProjection";

        private static double Xrot = 0;
        private static double Yrot = 0;

        private static double Speedrot = 0.1;

        private static readonly string VertexShaderSource = @"
        #version 330 core
        layout (location = 0) in vec3 vPos;
		layout (location = 1) in vec4 vCol;

        uniform mat4 uModel;
        uniform mat4 uView;
        uniform mat4 uProjection;

		out vec4 outCol;
        
        void main()
        {
			outCol = vCol;
            gl_Position = uProjection*uView*uModel*vec4(vPos.x, vPos.y, vPos.z, 1.0);
        }
        ";


        private static readonly string FragmentShaderSource = @"
        #version 330 core
        out vec4 FragColor;

		in vec4 outCol;

        void main()
        {
            FragColor = outCol;
        }
        ";

        static void Main(string[] args)
        {
            WindowOptions windowOptions = WindowOptions.Default;
            windowOptions.Title = "2 szeminárium";
            windowOptions.Size = new Vector2D<int>(500, 500);

            // on some systems there is no depth buffer by default, so we need to make sure one is created
            windowOptions.PreferredDepthBufferBits = 24;

            window = Window.Create(windowOptions);

            window.Load += Window_Load;
            window.Update += Window_Update;
            window.Render += Window_Render;
            window.Closing += Window_Closing;

            window.Run();
        }


        private static void Window_Load()
        {
            // interactive camera 
            Console.WriteLine("Load");

            //set up input handling

            IInputContext inputContext = window.CreateInput();
            foreach (var keyboard in inputContext.Keyboards)
            {
                keyboard.KeyDown += Keyboard_KeyDown;
            }
            //

            Gl = window.CreateOpenGL();
            Gl.ClearColor(1.0f, 0.85f, 0.85f, 1.0f);

            SetUpObjects();

            LinkProgram();

            Gl.Enable(EnableCap.CullFace);

            Gl.Enable(EnableCap.DepthTest);
            Gl.DepthFunc(DepthFunction.Lequal);

        }

        private static void LinkProgram()
        {
            uint vshader = Gl.CreateShader(ShaderType.VertexShader);
            uint fshader = Gl.CreateShader(ShaderType.FragmentShader);

            Gl.ShaderSource(vshader, VertexShaderSource);
            Gl.CompileShader(vshader);
            Gl.GetShader(vshader, ShaderParameterName.CompileStatus, out int vStatus);
            if (vStatus != (int)GLEnum.True)
                throw new Exception("Vertex shader failed to compile: " + Gl.GetShaderInfoLog(vshader));

            Gl.ShaderSource(fshader, FragmentShaderSource);
            Gl.CompileShader(fshader);

            program = Gl.CreateProgram();
            Gl.AttachShader(program, vshader);
            Gl.AttachShader(program, fshader);
            Gl.LinkProgram(program);
            Gl.GetProgram(program, GLEnum.LinkStatus, out var status);
            if (status == 0)
            {
                Console.WriteLine($"Error linking shader {Gl.GetProgramInfoLog(program)}");
            }
            Gl.DetachShader(program, vshader);
            Gl.DetachShader(program, fshader);
            Gl.DeleteShader(vshader);
            Gl.DeleteShader(fshader);
        }

        private static void Keyboard_KeyDown(IKeyboard keyboard, Key key, int arg3)
        {
            switch (key)
            {
                case Key.Left:
                    cameraDescriptor.DecreaseZYAngle();
                    break;
                    ;
                case Key.Right:
                    cameraDescriptor.IncreaseZYAngle();
                    break;
                case Key.Down:
                    cameraDescriptor.IncreaseDistance();
                    break;
                case Key.Up:
                    cameraDescriptor.DecreaseDistance();
                    break;
                case Key.U:
                    cameraDescriptor.IncreaseZXAngle();
                    break;
                case Key.D:
                    cameraDescriptor.DecreaseZXAngle();
                    break;

                case Key.Space:
                    RightRotation();
                    break;
                case Key.Backspace:
                    LeftRotation();
                    break;
            }
        }

        private static void RightRotation()
        {
            Xrot += Math.PI / 2;
        }
        private static void LeftRotation()
        {
            Xrot -= Math.PI / 2;
        }
        private static void Window_Update(double deltaTime)
        {
            if (Xrot < 0)
            {
                Xrot += Speedrot;
                Yrot -= Speedrot;

            }
            if (Xrot > 0)
            {
                Xrot -= Speedrot;
                Yrot += Speedrot;
            }

            cubeArrangementModel.AdvanceTime(deltaTime);
        }

        private static unsafe void Window_Render(double deltaTime)
        {
            //Console.WriteLine($"Render after {deltaTime} [s].");

            // GL here
            Gl.Clear(ClearBufferMask.ColorBufferBit);
            Gl.Clear(ClearBufferMask.DepthBufferBit);


            Gl.UseProgram(program);

            SetViewMatrix();
            SetProjectionMatrix();

            DrawRubicCube();

        }
        private static unsafe void DrawRubicCube()
        {
            float cubeScale = (float)cubeArrangementModel.CenterCubeScale;
            float spacing = 0.1f;
            float scale = cubeScale + spacing;

          

            var scaleMatrixForCube = Matrix4X4.CreateScale((float)cubeArrangementModel.CenterCubeScale);
            foreach (var cube in rubiksCube)
            {
                Matrix4X4<float> originalModelMatrix = scaleMatrixForCube * Matrix4X4.CreateTranslation(cube.TranslationX * scale, cube.TranslationY * scale, cube.TranslationZ * scale);
                Matrix4X4<float> rotY = Matrix4X4<float>.Identity;
                if (Xrot != 0)
                {
                    if (cube.PosY == 1)
                    {
                        rotY = Matrix4X4.CreateRotationY((float)(Yrot));
                    }
                }
             
                Matrix4X4<float> modelMatrix = originalModelMatrix * rotY;
                SetModelMatrix(modelMatrix);

                Gl.BindVertexArray(cube.Vao);
                Gl.DrawElements(GLEnum.Triangles, cube.IndexArrayLength, GLEnum.UnsignedInt, null);
                Gl.BindVertexArray(0);
            }
        }

        private static unsafe void SetModelMatrix(Matrix4X4<float> modelMatrix)
        {
            int location = Gl.GetUniformLocation(program, ModelMatrixVariableName);
            if (location == -1)
            {
                throw new Exception($"{ModelMatrixVariableName} uniform not found on shader.");
            }

            Gl.UniformMatrix4(location, 1, false, (float*)&modelMatrix);
            CheckError();
        }

        private static unsafe void SetUpObjects()
        {

            float[] red = new float[] { 1.0f, 0.0f, 0.0f, 1.0f };
            float[] green = new float[] { 0.0f, 1.0f, 0.0f, 1.0f };
            float[] blue = new float[] { 0.0f, 0.0f, 1.0f, 1.0f };
            float[] white = new float[] { 1.0f, 1.0f, 1.0f, 1.0f };
            float[] orange = new float[] { 1.0f, 0.5f, 0.0f, 1.0f };
            float[] yellow = new float[] { 1.0f, 1.0f, 0.0f, 1.0f };

            float[] black = new float[] { 0.0f, 0.0f, 0.0f, 1.0f };

            // Y = 1
            rubiksCube.Add(GlCube.CreateCubeWithFaceColors(Gl, red, black, black, black, white, black, 0f, 1f, -1f));
            rubiksCube.Add(GlCube.CreateCubeWithFaceColors(Gl, red, black, black, black, white, blue, 1f, 1f, -1f));
            rubiksCube.Add(GlCube.CreateCubeWithFaceColors(Gl, red, black, green, black, white, black, -1f, 1f, -1f));

            rubiksCube.Add(GlCube.CreateCubeWithFaceColors(Gl, red, black, black, black, black, black, 0f, 1f, 0f));
            rubiksCube.Add(GlCube.CreateCubeWithFaceColors(Gl, red, black, black, black, black, blue, 1f, 1f, 0f));
            rubiksCube.Add(GlCube.CreateCubeWithFaceColors(Gl, red, black, green, black, black, black, -1f, 1f, 0f));

            rubiksCube.Add(GlCube.CreateCubeWithFaceColors(Gl, red, yellow, black, black, black, black, 0f, 1f, 1f));
            rubiksCube.Add(GlCube.CreateCubeWithFaceColors(Gl, red, yellow, black, black, black, blue, 1f, 1f, 1f));
            rubiksCube.Add(GlCube.CreateCubeWithFaceColors(Gl, red, yellow, green, black, black, black, -1f, 1f, 1f));


            // Y = 0
            rubiksCube.Add(GlCube.CreateCubeWithFaceColors(Gl, black, black, black, black, white, black, 0f, 0f, -1f));
            rubiksCube.Add(GlCube.CreateCubeWithFaceColors(Gl, black, black, black, black, white, blue, 1f, 0f, -1f));
            rubiksCube.Add(GlCube.CreateCubeWithFaceColors(Gl, black, black, green, black, white, black, -1f, 0f, -1f));

            rubiksCube.Add(GlCube.CreateCubeWithFaceColors(Gl, black, black, black, black, black, black, 0f, 0f, 0f));
            rubiksCube.Add(GlCube.CreateCubeWithFaceColors(Gl, black, black, black, black, black, blue, 1f, 0f, 0f));
            rubiksCube.Add(GlCube.CreateCubeWithFaceColors(Gl, black, black, green, black, black, black, -1f, 0f, 0f));

            rubiksCube.Add(GlCube.CreateCubeWithFaceColors(Gl, black, yellow, black, black, black, black, 0f, 0f, 1f));
            rubiksCube.Add(GlCube.CreateCubeWithFaceColors(Gl, black, yellow, black, black, black, blue, 1f, 0f, 1f));
            rubiksCube.Add(GlCube.CreateCubeWithFaceColors(Gl, black, yellow, green, black, black, black, -1f, 0f, 1f));


            // Y = -1

            // Y = -1
            rubiksCube.Add(GlCube.CreateCubeWithFaceColors(Gl, black, black, black, orange, white, black, 0f, -1f, -1f));
            rubiksCube.Add(GlCube.CreateCubeWithFaceColors(Gl, black, black, black, orange, white, blue, 1f, -1f, -1f));
            rubiksCube.Add(GlCube.CreateCubeWithFaceColors(Gl, black, black, green, orange, white, black, -1f, -1f, -1f));

            rubiksCube.Add(GlCube.CreateCubeWithFaceColors(Gl, black, black, black, orange, black, black, 0f, -1f, 0f));
            rubiksCube.Add(GlCube.CreateCubeWithFaceColors(Gl, black, black, black, orange, black, blue, 1f, -1f, 0f));
            rubiksCube.Add(GlCube.CreateCubeWithFaceColors(Gl, black, black, green, orange, black, black, -1f, -1f, 0f));

            rubiksCube.Add(GlCube.CreateCubeWithFaceColors(Gl, black, yellow, black, orange, black, black, 0f, -1f, 1f));
            rubiksCube.Add(GlCube.CreateCubeWithFaceColors(Gl, black, yellow, black, orange, black, blue, 1f, -1f, 1f));
            rubiksCube.Add(GlCube.CreateCubeWithFaceColors(Gl, black, yellow, green, orange, black, black, -1f, -1f, 1f));

        }
        //probald megcserelni a sorrendet a materix szorzasnal!!!!!!!!!!!!!!!!!!!!!!!
        private static void Window_Closing()
        {
            foreach (var cube in rubiksCube)
            {
                cube.ReleaseGlCube();
            }
        }

        private static unsafe void SetProjectionMatrix()
        {
            var projectionMatrix = Matrix4X4.CreatePerspectiveFieldOfView<float>((float)Math.PI / 4f, 1024f / 768f, 0.1f, 100);
            int location = Gl.GetUniformLocation(program, ProjectionMatrixVariableName);

            if (location == -1)
            {
                throw new Exception($"{ViewMatrixVariableName} uniform not found on shader.");
            }

            Gl.UniformMatrix4(location, 1, false, (float*)&projectionMatrix);
            CheckError();
        }

        private static unsafe void SetViewMatrix()
        {

            //// fixed camera
            //var fixedPosition = new Vector3D<float>(5.0f, 5.0f, 5.0f); // camera position
            //var fixedTarget = new Vector3D<float>(0.0f, 0.0f, 0.0f); // cube position
            //var fixedUpVector = new Vector3D<float>(0.0f, 1.0f, 0.0f); h// camera position - upwards

            //var viewMatrix = Matrix4X4.CreateLookAt(fixedPosition, fixedTarget, fixedUpVector);
            //int location = Gl.GetUniformLocation(program, ViewMatrixVariableName);

            // interactive camera
            var viewMatrix = Matrix4X4.CreateLookAt(cameraDescriptor.Position, cameraDescriptor.Target, cameraDescriptor.UpVector);
            int location = Gl.GetUniformLocation(program, ViewMatrixVariableName);
            //
            if (location == -1)
            {
                throw new Exception($"{ViewMatrixVariableName} uniform not found on shader.");
            }

            Gl.UniformMatrix4(location, 1, false, (float*)&viewMatrix);
            CheckError();
        }

        public static void CheckError()
        {
            var error = (ErrorCode)Gl.GetError();
            if (error != ErrorCode.NoError)
                throw new Exception("GL.GetError() returned " + error.ToString());
        }
    }
}