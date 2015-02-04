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

      // block
      ArrayObject arrayObject = new ArrayObject();
      arrayObject.Bind();

      BufferObject vertex = new BufferObject(BufferObject.Type.Vertex);
      float[] vertexData = {
                             // front
                             -1f, -1f,  1f,  0f,  0f,  1f,
                              1f, -1f,  1f,  0f,  0f,  1f,
                              1f,  1f,  1f,  0f,  0f,  1f,
                             -1f,  1f,  1f,  0f,  0f,  1f,

                             // back
                              1f, -1f, -1f,  0f,  0f, -1f,
                             -1f, -1f, -1f,  0f,  0f, -1f,
                             -1f,  1f, -1f,  0f,  0f, -1f,
                              1f,  1f, -1f,  0f,  0f, -1f,

                              // right
                              1f, -1f,  1f,  1f,  0f,  0f,
                              1f, -1f, -1f,  1f,  0f,  0f,
                              1f,  1f, -1f,  1f,  0f,  0f,
                              1f,  1f,  1f,  1f,  0f,  0f,

                              // left
                             -1f, -1f, -1f, -1f,  0f,  0f,
                             -1f, -1f,  1f, -1f,  0f,  0f,
                             -1f,  1f,  1f, -1f,  0f,  0f,
                             -1f,  1f, -1f, -1f,  0f,  0f,

                             // top
                             -1f,  1f,  1f,  0f,  1f,  0f,
                              1f,  1f,  1f,  0f,  1f,  0f,
                              1f,  1f, -1f,  0f,  1f,  0f,
                             -1f,  1f, -1f,  0f,  1f,  0f,

                             // bottom
                              1f, -1f,  1f,  0f, -1f,  0f,
                             -1f, -1f,  1f,  0f, -1f,  0f,
                             -1f, -1f, -1f,  0f, -1f,  0f,
                              1f, -1f, -1f,  0f, -1f,  0f
                           };
      vertex.CopyData(vertexData, true);
      shader.Attribute("inPosition", 3, sizeof(float) * 6, 0);
      shader.Attribute("inNormal", 3, sizeof(float) * 6, sizeof(float) * 3);

      BufferObject element = new BufferObject(BufferObject.Type.Element);
      uint[] elementData = {
                             // front
                             0, 1, 2,
                             0, 2, 3,

                             // back
                             4, 5, 6,
                             4, 6, 7,

                             // right
                             8, 9, 10,
                             8, 10, 11,

                             // left
                             12, 13, 14,
                             12, 14, 15,

                             // top
                             16, 17, 18,
                             16, 18, 19,

                             // bottom
                             20, 21, 22,
                             20, 22, 23
                           };
      element.CopyData(elementData, true);

      // projection matrix
      Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView((float)Math.PI / 4, (float)window.Width / window.Height, .1f, 100f);
      shader.Uniform("Projection", ref projection);

      // camera object
      Transform cameraTransform = new Transform();
      GameObject camera = new GameObject();
      camera.AddComponent(cameraTransform);
      camera.AddComponent(new Camera());

      // camera control
      float cameraSpeed = 10f;
      float cameraRotateSpeed = 0.003f;

      bool mouseLock = false;
      window.NativeWindow.MouseDown += (object sender, MouseButtonEventArgs e) => {
        if (e.Button == MouseButton.Left) {
          mouseLock = !mouseLock;
          window.MouseVisible = !mouseLock;
        }
      };

      // previous mouse position
      Vector2 mousePrevious = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);

      // move camera back
      cameraTransform.Move(0f, 0f, 10f, Transform.Space.Global);

      // clock
      Stopwatch clock = Stopwatch.StartNew();
      float lastTime = 0f;

      while (window.Open) {
        // keyboard state, mouse state and delta time
        MouseState mouseState = Mouse.GetState();
        KeyboardState keyState = Keyboard.GetState();
        float deltaTime = clock.ElapsedMilliseconds / 1000f - lastTime;
        lastTime = clock.ElapsedMilliseconds / 1000f;

        window.Clear();

        // move camera
        int deltaX = (keyState[Key.D] ? 1 : 0) - (keyState[Key.A] ? 1 : 0);
        int deltaY = (keyState[Key.S] ? 1 : 0) - (keyState[Key.W] ? 1 : 0);

        cameraTransform.Move(deltaX * deltaTime * cameraSpeed, 0f, deltaY * deltaTime * cameraSpeed);

        // rotate camera
        if (mouseLock) {
          float mouseDeltaX = mouseState.X - mousePrevious.X;
          float mouseDeltaY = mouseState.Y - mousePrevious.Y;

          cameraTransform.Rotate(Vector3.UnitY, -mouseDeltaX * cameraRotateSpeed, Transform.Space.Global);
          cameraTransform.Rotate(Vector3.UnitX, -mouseDeltaY * cameraRotateSpeed);
        }

        // center mouse
        if (mouseLock)
          window.CenterMouse();

        // reset mouse
        mousePrevious.X = mouseState.X;
        mousePrevious.Y = mouseState.Y;

        // view matrix
        Matrix4 view = cameraTransform.Matrix.Inverted();
        shader.Uniform("View", ref view);

        // rendering
        GL.DrawElements(BeginMode.Triangles, 38, DrawElementsType.UnsignedInt, 0);

        window.Display();

        // events
        window.ProcessEvents();
        if (keyState[Key.Escape] || !window.Open) {
          window.Close();
          break;
        }

      }

    }

  }

}