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

    public void AddGameObject(GameObject gameObject) {
      mGameObjects.Add(gameObject);
    }

    public virtual void BeginFrame() {
      // delta time
      mDeltaTime = mClock.ElapsedMilliseconds / 1000f - mLastTime;
      mLastTime = mClock.ElapsedMilliseconds / 1000f;

      // clear window
      mWindow.Clear();
    }

    public virtual void End() {
      mWindow.NativeWindow.MouseDown -= OnMouseDown;

      mCurrent = null;
    }

    public virtual void EndFrame() {
      mWindow.Display();

      if (!mWindow.Open)
        End();
    }

    public virtual void Frame() {
      // remove objects
      foreach (GameObject gameObject in mToRemove)
        mGameObjects.Remove(gameObject);
      mToRemove.Clear();

      // update camera
      mCamera.Update(mDeltaTime);

      // update objects
      foreach (GameObject gameObject in mGameObjects)
        gameObject.Update(mDeltaTime);

      // update view
      Transform cameraTransform = mCamera["Transform"] as Transform;
      Matrix4 view = cameraTransform.Matrix.Inverted();
      ShaderProgram.Current.Uniform("View", ref view);

      // draw camera
      mCamera.Draw();

      // draw objects
      foreach (GameObject gameObject in mGameObjects)
        gameObject.Draw();
    }

    public void RemoveGameObject(GameObject gameObject) {
      mToRemove.Add(gameObject);
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

    public static State Current {
      get {
        return mCurrent;
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

    public Window Window {
      get {
        return mWindow;
      }
    }

    protected static State mCurrent;

    protected GameObject mCamera = new GameObject();

    protected float mDeltaTime = 0f;

    protected ArrayList mGameObjects = new ArrayList();

    protected Window mWindow;

    private void OnMouseDown(object sender, MouseButtonEventArgs e) {
      Camera camera = mCamera["Camera"] as Camera;

      if (e.Button == MouseButton.Left) {
        camera.MouseLock = !camera.MouseLock;
        mWindow.MouseVisible = !camera.MouseLock;
      }
    }

    private Stopwatch mClock = new Stopwatch();
    private float mLastTime = 0f;
    private ArrayList mToRemove = new ArrayList();
  }

}