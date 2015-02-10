namespace Blockland {

  /// <summary>
  /// Stores information about the block.
  /// </summary>
  public struct Block {

    #region Types

    /// <summary>
    /// The side of a block.
    /// </summary>
    public enum Side {
      Front,
      Back,
      Right,
      Left,
      Top,
      Bottom
    }

    /// <summary>
    /// Block type.
    /// </summary>
    public enum Type {
      Grass = 1,
      Dirt = 2,
      Stone = 3
    }

    #endregion Types

    #region Constructor

    /// <summary>
    /// Create a new block.
    /// </summary>
    /// <param name="type">Block type</param>
    /// <param name="light">Block light</param>
    public Block(Type type, float light = 0) {
      BlockType = type;
      Light = light;
    }

    #endregion Constructor

    #region Fields

    /// <summary>
    /// Standard block size.
    /// </summary>
    public static float Size = 2f;

    /// <summary>
    /// The type of the block.
    /// </summary>
    public Type BlockType;

    /// <summary>
    /// Block light.
    /// </summary>
    public float Light;

    #endregion Fields

  }

}