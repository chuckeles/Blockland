using OpenTK;

namespace Blockland {

  public class Transform
    : Component {

    public enum Space {
      Global,
      Local
    }

    public Transform()
      : this(new Vector3()) {
    }

    public Transform(float x, float y, float z)
      : this(new Vector3(x, y, z)) {
    }

    public Transform(Vector3 position)
      : base("Transform") {
      Position = position;
      Rotation = Quaternion.Identity;
    }

    public Transform Move(float x, float y, float z, Space space = Space.Local) {
      return Move(new Vector3(x, y, z), space);
    }

    public Transform Move(Vector3 delta, Space space = Space.Local) {
      if (space == Space.Local)
        Position += Vector3.Transform(delta, Rotation);
      else
        Position += delta;

      return this;
    }

    public Transform Rotate(Vector3 axis, float angle, Space space = Space.Local) {
      return Rotate(Quaternion.FromAxisAngle(axis, angle), space);
    }

    public Transform Rotate(Quaternion delta, Space space = Space.Local) {
      delta.Normalize();

      if (space == Space.Local)
        Rotation = Rotation * delta;
      else
        Rotation = delta * Rotation;

      return this;
    }

    public Matrix4 Matrix {
      get {
        Matrix4 pos = Matrix4.CreateTranslation(Position);
        Matrix4 rot = Matrix4.CreateFromQuaternion(Rotation);

        return pos * rot;
      }
    }

    public Vector3 Position;
    public Quaternion Rotation;

  }

}