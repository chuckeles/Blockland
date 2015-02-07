using OpenTK.Graphics.OpenGL4;
using System;
using System.IO;

namespace Blockland {

  /// <summary>
  /// OpenGL shader.
  /// </summary>
  public class Shader
    : IResource {

    #region Types

    /// <summary>
    /// Enum for shader type.
    /// </summary>
    public enum Type {
      Vertex,
      Fragment
    }

    #endregion Types

    #region Constructor

    /// <summary>
    /// Create a new shader.
    /// </summary>
    /// <param name="type">Shader type</param>
    /// <exception cref="ArgumentNullException">When empty name is supplied</exception>
    public Shader(string name, Type type) {
      if (name == "")
        throw new ArgumentNullException("Resource needs a name");

      mType = type;
      mName = name;
    }

    /// <summary>
    /// Create a new shader, load and compile it.
    /// </summary>
    /// <param name="type">Shader type</param>
    /// <param name="fileName">Source file</param>
    public Shader(string name, Type type, string fileName)
      : this(name, type) {
      Create(fileName);
    }

    /// <summary>
    /// Destructor.
    /// </summary>
    ~Shader() {
      if (Window.Instance != null && Window.Instance.Open)
        Dispose();
    }

    #endregion Constructor

    #region Methods

    /// <summary>
    /// Compile the shader.
    /// </summary>
    /// <exception cref="Exception">When the shader fails to compile</exception>
    public void Compile() {
      GL.CompileShader(mId);

      if (!Compiled)
        throw new exce("Shader " + mId + " failed to compile");
    }

    /// <summary>
    /// Load and compile the shader.
    /// </summary>
    /// <param name="fileName">Source file</param>
    public void Create(string fileName) {
      Load(fileName);
      Compile();
    }

    /// <summary>
    /// Destroy the shader and release it from memory.
    /// </summary>
    public void Destroy() {
      if (mId != 0) {
        GL.DeleteShader(mId);
        mId = 0;
      }
    }

    /// <summary>
    /// Release the shader from memory.
    /// </summary>
    /// <seealso cref="Destroy" />
    public void Dispose() {
      Destroy();
    }

    /// <summary>
    /// Load shader code from a file.
    /// </summary>
    /// <param name="fileName">Source file</param>
    public void Load(string fileName) {
      if (mLoaded)
        return;

      StreamReader file = new StreamReader(fileName);
      string code = file.ReadToEnd();
      file.Close();

      mId = GL.CreateShader(mType == Type.Vertex ? ShaderType.VertexShader : ShaderType.FragmentShader);
      GL.ShaderSource(mId, code);

      mLoaded = true;
    }

    #endregion Methods

    #region Properties

    /// <summary>
    /// Check if the shader is compiled.
    /// </summary>
    public bool Compiled {
      get {
        if (mId == 0)
          return false;
        else {
          int status;
          GL.GetShader(mId, ShaderParameter.CompileStatus, out status);

          return status == 1;
        }
      }
    }

    /// <summary>
    /// Get OpenGL shader id.
    /// </summary>
    public int Id {
      get {
        return mId;
      }
    }

    /// <summary>
    /// Check is the resource is loaded.
    /// </summary>
    public bool Loaded {
      get {
        return mLoaded;
      }
    }

    /// <summary>
    /// Get resource name.
    /// </summary>
    public string Name {
      get {
        return mName;
      }
    }

    #endregion Properties

    #region Fields

    /// <summary>
    /// OpenGL shader id.
    /// </summary>
    private int mId = 0;

    /// <summary>
    /// Whether the resource is loaded.
    /// </summary>
    private bool mLoaded = false;

    /// <summary>
    /// Resource name.
    /// </summary>
    private string mName;

    /// <summary>
    /// Shader type.
    /// </summary>
    private Type mType;

    #endregion Fields
  }

}