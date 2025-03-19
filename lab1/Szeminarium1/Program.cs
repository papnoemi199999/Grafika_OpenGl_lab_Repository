﻿using Silk.NET.OpenGL;
using Silk.NET.Windowing;

namespace Szeminarium1
{
    internal static class Program
    {
        private static IWindow graphicWindow;

        private static GL Gl;

        private static uint program;

        private static readonly string VertexShaderSource = @"
        #version 330 core
        layout (location = 0) in vec3 vPos;
		layout (location = 1) in vec4 vCol;

		out vec4 outCol;
        
        void main()
        {
			outCol = vCol;
            gl_Position = vec4(vPos.x, vPos.y, vPos.z, 1.0);
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
            windowOptions.Title = "1. szeminárium - háromszög";
            windowOptions.Size = new Silk.NET.Maths.Vector2D<int>(500, 500);

            graphicWindow = Window.Create(windowOptions);

            graphicWindow.Load += GraphicWindow_Load;
            graphicWindow.Update += GraphicWindow_Update;
            graphicWindow.Render += GraphicWindow_Render;

            graphicWindow.Run();
        }

        private static void GraphicWindow_Load()
        {
            // egszeri beallitasokat
            //Console.WriteLine("Loaded");

            Gl = graphicWindow.CreateOpenGL();

            Gl.ClearColor(System.Drawing.Color.White);

            uint vshader = Gl.CreateShader(ShaderType.VertexShader);
            uint fshader = Gl.CreateShader(ShaderType.FragmentShader);

            Gl.ShaderSource(vshader, VertexShaderSource);
            Gl.CompileShader(vshader);
            Gl.GetShader(vshader, ShaderParameterName.CompileStatus, out int vStatus);
            if (vStatus != (int)GLEnum.True)
                throw new Exception("Vertex shader failed to compile: " + Gl.GetShaderInfoLog(vshader));

            Gl.ShaderSource(fshader, FragmentShaderSource);
            Gl.CompileShader(fshader);
            Gl.GetShader(fshader, ShaderParameterName.CompileStatus, out int fStatus);
            if (fStatus != (int)GLEnum.True)
                throw new Exception("Vertex shader failed to compile: " + Gl.GetShaderInfoLog(fshader));

            program = Gl.CreateProgram();
            Gl.AttachShader(program, vshader);
            Gl.AttachShader(program, fshader);
            CheckError();
            Gl.LinkProgram(program);
            Gl.DetachShader(program, vshader);
            Gl.DetachShader(program, fshader);
            Gl.DeleteShader(vshader);
            Gl.DeleteShader(fshader);

            Gl.GetProgram(program, GLEnum.LinkStatus, out var status);
            if (status == 0)
            {
                Console.WriteLine($"Error linking shader {Gl.GetProgramInfoLog(program)}");
            }

        }

        private static void GraphicWindow_Update(double deltaTime)
        {
            // NO GL
            // make it threadsave
            //Console.WriteLine($"Update after {deltaTime} [s]");
        }

