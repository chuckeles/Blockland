using System.Collections.Generic;
using System.IO;

namespace Blockland {

  /// <summary>
  /// Resource loader and manager.
  /// </summary>
  public class Resources {

    #region Methods

    /// <summary>
    /// Get a resource.
    /// </summary>
    /// <param name="name">Name of the resource</param>
    /// <returns>Requested resource</returns>
    public static IResource Get(string name) {
      return mResources[name];
    }

    /// <summary>
    /// Loads all the necessary resources from the disk.
    /// </summary>
    public static void LoadAll() {
      Shader vertexShader = new Shader("VertexShader", Shader.Type.Vertex, "shaders/vertex.glsl");
      vertexShader.Compile();

      Shader fragmentShader = new Shader("FragmentShader", Shader.Type.Fragment, "shaders/fragment.glsl");
      fragmentShader.Compile();

      ShaderProgram shader = new ShaderProgram();
      shader.Attach(vertexShader);
      shader.Attach(fragmentShader);
      shader.Link();

      shader.Use();

      mResources.Add(vertexShader.Name, vertexShader);
      mResources.Add(fragmentShader.Name, fragmentShader);

      ArrayTexture blockTexture = new ArrayTexture("BlockTextureArray", 64, 64, 3);
      blockTexture.Load("textures/Stone.png");
      blockTexture.Load("textures/Dirt.png");
      blockTexture.Load("textures/Grass.png");

      blockTexture.Use(0);
      ShaderProgram.Current.Uniform("Texture", 0);

      mResources.Add(blockTexture.Name, blockTexture);
    }

    #endregion Methods

    #region Fields

    /// <summary>
    /// Loaded resources.
    /// </summary>
    private static Dictionary<string, IResource> mResources = new Dictionary<string, IResource>();

    #endregion Fields

  }

}