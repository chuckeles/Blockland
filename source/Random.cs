using OpenTK;

namespace Blockland {

  /// <summary>
  /// Helper class for random numbers.
  /// </summary>
  public class Random {

    #region Constructor

    /// <summary>
    /// Constructor.
    /// </summary>
    public Random() {
      mRandom = new System.Random();
    }

    #endregion Constructor

    #region Methods

    /// <summary>
    /// Generate random float number in range.
    /// </summary>
    /// <param name="from">Minimum value</param>
    /// <param name="to">Maximum value</param>
    /// <returns>Random float number</returns>
    public static float Range(float from, float to) {
      return Value * (to - from) + from;
    }

    /// <summary>
    /// Generate random int number in range.
    /// </summary>
    /// <param name="from">Minimum value</param>
    /// <param name="to">Maximum value</param>
    /// <returns>Random int number</returns>
    public static int Range(int from, int to) {
      return (int)(Value * (to - from)) + from;
    }

    #endregion Methods

    #region Properties

    /// <summary>
    /// Get random float value from 0f to 1f.
    /// </summary>
    public static float Value {
      get {
        return (float)mStatic.mRandom.NextDouble();
      }
    }

    #endregion Properties

    #region Fields

    /// <summary>
    /// Static instance.
    /// </summary>
    public static Random mStatic = new Random();

    /// <summary>
    /// System random generator.
    /// </summary>
    private System.Random mRandom;

    #endregion Fields
  }

}