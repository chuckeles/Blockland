using System.Collections;
using System.Diagnostics;

namespace Blockland {

  /// <summary>
  /// Represents a state in which the program currently is.
  /// </summary>
  public class State {

    #region Methods

    /// <summary>
    /// Add new state to the state queue.
    /// </summary>
    /// <param name="state">State to add</param>
    public static void Add(State state) {
      mStates.Enqueue(state);
    }

    /// <summary>
    /// Execute all the states in the queue.
    /// </summary>
    public static void Run() {
      Stopwatch clock = Stopwatch.StartNew();
      float lastTime = 0f;

      while (true) {

        if (mCurrent != null) {
          if (mCurrent.Running) {
            float deltaTime = clock.ElapsedMilliseconds / 1000f - lastTime;
            lastTime = clock.ElapsedMilliseconds / 1000f;

            mCurrent.Update(deltaTime);
          }
          else
            mCurrent = null;
        }
        else {
          if (mStates.Count > 0) {
            mCurrent = mStates.Dequeue() as State;
            mCurrent.Start();
          }
          else
            break;
        }

      }
    }

    /// <summary>
    /// Add new game object to the list.
    /// </summary>
    /// <param name="gameObject">Game object to add</param>
    public void AddGameObject(GameObject gameObject) {
      mGameObjects.Add(gameObject);
    }

    /// <summary>
    /// End the state.
    /// </summary>
    public virtual void End() {
      Program.Events.End();

      RemoveGameObject(mCamera);
      mCamera = new GameObject();

      mRunning = false;
    }

    /// <summary>
    /// Remove existing game object from the list.
    /// </summary>
    /// <param name="gameObject">Game object to remove</param>
    public void RemoveGameObject(GameObject gameObject) {
      mToRemove.Add(gameObject);
    }

    /// <summary>
    /// Start the state.
    /// </summary>
    public virtual void Start() {
      mRunning = true;

      mCamera.AddComponent(new Transform());
      mCamera.AddComponent(new Camera());

      AddGameObject(mCamera);

      Program.Events.Start();
    }

    /// <summary>
    /// Update the state.
    /// </summary>
    /// <param name="deltaTime">Delta time since last update.</param>
    public virtual void Update(float deltaTime) {
      foreach (GameObject gameObject in mToRemove)
        mGameObjects.Remove(gameObject);
      mToRemove.Clear();

      Program.Events.Update(deltaTime);

      if (!mRunning)
        return;

      Program.Events.Clear();
      Program.Events.Render();
      Program.Events.Display();
    }

    #endregion Methods

    #region Properties

    /// <summary>
    /// Get current program state.
    /// </summary>
    public static State Current {
      get {
        return mCurrent;
      }
    }

    /// <summary>
    /// Get camera object.
    /// </summary>
    public GameObject Camera {
      get {
        return mCamera;
      }
    }

    /// <summary>
    /// Get list of game objects.
    /// </summary>
    public ArrayList GameObjects {
      get {
        return mGameObjects;
      }
    }

    /// <summary>
    /// Check if the state is running.
    /// </summary>
    public bool Running {
      get {
        return mRunning;
      }
    }

    #endregion Properties

    #region Fields

    /// <summary>
    /// The camera object.
    /// </summary>
    protected GameObject mCamera = new GameObject();

    /// <summary>
    /// List of game objects.
    /// </summary>
    protected ArrayList mGameObjects = new ArrayList();

    /// <summary>
    /// Current program state.
    /// </summary>
    private static State mCurrent;

    /// <summary>
    /// Queue of program states.
    /// </summary>
    private static Queue mStates = new Queue();

    /// <summary>
    /// Whether the state is running.
    /// </summary>
    private bool mRunning = false;

    /// <summary>
    /// List of game objects to remove.
    /// </summary>
    private ArrayList mToRemove = new ArrayList();

    #endregion Fields

  }

}