using OpenTK.Graphics.OpenGL4;
using System;
using System.IO;

namespace Blockland {

  public class Shader {

    public Shader(ShaderType type) {
      mType = type;
    }

    public Shader(ShaderType type, string fileName)
      : this(type) {
      Create(fileName);
    }

    ~Shader() {
      if (Window.Instance != null && Window.Instance.Open)
        Destroy();
    }

    public void Create(string fileName) {
      Load(fileName);
      Compile();
    }

    public void Destroy() {
      if (mId != 0) {
        GL.DeleteShader(mId);
        mId = 0;
      }
    }

    public void Load(string fileName) {
      StreamReader file = new StreamReader(fileName);
      string code = file.ReadToEnd();
      file.Close();

      mId = GL.CreateShader(mType);
      GL.ShaderSource(mId, code);
    }

    public void Compile() {
      GL.CompileShader(mId);

      if (!Compiled)
        throw new Exception("Shader " + mId + " failed to compile");
    }

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

    public int Id {
      get {
        return mId;
      }
    }

    private ShaderType mType;
    private int mId = 0;

  }

}