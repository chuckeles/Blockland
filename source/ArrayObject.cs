using OpenTK.Graphics.OpenGL4;

namespace Blockland {

  public class ArrayObject {

    public ArrayObject() {
      Create();
    }

    ~ArrayObject() {
      if (Window.Instance != null && Window.Instance.Open)
        Destroy();
    }

    public void Create() {
      mId = GL.GenVertexArray();
    }

    public void Destroy() {
      if (mId != 0) {
        GL.DeleteVertexArray(mId);
        mId = 0;
      }
    }

    public void Bind() {
      GL.BindVertexArray(mId);
    }

    private int mId = 0;

  }

}