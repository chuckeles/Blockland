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
    }

    #endregion Methods

    #region Properties

    /// <summary>
    /// Get the current world instance.
    /// </summary>
    public World Current {
      get {
        return mCurrent;
      }
    }

    #endregion Properties

    #region Fields

    /// <summary>
    /// Static instance.
    /// </summary>
    private World mCurrent;

    /// <summary>
    /// Dictionary of loaded and ready chunks.
    /// </summary>
    private Dictionary<Vector3i, Chunk> mChunks = new Dictionary<Vector3i, Chunk>();

    /// <summary>
    /// List of chunks waiting to be built.
    /// </summary>
    private ArrayList mChunksToBuild = new ArrayList();

    /// <summary>
    /// List of chunks waiting to be generated.
    /// </summary>
    private ArrayList mChunksToGenerate = new ArrayList();

    #endregion Fields

  }

}