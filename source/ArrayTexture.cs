using OpenTK.Graphics.OpenGL4;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Blockland {

  public class ArrayTexture
    : IResource {

    #region Constructor

    /// <summary>
    /// Create a new array texture.
    /// </summary>
    /// <param name="name">Resource name</param>
    /// <exception cref="ArgumentNullException">When empty name is supplied</exception>
    public ArrayTexture(string name) {
      if (name == "")
        throw new ArgumentNullException("Resource needs a name");

      mName = name;
    }

    /// <summary>
    /// Create a new array texture and allocate storage for the layers.
    /// </summary>
    /// <param name="name">Resource name</param>
    /// <param name="width">Texture width</param>
    /// <param name="height">Texture height</param>
    /// <param name="layers">Number of layers</param>
    public ArrayTexture(string name, int width, int height, int layers)
      : this(name) {
      Create(width, height, layers);
    }

    /// <summary>
    /// Destructor.
    /// </summary>
    //~ArrayTexture() {
    //  if (Window.Instance != null && Window.Instance.Open)
    //    Dispose();
    //}

    #endregion Constructor

    #region Methods

    /// <summary>
    /// Use the array texture.
    /// </summary>
    /// <param name="textureUnit">Target texture unit</param>
    public static void Use(int textureUnit = 0) {
      GL.ActiveTexture(mUnits[textureUnit]);
    }

    /// <summary>
    /// Bind the array texture.
    /// </summary>
    public void Bind() {
      GL.BindTexture(TextureTarget.Texture2DArray, mId);
    }

    /// <summary>
    /// Create a new array texture and alocate texture storage.
    /// </summary>
    /// <param name="width">Texture width</param>
    /// <param name="height">Texture height</param>
    /// <param name="layers">Number of layers</param>
    public void Create(int width, int height, int layers) {
      mId = GL.GenTexture();

      GL.BindTexture(TextureTarget.Texture2DArray, mId);
      GL.TexStorage3D(TextureTarget3d.Texture2DArray, 1, SizedInternalFormat.Rgba8, width, height, layers);

      GL.TexParameter(TextureTarget.Texture2DArray, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
      GL.TexParameter(TextureTarget.Texture2DArray, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

      // TODO: Add mipmaps

      GL.TexParameter(TextureTarget.Texture2DArray, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
      GL.TexParameter(TextureTarget.Texture2DArray, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
    }

    /// <summary>
    /// Destroy the array texture and free it from memory.
    /// </summary>
    public void Destroy() {
      if (mId != 0) {
        GL.DeleteTexture(mId);

        mId = 0;
      }
    }

    /// <summary>
    /// Free the array texture from memory.
    /// </summary>
    /// <seealso cref="Destroy" />
    public void Dispose() {
      Destroy();
    }

    /// <summary>
    /// Load next layer to the array texture. Doesn't bind the texture automatically.
    /// </summary>
    /// <param name="fileName">Source file</param>
    public void Load(string fileName) {
      Load(fileName, (int)mLoadedLayers, false);
    }

    /// <summary>
    /// Load one layer to the array texture.
    /// </summary>
    /// <param name="fileName">Source file name</param>
    /// <param name="bind">Whether to automatically bind the texture</param>
    public void Load(string fileName, int layer, bool bind = false) {
      if (bind)
        Bind();

      if (!File.Exists(fileName))
        throw new FileNotFoundException("Source file doesn't exist");

      Bitmap bitmap = new Bitmap(fileName);
      BitmapData data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

      GL.TexSubImage3D(TextureTarget.Texture2DArray, 0, 0, 0, layer, data.Width, data.Height, 1, OpenTK.Graphics.OpenGL4.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);

      bitmap.UnlockBits(data);

      ++mLoadedLayers;
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
        return mLoadedLayers >= mLayers;
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
    /// Array of texture units for better indexing.
    /// </summary>
    private static TextureUnit[] mUnits = {
      TextureUnit.Texture0,
      TextureUnit.Texture1,
      TextureUnit.Texture2,
      TextureUnit.Texture3,
      TextureUnit.Texture4,
      TextureUnit.Texture5,
      TextureUnit.Texture6,
      TextureUnit.Texture7
    };

    /// <summary>
    /// OpenGL id.
    /// </summary>
    private int mId = 0;

    /// <summary>
    /// Number of layers.
    /// </summary>
    private uint mLayers = 0;

    /// <summary>
    /// Number of loaded layers.
    /// </summary>
    private uint mLoadedLayers = 0;

    /// <summary>
    /// The name of the resource.
    /// </summary>
    private string mName;

    #endregion Fields

  }

}