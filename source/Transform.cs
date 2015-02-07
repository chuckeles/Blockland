using OpenTK;

namespace Blockland {

  /// <summary>
  /// Transform component keeps game object's position and orientation.
  /// </summary>
  public class Transform
    : Component {

    #region Types

    /// <summary>
    /// Transform space.
    /// </summary>
    public enum Space {

      /// <summary>
      /// Transform by world axis, object's orientation has no effect.
      /// </summary>
      Global,

      /// <summary>
      /// Transform by local axis, takes into account object's orientation.
      /// </summary>
      Local
    }

    #endregion Types

    #region Constructor

    /// <summary>
    /// Create a new transform with position at the origin.
    /// </summary>
    public Transform()
      : this(new Vector3()) {
    }

    /// <summary>
    /// Create a new transform on position.
    /// </summary>
    /// <param name="x">X</param>
    /// <param name="y">Y</param>
    /// <param name="z">Z</param>
    public Transform(float x, float y, float z)
      : this(new Vector3(x, y, z)) {
    }

    /// <summary>
    /// Create a new transform on position.
    /// </summary>
    /// <param name="position">Start position</param>
    public Transform(Vector3 position)
      : base("Transform") {
      Position = position;
      Rotation = Quaternion.Identity;
    }

    #endregion Constructor

    #region Methods

    /// <summary>
    /// Called by game object when the transform is attached to it. Registers for render event.
    /// </summary>
    /// <param name="gameObject">Target game object</param>
    public override void Attached(GameObject gameObject) {
      base.Attached(gameObject);

      gameObject.OnRender += UploadModel;
    }

    /// <summary>
    /// Called by game object when the transform is detached from it. Unregisters from render event.
    /// </summary>
    public override void Detached() {
      mGameObject.OnRender -= UploadModel;

      base.Detached();
    }

    /// <summary>
    /// Move the game object.
    /// </summary>
    /// <param name="x">X</param>
    /// <param name="y">Y</param>
    /// <param name="z">Z</param>
    /// <param name="space">Transform space</param>
    /// <returns>This</returns>
    public Transform Move(float x, float y, float z, Space space = Space.Local) {
      return Move(new Vector3(x, y, z), space);
    }

    /// <summary>
    /// Move the game object.
    /// </summary>
    /// <param name="delta">Delta by which to move</param>
    /// <param name="space">Transform space</param>
    /// <returns>This</returns>
    public Transform Move(Vector3 delta, Space space = Space.Local) {
      if (space == Space.Local)
        Position += Vector3.Transform(delta, Rotation);
      else
        Position += delta;

      return this;
    }

    /// <summary>
    /// Rotate the game object around an axis.
    /// </summary>
    /// <param name="axis">Axis around which to rotate</param>
    /// <param name="angle">Angle by which to rotate</param>
    /// <param name="space">Transform space</param>
    /// <returns>This</returns>
    public Transform Rotate(Vector3 axis, float angle, Space space = Space.Local) {
      return Rotate(Quaternion.FromAxisAngle(axis, angle), space);
    }

    /// <summary>
    /// Rotate the game object using quaternion.
    /// </summary>
    /// <param name="delta">Delta Quaternion</param>
    /// <param name="space">Transform space</param>
    /// <returns>This</returns>
    public Transform Rotate(Quaternion delta, Space space = Space.Local) {
      delta.Normalize();

      if (space == Space.Local)
        Rotation = Rotation * delta;
      else
        Rotation = delta * Rotation;

      return this;
    }

    /// <summary>
    /// Update the model matrix.
    /// </summary>
    private void UploadModel() {
      Matrix4 matrix = Matrix;

      ShaderProgram.Current.Uniform("Model", ref matrix);
    }

    #endregion Methods

    #region Properties

    /// <summary>
    /// Create a matrix from the transform.
    /// </summary>
    public Matrix4 Matrix {
      get {
        Matrix4 pos = Matrix4.CreateTranslation(Position);
        Matrix4 rot = Matrix4.CreateFromQuaternion(Rotation);

        return rot * pos;
      }
    }

    #endregion Properties

    #region Fields

    /// <summary>
    /// Game object's position.
    /// </summary>
    public Vector3 Position;

    /// <summary>
    /// Game object's orientation.
    /// </summary>
    public Quaternion Rotation;

    #endregion Fields
  }

}