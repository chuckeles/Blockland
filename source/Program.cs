using OpenTK.Graphics.OpenGL4;
using System;

namespace Blockland {

  /// <summary>
  /// Represents the Blockland program. Contains program-wide events and the state of the program.
  /// </summary>
  public class Program {

    #region Methods

    /// <summary>
    /// Program entry point.
    /// </summary>
    public static void Main() {

      Window.Create("Blockland", 1200, 800);
      Resources.LoadAll();

      State.Add(new PrepareState());
      State.Add(new GameState());

      State.Run();

    }

    #endregion Methods

    #region Fields

    /// <summary>
    /// Program-wide events.
    /// </summary>
    public static Events Events = new Events();

    /// <summary>
    /// Current state of the program.
    /// </summary>
    public static State State;

    #endregion Fields

  }

}