        private static unsafe void GraphicWindow_Render(double deltaTime)
        {
            //Console.WriteLine($"Render after {deltaTime} [s]");

            Gl.Clear(ClearBufferMask.ColorBufferBit);

            uint vao = Gl.GenVertexArray();
            Gl.BindVertexArray(vao);
            CheckError();

            float[] vertexArray = new float[] {
                   //TOP
                   0.2833f,  0.1667f,  0.0000f,
                   0.0000f,  0.3333f,  0.0000f,
                  -0.2833f,  0.1667f,  0.0000f,
                   0.0000f,  0.0000f,  0.0000f,

                   0.0000f,  0.3333f,  0.0000f,
                  -0.2833f,  0.5000f,  0.0000f,
                  -0.5666f,  0.3333f,  0.0000f,
                  -0.2833f,  0.1667f,  0.0000f,

                  -0.2833f,  0.5000f,  0.0000f,
                  -0.5666f,  0.6666f,  0.0000f,
                  -0.8500f,  0.5000f,  0.0000f,
                  -0.5666f,  0.3333f,  0.0000f,

                   0.5666f,  0.3333f,  0.0000f,
                   0.2833f,  0.5000f,  0.0000f,
                   0.0000f,  0.3333f,  0.0000f,
                   0.2833f,  0.1667f,  0.0000f,

                   0.2833f,  0.5000f,  0.0000f,
                   0.0000f,  0.6666f,  0.0000f,
                  -0.2833f,  0.5000f,  0.0000f,
                   0.0000f,  0.3333f,  0.0000f,

                   0.0000f,  0.6666f,  0.0000f,
                  -0.2833f,  0.8333f,  0.0000f,
                  -0.5666f,  0.6666f,  0.0000f,
                  -0.2833f,  0.5000f,  0.0000f,

                   0.8500f,  0.5000f,  0.0000f,
                   0.5666f,  0.6666f,  0.0000f,
                   0.2833f,  0.5000f,  0.0000f,
                   0.5666f,  0.3333f,  0.0000f,

                   0.5666f,  0.6666f,  0.0000f,
                   0.2833f,  0.8333f,  0.0000f,
                   0.0000f,  0.6666f,  0.0000f,
                   0.2833f,  0.5000f,  0.0000f,

                   0.2833f,  0.8333f,  0.0000f,
                   0.0000f,  1.0000f,  0.0000f,
                  -0.2833f,  0.8333f,  0.0000f,
                   0.0000f,  0.6666f,  0.0000f,

                   //LEFT
                  -0.5666f,  0.3333f,  0.0000f,
                  -0.8500f,  0.5000f,  0.0000f,
                  -0.8500f,  0.1667f,  0.0000f,
                  -0.5666f,  0.0000f,  0.0000f,

                  -0.5666f,  0.0000f,  0.0000f,
                  -0.8500f,  0.1667f,  0.0000f,
                  -0.8500f, -0.1667f,  0.0000f,
                  -0.5666f, -0.3333f,  0.0000f,

                  -0.5666f, -0.3333f,  0.0000f,
                  -0.8500f, -0.1667f,  0.0000f,
                  -0.8500f, -0.5000f,  0.0000f,
                  -0.5666f, -0.6666f,  0.0000f,

                  -0.2833f,  0.1667f,  0.0000f,
                  -0.5666f,  0.3333f,  0.0000f,
                  -0.5666f,  0.0000f,  0.0000f,
                  -0.2833f, -0.1667f,  0.0000f,

                  -0.2833f, -0.1667f,  0.0000f,
                  -0.5666f,  0.0000f,  0.0000f,
                  -0.5666f, -0.3333f,  0.0000f,
                  -0.2833f, -0.5000f,  0.0000f,

                  -0.2833f, -0.5000f,  0.0000f,
                  -0.5666f, -0.3333f,  0.0000f,
                  -0.5666f, -0.6666f,  0.0000f,
                  -0.2833f, -0.8333f,  0.0000f,

                  0.0000f,   0.0000f,  0.0000f,
                  -0.2833f,  0.1667f,  0.0000f,
                  -0.2833f, -0.1667f,  0.0000f,
                  0.0000f,  -0.3333f,  0.0000f,

                  0.0000f,  -0.3333f,  0.0000f,
                  -0.2833f, -0.1667f,  0.0000f,
                  -0.2833f, -0.5000f,  0.0000f,
                  0.0000f,  -0.6666f,  0.0000f,

                  0.0000f,  -0.6666f,  0.0000f,
                  -0.2833f, -0.5000f,  0.0000f,
                  -0.2833f, -0.8333f,  0.0000f,
                  0.0000f,  -0.9999f,  0.0000f,


                //RIGHT
                 0.5666f,  0.3333f,  0.0000f,
                 0.8500f,  0.5000f,  0.0000f,
                 0.8500f,  0.1667f,  0.0000f,
                 0.5666f,  0.0000f,  0.0000f,

                 0.5666f,  0.0000f,  0.0000f,
                 0.8500f,  0.1667f,  0.0000f,
                 0.8500f, -0.1667f,  0.0000f,
                 0.5666f, -0.3333f,  0.0000f,

                 0.5666f, -0.3333f,  0.0000f,
                 0.8500f, -0.1667f,  0.0000f,
                 0.8500f, -0.5000f,  0.0000f,
                 0.5666f, -0.6666f,  0.0000f,

                 0.2833f,  0.1667f,  0.0000f,
                 0.5666f,  0.3333f,  0.0000f,
                 0.5666f,  0.0000f,  0.0000f,
                 0.2833f, -0.1667f,  0.0000f,

                 0.2833f, -0.1667f,  0.0000f,
                 0.5666f,  0.0000f,  0.0000f,
                 0.5666f, -0.3333f,  0.0000f,
                 0.2833f, -0.5000f,  0.0000f,

                 0.2833f, -0.5000f,  0.0000f,
                 0.5666f, -0.3333f,  0.0000f,
                 0.5666f, -0.6666f,  0.0000f,
                 0.2833f, -0.8333f,  0.0000f,

                 0.0000f,   0.0000f,  0.0000f,
                 0.2833f,  0.1667f,  0.0000f,
                 0.2833f, -0.1667f,  0.0000f,
                 0.0000f,  -0.3333f,  0.0000f,

                 0.0000f,  -0.3333f,  0.0000f,
                 0.2833f, -0.1667f,  0.0000f,
                 0.2833f, -0.5000f,  0.0000f,
                 0.0000f,  -0.6666f,  0.0000f,

                 0.0000f,  -0.6666f,  0.0000f,
                 0.2833f, -0.5000f,  0.0000f,
                 0.2833f, -0.8333f,  0.0000f,
                 0.0000f,  -0.9999f,  0.0000f,


            };

            float[] colorArray = new float[] {
             
                1.0f, 0.0f, 0.0f, 1.0f,
                1.0f, 0.0f, 0.0f, 1.0f,
                1.0f, 0.0f, 0.0f, 1.0f,
                1.0f, 0.0f, 0.0f, 1.0f,

                0.9f, 0.0f, 0.0f, 1.0f,
                0.9f, 0.0f, 0.0f, 1.0f,
                0.9f, 0.0f, 0.0f, 1.0f,
                0.9f, 0.0f, 0.0f, 1.0f,

                0.8f, 0.0f, 0.0f, 1.0f,
                0.8f, 0.0f, 0.0f, 1.0f,
                0.8f, 0.0f, 0.0f, 1.0f,
                0.8f, 0.0f, 0.0f, 1.0f,

                0.7f, 0.0f, 0.0f, 1.0f,
                0.7f, 0.0f, 0.0f, 1.0f,
                0.7f, 0.0f, 0.0f, 1.0f,
                0.7f, 0.0f, 0.0f, 1.0f,

                0.8f, 0.0f, 0.0f, 1.0f,
                0.8f, 0.0f, 0.0f, 1.0f,
                0.8f, 0.0f, 0.0f, 1.0f,
                0.8f, 0.0f, 0.0f, 1.0f,

                0.7f, 0.0f, 0.0f, 1.0f,
                0.7f, 0.0f, 0.0f, 1.0f,
                0.7f, 0.0f, 0.0f, 1.0f,
                0.7f, 0.0f, 0.0f, 1.0f,

                0.8f, 0.0f, 0.0f, 1.0f,
                0.8f, 0.0f, 0.0f, 1.0f,
                0.8f, 0.0f, 0.0f, 1.0f,
                0.8f, 0.0f, 0.0f, 1.0f,

                0.7f, 0.0f, 0.0f, 1.0f,
                0.7f, 0.0f, 0.0f, 1.0f,
                0.7f, 0.0f, 0.0f, 1.0f,
                0.7f, 0.0f, 0.0f, 1.0f,

                0.8f, 0.0f, 0.0f, 1.0f,
                0.8f, 0.0f, 0.0f, 1.0f,
                0.8f, 0.0f, 0.0f, 1.0f,
                0.8f, 0.0f, 0.0f, 1.0f,


                0.0f, 1.0f, 0.0f, 1.0f,
                0.0f, 1.0f, 0.0f, 1.0f,
                0.0f, 1.0f, 0.0f, 1.0f,
                0.0f, 1.0f, 0.0f, 1.0f,

                0.0f, 0.9f, 0.0f, 1.0f,
                0.0f, 0.9f, 0.0f, 1.0f,
                0.0f, 0.9f, 0.0f, 1.0f,
                0.0f, 0.9f, 0.0f, 1.0f,

                0.0f, 0.8f, 0.0f, 1.0f,
                0.0f, 0.8f, 0.0f, 1.0f,
                0.0f, 0.8f, 0.0f, 1.0f,
                0.0f, 0.8f, 0.0f, 1.0f,

                0.0f, 0.7f, 0.0f, 1.0f,
                0.0f, 0.7f, 0.0f, 1.0f,
                0.0f, 0.7f, 0.0f, 1.0f,
                0.0f, 0.7f, 0.0f, 1.0f,

                0.0f, 0.8f, 0.0f, 1.0f,
                0.0f, 0.8f, 0.0f, 1.0f,
                0.0f, 0.8f, 0.0f, 1.0f,
                0.0f, 0.8f, 0.0f, 1.0f,

                0.0f, 0.7f, 0.0f, 1.0f,
                0.0f, 0.7f, 0.0f, 1.0f,
                0.0f, 0.7f, 0.0f, 1.0f,
                0.0f, 0.7f, 0.0f, 1.0f,

                0.0f, 0.8f, 0.0f, 1.0f,
                0.0f, 0.8f, 0.0f, 1.0f,
                0.0f, 0.8f, 0.0f, 1.0f,
                0.0f, 0.8f, 0.0f, 1.0f,

                0.0f, 0.7f, 0.0f, 1.0f,
                0.0f, 0.7f, 0.0f, 1.0f,
                0.0f, 0.7f, 0.0f, 1.0f,
                0.0f, 0.7f, 0.0f, 1.0f,

                0.0f, 0.8f, 0.0f, 1.0f,
                0.0f, 0.8f, 0.0f, 1.0f,
                0.0f, 0.8f, 0.0f, 1.0f,
                0.0f, 0.8f, 0.0f, 1.0f,



                0.0f, 0.0f, 1.0f, 1.0f,
                0.0f, 0.0f, 1.0f, 1.0f,
                0.0f, 0.0f, 1.0f, 1.0f,
                0.0f, 0.0f, 1.0f, 1.0f,

                0.0f, 0.0f, 0.9f, 1.0f,
                0.0f, 0.0f, 0.9f, 1.0f,
                0.0f, 0.0f, 0.9f, 1.0f,
                0.0f, 0.0f, 0.9f, 1.0f,

                0.0f, 0.0f, 0.8f, 1.0f,
                0.0f, 0.0f, 0.8f, 1.0f,
                0.0f, 0.0f, 0.8f, 1.0f,
                0.0f, 0.0f, 0.8f, 1.0f,

                0.0f, 0.0f, 0.7f, 1.0f,
                0.0f, 0.0f, 0.7f, 1.0f,
                0.0f, 0.0f, 0.7f, 1.0f,
                0.0f, 0.0f, 0.7f, 1.0f,

                0.0f, 0.0f, 0.8f, 1.0f,
                0.0f, 0.0f, 0.8f, 1.0f,
                0.0f, 0.0f, 0.8f, 1.0f,
                0.0f, 0.0f, 0.8f, 1.0f,

                0.0f, 0.0f, 0.7f, 1.0f,
                0.0f, 0.0f, 0.7f, 1.0f,
                0.0f, 0.0f, 0.7f, 1.0f,
                0.0f, 0.0f, 0.7f, 1.0f,

                0.0f, 0.0f, 0.8f, 1.0f,
                0.0f, 0.0f, 0.8f, 1.0f,
                0.0f, 0.0f, 0.8f, 1.0f,
                0.0f, 0.0f, 0.8f, 1.0f,

                0.0f, 0.0f, 0.7f, 1.0f,
                0.0f, 0.0f, 0.7f, 1.0f,
                0.0f, 0.0f, 0.7f, 1.0f,
                0.0f, 0.0f, 0.7f, 1.0f,

                0.0f, 0.0f, 0.8f, 1.0f,
                0.0f, 0.0f, 0.8f, 1.0f,
                0.0f, 0.0f, 0.8f, 1.0f,
                0.0f, 0.0f, 0.8f, 1.0f,







            };

            uint[] indexArray = new uint[] {
                0, 1, 2,
                0, 3, 2,

                4, 5, 6,
                4, 7, 6,

                8, 9, 10,
                8, 11, 10,

                12, 13, 14,
                12, 15, 14,

                16, 17, 18,  
                16, 19, 18,  

                20, 21, 22,
                20, 23, 22,

                24, 25, 26,
                24, 27, 26,

                28, 29, 30,
                28, 31, 30,

                32, 33, 34,
                32, 35, 34,

                36, 37, 38,
                36, 39, 38,

                40, 41, 42,
                40, 43, 42,

                44, 45, 46,
                44, 47, 46,

                48, 49, 50,
                48, 51, 50,

                52, 53, 54,
                52, 55, 54,

                56, 57, 58,
                56, 59, 58,

                60, 61, 62,
                60, 63, 62,

                64, 65, 66,
                64, 67, 66,

                68, 69, 70,
                68, 71, 70,

                72, 73, 74,
                72, 75, 74,

                76, 77, 78,
                76, 79, 78,

                80, 81, 82,
                80, 83, 82,

                84, 85, 86,
                84, 87, 86,

                88, 89, 90,
                88, 91, 90,

                92, 93, 94,
                92, 95, 94,

                96, 97, 98,
                96, 99, 98,

                100, 101, 102,
                100, 103, 102,

                104, 105, 106,
                104, 107, 106,



            };

            uint vertices = Gl.GenBuffer();
            Gl.BindBuffer(GLEnum.ArrayBuffer, vertices);
            CheckError();
            Gl.BufferData(GLEnum.ArrayBuffer, (ReadOnlySpan<float>)vertexArray.AsSpan(), GLEnum.StaticDraw);
            Gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, null);
            Gl.EnableVertexAttribArray(0);

