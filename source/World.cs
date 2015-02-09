using System;
using System.Collections;
using System.Collections.Generic;

namespace Blockland {

  /// <summary>
  /// The world is a container for chunks. It spawns worker threads to manage the chunks.
  /// </summary>
  public class World {

    #region Constructor

    /// <summary>
    /// Create new world.
    /// </summary>
    /// <exception cref="Exception">When there already is a world</exception>
    public World() {
      if (mCurrent != null)
        throw new Exception("The world already exists");

      mCurrent = this;
    }

    #endregion Constructor

    #region Methods

    /// <summary>
    /// Start creating chunks and building the world.
    /// </summary>
    /// <param name="renderDistance">How many chunks to build around the player</param>
    /// <param name="height">World height in chunks</param>
    public void Create(int renderDistance, int height) {
      Console.WriteLine("Creating the world");

      mHeight = height;

      mAllocator = new ChunkAllocator(mChunksToGenerate, renderDistance, height);
      mGenerator = new ChunkGenerator(mChunksToGenerate, mChunksToBuild, height);
    }

    /// <summary>
    /// Destroy the world.
    /// </summary>
    public void Destroy() {
      mCurrent = null;
    }

    #endregion Methods

    #region Properties

    /// <summary>
    /// Get the current world instance.
    /// </summary>
    public static World Current {
      get {
        return mCurrent;
      }
    }

    /// <summary>
    /// Get world height.
    /// </summary>
    public int Height {
      get {
        return mHeight;
      }
    }

    /// <summary>
    /// Get chunks to build.
    /// </summary>
    public Queue ChunksToBuild {
      get {
        return mChunksToBuild;
      }
    }

    /// <summary>
    /// Get chunks to generate.
    /// </summary>
    public Queue ChunksToGenerate {
      get {
        return mChunksToGenerate;
      }
    }

    #endregion Properties

    #region Fields

    /// <summary>
    /// Static instance.
    /// </summary>
    private static World mCurrent;

    /// <summary>
    /// Chunk allocator thread.
    /// </summary>
    private ChunkAllocator mAllocator;

    /// <summary>
    /// Chunk generator thread.
    /// </summary>
    private ChunkGenerator mGenerator;

    /// <summary>
    /// World height in chunks.
    /// </summary>
    private int mHeight = 0;

    /// <summary>
    /// Dictionary of loaded and ready chunks.
    /// </summary>
    private Dictionary<Vector3i, Chunk> mChunks = new Dictionary<Vector3i, Chunk>();

    /// <summary>
    /// Queue of chunks waiting to be built.
    /// </summary>
    private Queue mChunksToBuild = new Queue();

    /// <summary>
    /// Queue of chunks waiting to be generated.
    /// </summary>
    private Queue mChunksToGenerate = new Queue();

    #endregion Fields

  }

}