using System.Collections;
using System.Threading;

namespace Blockland {

  /// <summary>
  /// Thread that allocates and deallocates threads based on player position.
  /// </summary>
  public class ChunkAllocator {

    #region Constructor

    /// <summary>
    /// Create new chunk allocator. Starts the thread.
    /// </summary>
    /// <param name="chunksToGenerate">World's queue of chunks to generate</param>
    /// <param name="renderDistance">Chunk render distance</param>
    /// <param name="height">World's height in chunks</param>
    public ChunkAllocator(PriorityQueue<Chunk> chunksToGenerate, int renderDistance, int height) {
      mChunksToGenerate = chunksToGenerate;
      mRenderDistance = renderDistance;
      mHeight = height;

      Thread thread = new Thread(Start);
      thread.Start();
    }

    #endregion Constructor

    #region Methods

    /// <summary>
    /// Thread entry point.
    /// </summary>
    private void Start() {
      lock (mChunksToGenerate) {
        for (int x = -mRenderDistance / 2, xMax = (int)((mRenderDistance / 2f) + 0.5f); x < xMax; ++x)
          for (int z = -mRenderDistance / 2, zMax = (int)((mRenderDistance / 2f) + 0.5f); z < zMax; ++z)
            for (int y = 0; y < mHeight; ++y) {
              Chunk chunk = new Chunk(x, y, z);
              mChunksToGenerate.Enqueue((int)(new Vector3i(chunk.Position.X, mHeight - chunk.Position.Y, chunk.Position.Z).Length * 90), chunk);
            }
      }
    }

    #endregion Methods

    #region Fields

    /// <summary>
    /// World height in chunks.
    /// </summary>
    private int mHeight;

    /// <summary>
    /// Queue of chunks to generate.
    /// </summary>
    private PriorityQueue<Chunk> mChunksToGenerate;

    /// <summary>
    /// Render distance.
    /// </summary>
    private int mRenderDistance;

    #endregion Fields

  }

}