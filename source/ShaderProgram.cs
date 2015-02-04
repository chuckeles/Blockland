using OpenTK;
using OpenTK.Graphics.OpenGL4;
using System;

namespace Blockland {

  public class ShaderProgram {

    public ShaderProgram() {
      Create();
    }

    ~ShaderProgram() {
      if (Window.Instance != null && Window.Instance.Open)
        Destroy();
    }

    public void Create() {
      mId = GL.CreateProgram();
    }

    public void Destroy() {
      if (mId != 0) {
        GL.DeleteProgram(mId);
        mId = 0;
      }
    }

    public void Attach(Shader shader) {
      GL.AttachShader(mId, shader.Id);
    }

    public void Link() {
      GL.LinkProgram(mId);

      if (!Linked)
        throw new Exception("Shader program " + mId + " failed to link");
    }

    public void Use() {
      GL.UseProgram(mId);
    }

    public void UniformMatrix4(string uniformName, ref Matrix4 matrix) {
      int uniform = GL.GetUniformLocation(mId, uniformName);
      GL.UniformMatrix4(uniform, false, ref matrix);
    }

    public void Attribute(string name, int size, int stride, int offset) {
      int attrib = GL.GetAttribLocation(mId, name);
      GL.VertexAttribPointer(attrib, size, VertexAttribPointerType.Float, false, stride, offset);
      GL.EnableVertexAttribArray(attrib);
    }

    public bool Linked {
      get {
        if (mId == 0)
          return false;

        int status;
        GL.GetProgram(mId, GetProgramParameterName.LinkStatus, out status);
        return status == 1;
      }
    }

    public int Id {
      get {
        return mId;
      }
    }

    private int mId = 0;

  }

}