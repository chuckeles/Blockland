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

      State state = new State(window);
      state.Start();

      // chunk
      Chunk chunk = new Chunk();
      GameObject chunkObject = new GameObject();
      chunkObject.AddComponent(chunk);

      chunk.Build(shader);

      while (window.Open) {
        state.Frame();
      }

      state.End();

      GC.KeepAlive(fragmentShader);
      GC.KeepAlive(vertexShader);

    }

  }

}