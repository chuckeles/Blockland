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

      ShaderProgram shader = new ShaderProgram();
      Shader vertexShader = new Shader(ShaderType.VertexShader, "shaders/vertex.glsl");
      Shader fragmentShader = new Shader(ShaderType.FragmentShader, "shaders/fragment.glsl");

      shader.Attach(vertexShader);
      shader.Attach(fragmentShader);
      shader.Link();
      shader.Use();

      State state = new StatePrepare(Window.Instance);
      state.Start();

      while (State.Current != null) {
        State.Current.BeginFrame();
        State.Current.Frame();
        State.Current.EndFrame();
      }

      GC.KeepAlive(fragmentShader);
      GC.KeepAlive(vertexShader);

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