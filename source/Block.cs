namespace Blockland {

  /// <summary>
  /// Stores information about the block.
  /// </summary>
  public struct Block {

    /// <summary>
    /// Block type.
    /// </summary>
    public enum Type {
      Grass = 1,
      Dirt = 2,
      Stone = 3
    }

    /// <summary>
    /// Create a new block.
    /// </summary>
    /// <param name="type">Block type</param>
    public Block(Type type) {
      BlockType = type;
    }

    public static float Size = 2f;
    public Type BlockType;
  }

}