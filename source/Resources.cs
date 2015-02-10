using System.Collections.Generic;

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
      shader.Uniform("uTexture", 0);
      shader.Uniform("uNormalTexture", 1);

      mResources.Add(vertexShader.Name, vertexShader);
      mResources.Add(fragmentShader.Name, fragmentShader);

      ArrayTexture.Use(0);
      ArrayTexture blockTexture = new ArrayTexture("BlockTextureArray", 64, 64, 3);
      blockTexture.Load("textures/Stone.png");
      blockTexture.Load("textures/Dirt.png");
      blockTexture.Load("textures/Grass.png");

      mResources.Add(blockTexture.Name, blockTexture);

      ArrayTexture.Use(1);
      ArrayTexture blockNormalTexture = new ArrayTexture("BlockNormalTextureArray", 64, 64, 3);
      blockNormalTexture.Load("textures/StoneNormal.png");
      blockNormalTexture.Load("textures/DirtNormal.png");
      blockNormalTexture.Load("textures/GrassNormal.png");

      mResources.Add(blockNormalTexture.Name, blockNormalTexture);
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