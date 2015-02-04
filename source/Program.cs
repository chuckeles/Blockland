using OpenTK;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;
using System;
using System.Diagnostics;

namespace Blockland {

  public class Program {

    public static void Main() {

      Window window = new Window("Blockland");
      ShaderProgram shader = new ShaderProgram();

      Shader vertexShader = new Shader(ShaderType.VertexShader, "shaders/vertex.glsl");
      Shader fragmentShader = new Shader(ShaderType.FragmentShader, "shaders/fragment.glsl");

      shader.Attach(vertexShader);
      shader.Attach(fragmentShader);
      shader.Link();
      shader.Use();

      ArrayObject arrayObject = new ArrayObject();
      arrayObject.Bind();

      BufferObject vertex = new BufferObject(BufferObject.Type.Vertex);
      float[] vertexData = {
                             -1f, -1f, -10f,
                              1f, -1f, -10f,
                              1f,  1f, -10f,
                             -1f,  1f, -10f
                           };
      vertex.CopyData(vertexData, true);
      shader.Attribute("inPosition", 3, 0, 0);

      BufferObject element = new BufferObject(BufferObject.Type.Element);
      uint[] elementData = {
                             0, 1, 2,
                             0, 2, 3
                           };
      element.CopyData(elementData, true);

      Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView((float)Math.PI / 2, (float)window.Width / window.Height, .1f, 100f);
      shader.Uniform("projection", ref projection);

      Transform cameraTransform = new Transform();
      GameObject camera = new GameObject();
      camera.AddComponent(cameraTransform);
      camera.AddComponent(new Camera());

      float cameraSpeed = 2f;

      Stopwatch clock = Stopwatch.StartNew();
      float lastTime = 0f;

      while (window.Open) {
        KeyboardState state = Keyboard.GetState();
        float deltaTime = clock.ElapsedMilliseconds / 1000f - lastTime;
        lastTime = clock.ElapsedMilliseconds / 1000f;

        window.Clear();

        int deltaX = (state[Key.D] ? 1 : 0) - (state[Key.A] ? 1 : 0);
        int deltaY = (state[Key.W] ? 1 : 0) - (state[Key.S] ? 1 : 0);

        cameraTransform.Move(deltaX * deltaTime * cameraSpeed, deltaY * deltaTime * cameraSpeed, 0f);

        Matrix4 view = cameraTransform.Matrix.Inverted();
        shader.Uniform("view", ref view);

        GL.DrawElements(BeginMode.Triangles, 6, DrawElementsType.UnsignedInt, 0);

        window.Display();

        window.ProcessEvents();
        if (state[Key.Escape] || !window.Open) {
          window.Close();
          break;
        }

      }

    }

  }

}