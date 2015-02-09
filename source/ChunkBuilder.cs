using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace Blockland {

  /// <summary>
  /// Thread that builds chunk geometry from it's blocks.
  /// </summary>
  public class ChunkBuilder {

    #region Contructor

    /// <summary>
    /// Create new chunk builder. Spawns the thread.
    /// </summary>
    /// <param name="chunksToBuild">World's queue of chunks to build</param>
    /// <param name="chunksToBuildMain">World's queue of chunks to build in the main thread</param>
    /// <param name="chunks">List of ready chunks</param>
    public ChunkBuilder(Queue chunksToBuild, Queue chunksToBuildMain, Dictionary<Vector3i, Chunk> chunks) {
      mChunksToBuild = chunksToBuild;
      mChunksToBuildMain = chunksToBuildMain;
      mChunks = chunks;

      Console.WriteLine("Starting the chunk builder thread");
      Thread thread = new Thread(Start);
      thread.Start();
    }

    #endregion Contructor

    #region Methods

    /// <summary>
    /// Build a chunk.
    /// </summary>
    /// <param name="chunk">Chunk to build</param>
    private void Build(Chunk chunk) {
      Console.WriteLine("Building chunk [{0}, {1}, {2}]", chunk.Position.X, chunk.Position.Y, chunk.Position.Z);
    }

    /// <summary>
    /// Thread entry point.
    /// </summary>
    private void Start() {
      Console.WriteLine("Chunk builder thread started");

      while (World.Current != null) {
        if (mChunksToBuild.Count <= 0)
          continue;

        Chunk chunk;
        lock (mChunksToBuild) {
          chunk = mChunksToBuild.Dequeue() as Chunk;
        }

        Build(chunk);

        lock (mChunksToBuildMain) {
          mChunksToBuildMain.Enqueue(chunk);
        }
      }

      Console.WriteLine("Chunk builder thread ending");
    }

    #endregion Methods

    #region Fields

    /// <summary>
    /// World's list of ready chunks.
    /// </summary>
    private Dictionary<Vector3i, Chunk> mChunks;

    /// <summary>
    /// World's queue of chunks to build.
    /// </summary>
    private Queue mChunksToBuild;

    /// <summary>
    /// World's queue of chunks to build in the main thread.
    /// </summary>
    private Queue mChunksToBuildMain;

    #endregion Fields
  }

}