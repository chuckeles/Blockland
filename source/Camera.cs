namespace Blockland {

  public class Camera
    : Component {

    public Camera()
      : base("Camera") {
    }

    public override void Attached(GameObject gameObject) {
      base.Attached(gameObject);

      gameObject.EnsureTransform();
    }

  }

}