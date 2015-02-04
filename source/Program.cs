using OpenTK;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;
using System;

namespace Blockland {

  public class Program {

    public static void Main() {

      Window window = new Window("Blockland");
      ShaderProgram shader = new ShaderProgram();
      ArrayObject arrayObject = new ArrayObject();
      arrayObject.Bind();

      Shader vertexShader = new Shader(ShaderType.VertexShader, "shaders/vertex.glsl");
      Shader fragmentShader = new Shader(ShaderType.FragmentShader, "shaders/fragment.glsl");

      shader.Attach(vertexShader);
      shader.Attach(fragmentShader);
      shader.Link();
      shader.Use();

      BufferObject vertex = new BufferObject(BufferObject.Type.Vertex);
      float[] vertexData = {
                             -1f, -1f, -10f,
                              1f, -1f, -10f,
                              1f,  1f, -10f,
                             -1f,  1f, -10f
                           };
      vertex.CopyData(vertexData, true);
      shader.Attribute("inPosition", 3, 0, 0);

      Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView((float)Math.PI / 2, (float)window.Width / window.Height, .1f, 100f);
      shader.Uniform("projection", ref projection);

      while (window.Open) {
        window.Clear();

        GL.DrawArrays(PrimitiveType.LineLoop, 0, 4);

        window.Display();

        window.ProcessEvents();
        if (Keyboard.GetState()[Key.Escape] || !window.Open) {
          window.Close();
          break;
        }
      }

    }

  }

}