using OpenTK.Graphics.OpenGL4;
using System;

namespace Blockland {

  /// <summary>
  /// Represents OpenGL vertex buffer object.
  /// </summary>
  public class BufferObject
    : IDisposable {

    /// <summary>
    /// Buffer object type.
    /// </summary>
    public enum Type {
      Vertex,
      Element
    }

    #region Constructor

    /// <summary>
    /// Create new buffer object.
    /// </summary>
    /// <param name="type">Buffer object type</param>
    /// <seealso cref="Create" />
    public BufferObject(Type type) {
      mType = type;
    }

    /// <summary>
    /// Destructor.
    /// </summary>
    ~BufferObject() {
      if (Window.Instance != null && Window.Instance.Open)
        Dispose();
    }

    #endregion Constructor

    #region Methods

    /// <summary>
    /// Bind buffer object.
    /// </summary>
    public void Bind() {
      BufferTarget target = mType == Type.Element ? BufferTarget.ElementArrayBuffer : BufferTarget.ArrayBuffer;

      GL.BindBuffer(target, mId);
    }

    /// <summary>
    /// Copy data from float array to buffer object.
    /// </summary>
    /// <param name="data">Float data</param>
    /// <param name="bind">If true, the buffer object will be bound before copying</param>
    public void CopyData(float[] data, bool bind = false) {
      BufferTarget target = mType == Type.Element ? BufferTarget.ElementArrayBuffer : BufferTarget.ArrayBuffer;

      if (bind)
        Bind();

      GL.BufferData(target, (IntPtr)(data.Length * sizeof(float)), data, BufferUsageHint.StaticDraw);

      mLength = data.Length;
    }

    /// <summary>
    /// Copy data from uint array to buffer object.
    /// </summary>
    /// <param name="data">Uint data</param>
    /// <param name="bind">If true, the buffer object will be bound before copying</param>
    public void CopyData(uint[] data, bool bind = false) {
      BufferTarget target = mType == Type.Element ? BufferTarget.ElementArrayBuffer : BufferTarget.ArrayBuffer;

      if (bind)
        Bind();

      GL.BufferData(target, (IntPtr)(data.Length * sizeof(uint)), data, BufferUsageHint.StaticDraw);

      mLength = data.Length;
    }

    /// <summary>
    /// Create new buffer object.
    /// </summary>
    public void Create() {
      mId = GL.GenBuffer();
    }

    /// <summary>
    /// Destroy buffer object and free it from memory.
    /// </summary>
    public void Destroy() {
      if (mId != 0) {
        GL.DeleteBuffer(mId);
        mId = 0;
      }
    }

    /// <summary>
    /// Free object from memory.
    /// </summary>
    /// <seealso cref="Destroy" />
    public void Dispose() {
      Destroy();
    }

    #endregion Methods

    #region Properties

    public int Length {
      get {
        return mLength;
      }
    }

    #endregion Properties

    #region Fields

    /// <summary>
    /// OpenGl id.
    /// </summary>
    private int mId = 0;

    /// <summary>
    /// Buffer object length.
    /// </summary>
    private int mLength = 0;

    /// <summary>
    /// Buffer object type.
    /// </summary>
    private Type mType;

    #endregion Fields
  }

}