            uint colors = Gl.GenBuffer();
            Gl.BindBuffer(GLEnum.ArrayBuffer, colors);
            CheckError();
            Gl.BufferData(GLEnum.ArrayBuffer, (ReadOnlySpan<float>)colorArray.AsSpan(), GLEnum.StaticDraw);
            Gl.VertexAttribPointer(1, 4, VertexAttribPointerType.Float, false, 0, null);
            Gl.EnableVertexAttribArray(1);

            uint indices = Gl.GenBuffer();
            Gl.BindBuffer(GLEnum.ElementArrayBuffer, indices);
            CheckError();
            Gl.BufferData(GLEnum.ElementArrayBuffer, (ReadOnlySpan<uint>)indexArray.AsSpan(), GLEnum.StaticDraw);

            Gl.BindBuffer(GLEnum.ArrayBuffer, 0);
            CheckError();
            Gl.UseProgram(program);

            Gl.DrawElements(GLEnum.Triangles, (uint)indexArray.Length, GLEnum.UnsignedInt, null); // we used element buffer
            Gl.BindBuffer(GLEnum.ElementArrayBuffer, 0);

            Gl.BindVertexArray(vao);
            CheckError();
         

            // always unbound the vertex buffer first, so no halfway results are displayed by accident
            Gl.DeleteBuffer(vertices);
            Gl.DeleteBuffer(colors);
            Gl.DeleteBuffer(indices);
            Gl.DeleteVertexArray(vao);
        }
        public static void CheckError()
        {
            var error = (ErrorCode)Gl.GetError();
            if (error != ErrorCode.NoError)
                throw new Exception("GL.GetError() returned " + error.ToString());
        }
    }
}
