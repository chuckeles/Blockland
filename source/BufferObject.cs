using OpenTK.Graphics.OpenGL4;
using System;

namespace Blockland {

  public class BufferObject {

    public enum Type {
      Vertex,
      Element
    }

    public BufferObject(Type type) {
      mType = type;

      Create();
    }

    ~BufferObject() {
      if (Window.Instance != null && Window.Instance.Open)
        Destroy();
    }

    public void Create() {
      mId = GL.GenBuffer();
    }

    public void Destroy() {
      if (mId != 0) {
        GL.DeleteBuffer(mId);
        mId = 0;
      }
    }

    public void Bind() {
      BufferTarget target = mType == Type.Element ? BufferTarget.ElementArrayBuffer : BufferTarget.ArrayBuffer;

      GL.BindBuffer(target, mId);
    }

    public void CopyData(float[] data, bool bind = false) {
      BufferTarget target = mType == Type.Element ? BufferTarget.ElementArrayBuffer : BufferTarget.ArrayBuffer;

      if (bind)
        Bind();

      GL.BufferData(target, (IntPtr)(data.Length * sizeof(float)), data, BufferUsageHint.StaticDraw);

      mLength = data.Length;
    }

    public void CopyData(uint[] data, bool bind = false) {
      BufferTarget target = mType == Type.Element ? BufferTarget.ElementArrayBuffer : BufferTarget.ArrayBuffer;

      if (bind)
        Bind();

      GL.BufferData(target, (IntPtr)(data.Length * sizeof(uint)), data, BufferUsageHint.StaticDraw);

      mLength = data.Length;
    }

    public int Length {
      get {
        return mLength;
      }
    }

    private Type mType;
    private int mId = 0;
    private int mLength = 0;

  }

}