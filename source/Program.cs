using OpenTK;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;
using System;
using System.Diagnostics;

namespace Blockland {

  public class Program {

    public static void Main() {

      Window window = new Window("Blockland", 1200, 800);
      ShaderProgram shader = new ShaderProgram();
      Shader vertexShader = new Shader(ShaderType.VertexShader, "shaders/vertex.glsl");
      Shader fragmentShader = new Shader(ShaderType.FragmentShader, "shaders/fragment.glsl");

      shader.Attach(vertexShader);
      shader.Attach(fragmentShader);
      shader.Link();
      shader.Use();

      State state = new StatePrepare(window);
      state.Start();

      while (State.Current != null) {
        State.Current.BeginFrame();
        State.Current.Frame();
        State.Current.EndFrame();
      }

      GC.KeepAlive(fragmentShader);
      GC.KeepAlive(vertexShader);

    }

  }

}