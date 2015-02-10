using System;

namespace Blockland {

  /// <summary>
  /// 2-dimensional integer vector.
  /// </summary>
  public struct Vector2i {

    #region Constructor

    /// <summary>
    /// Create new vector.
    /// </summary>
    /// <param name="x">X</param>
    /// <param name="y">Y</param>
    public Vector2i(int x, int y) {
      X = x;
      Y = y;
    }

    #endregion Constructor

    #region Properties

    public float Length {
      get {
        return (float)Math.Sqrt(Math.Pow(X, 2) + Math.Pow(Y, 2));
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

    #endregion Fields
  }

}