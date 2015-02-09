using System;

namespace Blockland {

  /// <summary>
  /// 3-dimensional integer vector.
  /// </summary>
  public struct Vector3i {

    #region Constructor

    /// <summary>
    /// Create new vector.
    /// </summary>
    /// <param name="x">X</param>
    /// <param name="y">Y</param>
    /// <param name="z">Z</param>
    public Vector3i(int x, int y, int z) {
      X = x;
      Y = y;
      Z = z;
    }

    #endregion Constructor

    #region Properties

    public float Length {
      get {
        return (float)Math.Sqrt(Math.Pow(X, 2) + Math.Pow(Y, 2) + Math.Pow(Z, 2));
      }
    }

    #endregion Properties

    #region Fields

    /// <summary>
    /// X
    /// </summary>
    public int X;

    /// <summary>
    /// Y
    /// </summary>
    public int Y;

    /// <summary>
    /// Z
    /// </summary>
    public int Z;

    #endregion Fields
  }

}