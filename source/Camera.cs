using OpenTK;
using OpenTK.Input;

namespace Blockland {

  /// <summary>
  /// Camera component updates the view matrix.
  /// </summary>
  public class Camera
    : Component {

    #region Constructor

    /// <summary>
    /// Create a new camera component and set it's speed parameters.
    /// </summary>
    /// <param name="cameraSpeed">Movement speed</param>
    /// <param name="cameraRotateSpeed">Rotation speed</param>
    public Camera(float cameraSpeed = 30f, float cameraRotateSpeed = 0.003f)
      : base("Camera") {
      Speed = cameraSpeed;
      RotateSpeed = cameraRotateSpeed;
    }

    #endregion Constructor

    #region Methods

    /// <summary>
    /// Called by game object when the camera component is attached to a game object. Registers for
    /// the update event and the mouse button down event.
    /// </summary>
    /// <param name="gameObject">Target game object</param>
    public override void Attached(GameObject gameObject) {
      base.Attached(gameObject);

      gameObject.EnsureTransform();
      gameObject.OnUpdate += Update;

      Program.Events.OnMouseButtonDown += MouseDown;
    }

    /// <summary>
    /// Called by game object when the camera component is detached from it. Unregisters from the
    /// update event and the mouse button down event.
    /// </summary>
    public override void Detached() {
      Program.Events.OnMouseButtonDown -= MouseDown;
      mGameObject.OnUpdate -= Update;

      base.Detached();
    }

    /// <summary>
    /// Update the camera. This reads the input from the keyboard and the mouse and moves the camera
    /// </summary>
    /// <param name="deltaTime"></param>
    public void Update(float deltaTime) {
      MouseState mouseState = Mouse.GetState();
      KeyboardState keyState = Keyboard.GetState();
      Transform transform = mGameObject["Transform"] as Transform;

      // move
      int deltaX = (keyState[Key.D] ? 1 : 0) - (keyState[Key.A] ? 1 : 0);
      int deltaY = (keyState[Key.Space] ? 1 : 0) - (keyState[Key.ControlLeft] ? 1 : 0);
      int deltaZ = (keyState[Key.S] ? 1 : 0) - (keyState[Key.W] ? 1 : 0);

      int boost = keyState[Key.ShiftLeft] ? 2 : 1;

      transform.Move(deltaX * deltaTime * Speed * boost, 0f, deltaZ * deltaTime * Speed * boost);
      transform.Move(0f, deltaY * deltaTime * Speed * boost, 0f, Transform.Space.Global);

      // rotate
      if (MouseLock) {
        float mouseDeltaX = mouseState.X - mMousePrevious.X;
        float mouseDeltaY = mouseState.Y - mMousePrevious.Y;

        transform.Rotate(Vector3.UnitY, -mouseDeltaX * RotateSpeed, Transform.Space.Global);
        transform.Rotate(Vector3.UnitX, -mouseDeltaY * RotateSpeed);

        // center mouse
        Window.Instance.CenterMouse();
      }

      mMousePrevious.X = mouseState.X;
      mMousePrevious.Y = mouseState.Y;

      // update view matrix
      Matrix4 view = transform.Matrix;
      view.Invert();
      ShaderProgram.Current.Uniform("View", ref view);
    }

    /// <summary>
    /// Toggle the mouse lock.
    /// </summary>
    /// <param name="button">Button event</param>
    private void MouseDown(MouseButtonEventArgs button) {
      if (button.Button == MouseButton.Left) {
        MouseLock = !MouseLock;
        Window.Instance.MouseVisible = !MouseLock;
      }
    }

    #endregion Methods

    #region Fields

    /// <summary>
    /// When the mouse is locked, it is invisible and kept centered in the window.
    /// </summary>
    public bool MouseLock = false;

    /// <summary>
    /// Rotate speed.
    /// </summary>
    public float RotateSpeed;

    /// <summary>
    /// Movement speed.
    /// </summary>
    public float Speed;

    /// <summary>
    /// Position of the mouse in the last update.
    /// </summary>
    private Vector2 mMousePrevious = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);

    #endregion Fields
  }

}