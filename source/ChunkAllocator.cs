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
      // set bounds
      int right = (int)((mRenderDistance / 2f) + 0.5f);
      int left = -mRenderDistance / 2;
      int front = (int)((mRenderDistance / 2f) + 0.5f);
      int back = -mRenderDistance / 2;

      // generate chunks
      lock (mChunksToGenerate) {
        for (int x = left, xMax = right; x <= xMax; ++x)
          for (int z = back, zMax = front; z <= zMax; ++z)
            for (int y = 0; y < mHeight; ++y) {
              Chunk chunk = new Chunk(x, y, z);
              mChunksToGenerate.Enqueue((int)(new Vector3i(chunk.Position.X, mHeight - chunk.Position.Y, chunk.Position.Z).Length * 90), chunk);
            }
      }

      // set camera position
      Transform camera = State.Current.Camera["Transform"] as Transform;
      mCameraPosition = new Vector2i((int)(camera.Position.X / (Chunk.Size * Block.Size)), (int)(camera.Position.Z / (Chunk.Size * Block.Size)));

      while (World.Current != null) {
        // check camera position
        Vector2i newPosition = new Vector2i((int)(camera.Position.X / (Chunk.Size * Block.Size)), (int)(camera.Position.Z / (Chunk.Size * Block.Size)));

        if (newPosition.X == mCameraPosition.X && newPosition.Y == mCameraPosition.Y) {
          Thread.Sleep(1000);
          continue;
        }

        // get delta movement
        int deltaX = newPosition.X - mCameraPosition.X;
        int deltaZ = newPosition.Y - mCameraPosition.Y;

        // allocate new chunks
        lock (mChunksToGenerate) {
          if (deltaX > 0) {
            // new chunks on the right
            int x = right + 1;
            ++right;

            for (int z = back, zMax = front; z <= zMax; ++z)
              for (int y = 0; y < mHeight; ++y) {
                Chunk chunk = new Chunk(x, y, z);
                mChunksToGenerate.Enqueue((int)(new Vector3i(chunk.Position.X - newPosition.X, mHeight - chunk.Position.Y, chunk.Position.Z - newPosition.Y).Length * 90), chunk);
              }
          }
          else if (deltaX < 0) {
            // new chunks on the left
            int x = left - 1;
            --left;

            for (int z = back, zMax = front; z <= zMax; ++z)
              for (int y = 0; y < mHeight; ++y) {
                Chunk chunk = new Chunk(x, y, z);
                mChunksToGenerate.Enqueue((int)(new Vector3i(chunk.Position.X - newPosition.X, mHeight - chunk.Position.Y, chunk.Position.Z - newPosition.Y).Length * 90), chunk);
              }
          }

          if (deltaZ > 0) {
            // new chunks on the front
            int z = front + 1;
            ++front;

            for (int x = left, xMax = right; x <= xMax; ++x)
              for (int y = 0; y < mHeight; ++y) {
                Chunk chunk = new Chunk(x, y, z);
                mChunksToGenerate.Enqueue((int)(new Vector3i(chunk.Position.X - newPosition.X, mHeight - chunk.Position.Y, chunk.Position.Z - newPosition.Y).Length * 90), chunk);
              }
          }
          else if (deltaZ < 0) {
            // new chunks on the back
            int z = back - 1;
            --back;

            for (int x = left, xMax = right; x <= xMax; ++x)
              for (int y = 0; y < mHeight; ++y) {
                Chunk chunk = new Chunk(x, y, z);
                mChunksToGenerate.Enqueue((int)(new Vector3i(chunk.Position.X - newPosition.X, mHeight - chunk.Position.Y, chunk.Position.Z - newPosition.Y).Length * 90), chunk);
              }
          }
        }

        // store camera position
        mCameraPosition = newPosition;
      }
    }

    #endregion Methods

    #region Fields

    /// <summary>
    /// Stored camera position.
    /// </summary>
    private Vector2i mCameraPosition;

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