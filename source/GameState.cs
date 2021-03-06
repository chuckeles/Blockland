﻿using OpenTK.Input;

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

      World.Current.Destroy();

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

      World world = new World();
      world.Create(12, 8);

      (mCamera["Transform"] as Transform).Move(Chunk.Size * Block.Size / 2, Chunk.Size * Block.Size * 6, Chunk.Size * Block.Size / 2);
    }

    #endregion Methods

  }

}