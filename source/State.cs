using OpenTK;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;
using System;
using System.Diagnostics;

namespace Blockland {

  public class State {

    public State(Window window) {
      mWindow = window;
    }

    public void Start() {
      // set current
      if (mCurrent != null)
        mCurrent.End();

      mCurrent = this;

      // set up projection
      Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView((float)Math.PI / 4, (float)mWindow.Width / mWindow.Height, .1f, 1000f);
      ShaderProgram.Current.Uniform("Projection", ref projection);

      // set up camera
      Camera camera = new Camera();

      mCamera.AddComponent(new Transform(0f, 20f, 40f));
      mCamera.AddComponent(camera);

      mWindow.NativeWindow.MouseDown += (object sender, MouseButtonEventArgs e) => {
        if (e.Button == MouseButton.Left) {
          camera.MouseLock = !camera.MouseLock;
          mWindow.MouseVisible = !camera.MouseLock;
        }
      };

      // listen to escape
      mWindow.NativeWindow.KeyDown += (object sender, KeyboardKeyEventArgs e) => {
        if (e.Key == Key.Escape)
          mWindow.Close();
      };

      // start clock
      mClock.Start();
    }

    public void Frame() {
      // delta time
      float deltaTime = mClock.ElapsedMilliseconds / 1000f - mLastTime;
      mLastTime = mClock.ElapsedMilliseconds / 1000f;

      mWindow.Clear();

      mCamera.Update(deltaTime);

      // update view
      Transform cameraTransform = mCamera["Transform"] as Transform;
      Matrix4 view = cameraTransform.Matrix.Inverted();
      ShaderProgram.Current.Uniform("View", ref view);

      mCamera.Draw();

      mWindow.Display();

      mWindow.ProcessEvents();
    }

    public void End() {
      mCurrent = null;
    }

    public Window Window {
      get {
        return mWindow;
      }
    }

    public bool Running {
      get {
        return mCurrent == this;
      }
    }

    public static State Current {
      get {
        return mCurrent;
      }
    }

    private Window mWindow;
    private GameObject mCamera = new GameObject();
    private Stopwatch mClock = new Stopwatch();
    private float mLastTime = 0f;

    private static State mCurrent;

  }

}