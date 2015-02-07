using OpenTK.Input;

namespace Blockland {

  /// <summary>
  /// Main game state. Runs the game loop.
  /// </summary>
  public class StateGame
    : State {

    #region Methods

    /// <summary>
    /// End the state.
    /// </summary>
    public override void End() {
      base.End();

      Window.Instance.NativeWindow.KeyDown -= OnEscape;
    }

    /// <summary>
    /// Event listener for escape key event.
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="e">Key event</param>
    public void OnEscape(object sender, KeyboardKeyEventArgs e) {
      if (e.Key == Key.Escape)
        Window.Instance.Close();
    }

    /// <summary>
    /// Start the state.
    /// </summary>
    public override void Start() {
      base.Start();

      Window.Instance.NativeWindow.KeyDown += OnEscape;

      int blocks = 8;
      mWorld.Create(blocks, 8);

      float halfSize = blocks * Chunk.Size * Block.Size / 2;
      (mCamera["Transform"] as Transform).Move(halfSize, 100f, halfSize);
    }

    #endregion Methods

    #region Fields

    /// <summary>
    /// World instance.
    /// </summary>
    private World mWorld = new World();

    #endregion Fields

  }

}