using OpenTK;
using OpenTK.Graphics.OpenGL4;
using System;

namespace Blockland {

  /// <summary>
  /// Represents OpenGL shader program.
  /// </summary>
  public class ShaderProgram
    : IDisposable {

    #region Constructor

    /// <summary>
    /// Create a new shader program.
    /// </summary>
    /// <seealso cref="Create" />
    public ShaderProgram() {
      Create();
    }

    /// <summary>
    /// Destructor.
    /// </summary>
    ~ShaderProgram() {
      if (Window.Instance != null && Window.Instance.Open)
        Dispose();
    }

    #endregion Constructor

    #region Methods

    /// <summary>
    /// Attach a shader to shader program.
    /// </summary>
    /// <param name="shader"></param>
    public void Attach(Shader shader) {
      GL.AttachShader(mId, shader.Id);
    }

    /// <summary>
    /// Set shader attribute. This must be done after binding the buffer object.
    /// </summary>
    /// <param name="name">Attribute name</param>
    /// <param name="size">Size of one vertex attribute (e. g. 3 for position in 3D space)</param>
    /// <param name="stride">
    /// How much data there is between attributes (will be multiplied by size of float)
    /// </param>
    /// <param name="offset">
    /// How much is this attribute offset from start (will be multiplied by size of float)
    /// </param>
    /// <example>
    /// <code>
    /// bufferObject.Bind();
    /// shaderProgram.Attribute("inPosition", 3, 6);
    /// shaderProgram.Attribute("inNormal", 3, 6, 3);
    /// </code>
    /// </example>
    public void Attribute(string name, int size, int stride = 0, int offset = 0) {
      int attrib = GL.GetAttribLocation(mId, name);
      GL.VertexAttribPointer(attrib, size, VertexAttribPointerType.Float, false, sizeof(float) * stride, sizeof(float) * offset);
      GL.EnableVertexAttribArray(attrib);
    }

    /// <summary>
    /// Create a new shader program.
    /// </summary>
    public void Create() {
      mId = GL.CreateProgram();
    }

    /// <summary>
    /// Destroy the shader program and free it from memory.
    /// </summary>
    public void Destroy() {
      if (mId != 0) {
        GL.DeleteProgram(mId);
        mId = 0;
      }
    }

    /// <summary>
    /// Free the object from the memory.
    /// </summary>
    /// <seealso cref="Destroy" />
    public void Dispose() {
      Destroy();
    }

    /// <summary>
    /// Link the shader program. Shaders must be already attached to this shader program.
    /// </summary>
    /// <exception cref="Exception">When the shader program fails to link.</exception>
    public void Link() {
      GL.LinkProgram(mId);

      if (!Linked)
        throw new Exception("Shader program " + mId + " failed to link");
    }

    /// <summary>
    /// Set shader uniform.
    /// </summary>
    /// <param name="uniformName">Name of the uniform (e. g. "View")</param>
    /// <param name="matrix">Matrix to upload</param>
    public void Uniform(string uniformName, ref Matrix4 matrix) {
      int uniform = GL.GetUniformLocation(mId, uniformName);
      GL.UniformMatrix4(uniform, false, ref matrix);
    }

    /// <summary>
    /// Use this shader program for rendering.
    /// </summary>
    public void Use() {
      GL.UseProgram(mId);

      mCurrent = this;
    }

    #endregion Methods

    #region Properties

    /// <summary>
    /// Get current shader program.
    /// </summary>
    public static ShaderProgram Current {
      get {
        return mCurrent;
      }
    }

    /// <summary>
    /// Get OpenGL id.
    /// </summary>
    public int Id {
      get {
        return mId;
      }
    }

    /// <summary>
    /// Check if the shader is linked.
    /// </summary>
    public bool Linked {
      get {
        if (mId == 0)
          return false;

        int status;
        GL.GetProgram(mId, GetProgramParameterName.LinkStatus, out status);
        return status == 1;
      }
    }

    #endregion Properties

    #region Fields

    /// <summary>
    /// Current shader program.
    /// </summary>
    private static ShaderProgram mCurrent;

    /// <summary>
    /// OpenGL id.
    /// </summary>
    private int mId = 0;

    #endregion Fields
  }

}