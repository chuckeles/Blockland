using OpenTK.Graphics.OpenGL4;
using System;

namespace Blockland {

  /// <summary>
  /// Represents an OpenGL array buffer object.
  /// </summary>
  public class ArrayObject
    : IDisposable {

    #region Constructor

    /// <summary>
    /// Destructor.
    /// </summary>
    //~ArrayObject() {
    //  if (Window.Instance != null && Window.Instance.Open)
    //    Dispose();
    //}

    #endregion Constructor

    #region Methods

    /// <summary>
    /// Bind array object.
    /// </summary>
    public void Bind() {
      GL.BindVertexArray(mId);
    }

    /// <summary>
    /// Create new array object.
    /// </summary>
    public void Create() {
      mId = GL.GenVertexArray();
    }

    /// <summary>
    /// Destroy array object and free it from the memory.
    /// </summary>
    public void Destroy() {
      if (mId != 0) {
        GL.DeleteVertexArray(mId);
        mId = 0;
      }
    }

    /// <summary>
    /// Free the object from memory.
    /// </summary>
    /// <seealso cref="Destroy" />
    public void Dispose() {
      Destroy();
    }

    #endregion Methods

    #region Properties

    /// <summary>
    /// Get OpenGL id.
    /// </summary>
    public int Id {
      get {
        return mId;
      }
    }

    #endregion Properties

    #region Fields

    /// <summary>
    /// OpenGL id.
    /// </summary>
    private int mId = 0;

    #endregion Fields

  }

}