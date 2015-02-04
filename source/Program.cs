using OpenTK;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;
using System;
using System.Diagnostics;

namespace Blockland {

  public class Program {

    public static void Main() {

      // window and shader
      Window window = new Window("Blockland");
      ShaderProgram shader = new ShaderProgram();

      Shader vertexShader = new Shader(ShaderType.VertexShader, "shaders/vertex.glsl");
      Shader fragmentShader = new Shader(ShaderType.FragmentShader, "shaders/fragment.glsl");

      shader.Attach(vertexShader);
      shader.Attach(fragmentShader);
      shader.Link();
      shader.Use();

      // projection matrix
      Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView((float)Math.PI / 4, (float)window.Width / window.Height, .1f, 1000f);
      shader.Uniform("Projection", ref projection);

      // chunk
      Chunk chunk = new Chunk();
      GameObject chunkObject = new GameObject();
      chunkObject.AddComponent(chunk);

      chunk.Build(shader);

      // camera object
      Transform cameraTransform = new Transform();
      Camera camera = new Camera();

      GameObject cameraObject = new GameObject();
      cameraObject.AddComponent(cameraTransform);
      cameraObject.AddComponent(camera);

      window.NativeWindow.MouseDown += (object sender, MouseButtonEventArgs e) => {
        if (e.Button == MouseButton.Left) {
          camera.MouseLock = !camera.MouseLock;
          window.MouseVisible = !camera.MouseLock;
        }
      };

      // move camera back
      cameraTransform.Move(0f, 20f, 40f, Transform.Space.Global);

      // clock
      Stopwatch clock = Stopwatch.StartNew();
      float lastTime = 0f;

      while (window.Open) {
        // keyboard state and delta time
        KeyboardState keyState = Keyboard.GetState();
        float deltaTime = clock.ElapsedMilliseconds / 1000f - lastTime;
        lastTime = clock.ElapsedMilliseconds / 1000f;

        window.Clear();

        // update
        cameraObject.Update(deltaTime);
        chunkObject.Update(deltaTime);

        // center mouse
        if (camera.MouseLock)
          window.CenterMouse();

        // view matrix
        Matrix4 view = cameraTransform.Matrix.Inverted();
        shader.Uniform("View", ref view);

        // rendering
        cameraObject.Draw();
        chunkObject.Draw();

        window.Display();

        // events
        window.ProcessEvents();
        if (keyState[Key.Escape] || !window.Open) {
          window.Close();
          break;
        }

      }

      GC.KeepAlive(fragmentShader);
      GC.KeepAlive(vertexShader);

    }

  }

}