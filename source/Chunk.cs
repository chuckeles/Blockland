using System.Collections.Generic;

namespace Blockland {

  public struct Block {

    public struct Position {
      public int X;
      public int Y;
      public int Z;

      public Position(int x, int y, int z) {
        X = x;
        Y = y;
        Z = z;
      }
    }

  }

  public class Chunk
    : Component {

    public Chunk()
      : base("Chunk") {
      mBlocks = new Dictionary<Block.Position, Block>();

      mBlocks.Add(new Block.Position(0, 0, 0), new Block());
    }

    private Dictionary<Block.Position, Block> mBlocks;

  }

}