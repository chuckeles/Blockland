using OpenTK.Graphics.OpenGL4;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Blockland {

  public class Texture
    : IResource {

    #region Constructor

    /// <summary>
    /// Create a new texture.
    /// </summary>
    /// <param name="name">Resource name</param>
    /// <exception cref="ArgumentNullException">When empty name is supplied</exception>
    public Texture(string name) {
      if (name == "")
        throw new ArgumentNullException("Resource needs a name");

      mName = name;
    }

    /// <summary>
    /// Create a new texture and load it from the file.
    /// </summary>
    /// <param name="name">Resource name</param>
    /// <param name="fileName">Source file</param>
    /// <exception cref="Exception">When empty name is supplied</exception>
    public Texture(string name, string fileName) :
      this(name) {

      Load(fileName);
    }

    /// <summary>
    /// Destructor.
    /// </summary>
    ~Texture() {
      if (Window.Instance != null && Window.Instance.Open)
        Dispose();
    }

    #endregion Constructor

    #region Methods

    /// <summary>
    /// Bind the texture.
    /// </summary>
    public void Bind() {
      GL.BindTexture(TextureTarget.Texture2D, mId);
    }

    /// <summary>
    /// Destroy the texture and free it from memory.
    /// </summary>
    public void Destroy() {
      if (mId != 0) {
        GL.DeleteTexture(mId);

        mId = 0;
      }
    }

    /// <summary>
    /// Free the texture from memory.
    /// </summary>
    /// <seealso cref="Destroy" />
    public void Dispose() {
      Destroy();
    }

    /// <summary>
    /// Load the resource. Also binds the texture.
    /// </summary>
    /// <param name="fileName">Source file name</param>
    public void Load(string fileName) {
      if (mLoaded)
        return;

      if (!File.Exists(fileName))
        throw new FileNotFoundException("Source file doesn't exist");

      Bitmap bitmap = new Bitmap(fileName);
      BitmapData data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

      mId = GL.GenTexture();
      GL.BindTexture(TextureTarget.Texture2D, mId);
      GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0, OpenTK.Graphics.OpenGL4.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);

      bitmap.UnlockBits(data);

      mLoaded = true;
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

    /// <summary>
    /// Check if the resource is loaded.
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
    /// OpenGL id.
    /// </summary>
    private int mId = 0;

    /// <summary>
    /// Whether the resource is loaded.
    /// </summary>
    private bool mLoaded = false;

    /// <summary>
    /// The name of the resource.
    /// </summary>
    private string mName;

    #endregion Fields

  }

}