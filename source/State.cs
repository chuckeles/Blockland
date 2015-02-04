using OpenTK;
using OpenTK.Input;
using System;
using System.Collections;
using System.Diagnostics;

namespace Blockland {

  public class State {

    public State(Window window) {
      mWindow = window;
    }

    public virtual void Start() {
      // set current
      if (mCurrent != null)
        mCurrent.End();

      mCurrent = this;

      // set up camera
      Camera camera = new Camera();

      mCamera.AddComponent(new Transform(0f, 20f, 40f));
      mCamera.AddComponent(camera);

      mWindow.NativeWindow.MouseDown += OnMouseDown;

      // start clock
      mClock.Start();
    }

    private void OnMouseDown(object sender, MouseButtonEventArgs e) {
      Camera camera = mCamera["Camera"] as Camera;

      if (e.Button == MouseButton.Left) {
        camera.MouseLock = !camera.MouseLock;
        mWindow.MouseVisible = !camera.MouseLock;
      }
    }

    public virtual void BeginFrame() {
      // delta time
      mDeltaTime = mClock.ElapsedMilliseconds / 1000f - mLastTime;
      mLastTime = mClock.ElapsedMilliseconds / 1000f;

      // clear window
      mWindow.Clear();
    }

    public virtual void Frame() {
      // update camera
      mCamera.Update(mDeltaTime);

      // update view
      Transform cameraTransform = mCamera["Transform"] as Transform;
      Matrix4 view = cameraTransform.Matrix.Inverted();
      ShaderProgram.Current.Uniform("View", ref view);

      // draw camera
      mCamera.Draw();
    }

    public virtual void EndFrame() {
      mWindow.Display();
      mWindow.ProcessEvents();

      if (!mWindow.Open)
        End();
    }

    public virtual void End() {
      mWindow.NativeWindow.MouseDown -= OnMouseDown;

      mCurrent = null;
    }

    public Window Window {
      get {
        return mWindow;
      }
    }

    public ArrayList GameObjects {
      get {
        return mGameObjects;
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

    protected Window mWindow;
    protected GameObject mCamera = new GameObject();
    protected ArrayList mGameObjects = new ArrayList();
    private ArrayList mToRemove = new ArrayList();

    private Stopwatch mClock = new Stopwatch();
    private float mLastTime = 0f;
    protected float mDeltaTime = 0f;

    protected static State mCurrent;

  }

}