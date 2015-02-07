using OpenTK.Input;

namespace Blockland {

  /// <summary>
  /// Main game state. Runs the game loop.
  /// </summary>
  public class GameState
    : State {

    #region Methods

    /// <summary>
    /// End the state.
    /// </summary>
    public override void End() {
      base.End();

      Program.Events.OnKeyDown -= Escape;
    }

    /// <summary>
    /// Event listener for escape key event.
    /// </summary>
    /// <param name="key">Key event</param>
    public void Escape(KeyboardKeyEventArgs key) {
      if (key.Key == Key.Escape) {
        End();
      }
    }

    /// <summary>
    /// Start the state.
    /// </summary>
    public override void Start() {
      base.Start();

      Program.Events.OnKeyDown += Escape;

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