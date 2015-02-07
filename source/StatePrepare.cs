using OpenTK;

namespace Blockland {

  /// <summary>
  /// Program state to prepare program before any execution. Currently only sets up the projection matrix.
  /// </summary>
  public class StatePrepare
    : State {

    #region Methods

    /// <summary>
    /// Start the state. Sets up the projection matrix.
    /// </summary>
    public override void Start() {
      base.Start();

      Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, (float)Window.Instance.Width / Window.Instance.Height, .1f, 1000f);
      ShaderProgram.Current.Uniform("Projection", ref projection);

      End();
    }

    #endregion Methods

  }

}