using OpenTK;
using OpenTK.Input;

namespace Blockland {

  public class Camera
    : Component {

    public Camera(float cameraSpeed = 30f, float cameraRotateSpeed = 0.003f)
      : base("Camera") {
      Speed = cameraSpeed;
      RotateSpeed = cameraRotateSpeed;

      mMousePrevious = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
    }

    public override void Attached(GameObject gameObject) {
      base.Attached(gameObject);

      gameObject.EnsureTransform();

      gameObject.OnUpdate += Update;
    }

    public void Update(float deltaTime) {
      MouseState mouseState = Mouse.GetState();
      KeyboardState keyState = Keyboard.GetState();

      Transform transform = (Transform)mGameObject.GetComponent("Transform");

      int deltaX = (keyState[Key.D] ? 1 : 0) - (keyState[Key.A] ? 1 : 0);
      int deltaY = (keyState[Key.Space] ? 1 : 0) - (keyState[Key.ControlLeft] ? 1 : 0);
      int deltaZ = (keyState[Key.S] ? 1 : 0) - (keyState[Key.W] ? 1 : 0);

      int boost = keyState[Key.ShiftLeft] ? 2 : 1;

      transform.Move(deltaX * deltaTime * Speed * boost, 0f, deltaZ * deltaTime * Speed * boost);
      transform.Move(0f, deltaY * deltaTime * Speed * boost, 0f, Transform.Space.Global);

      if (MouseLock) {
        float mouseDeltaX = mouseState.X - mMousePrevious.X;
        float mouseDeltaY = mouseState.Y - mMousePrevious.Y;

        transform.Rotate(Vector3.UnitY, -mouseDeltaX * RotateSpeed, Transform.Space.Global);
        transform.Rotate(Vector3.UnitX, -mouseDeltaY * RotateSpeed);

        Window.Instance.CenterMouse();
      }

      mMousePrevious.X = mouseState.X;
      mMousePrevious.Y = mouseState.Y;
    }

    public float Speed;
    public float RotateSpeed;

    private Vector2 mMousePrevious;

    public bool MouseLock = false;

  }

}