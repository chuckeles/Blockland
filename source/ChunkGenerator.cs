using LibNoise;
using System;
using System.Collections;
using System.Threading;

namespace Blockland {

  /// <summary>
  /// Thread that generates chunks.
  /// </summary>
  public class ChunkGenerator {

    #region Constructor

    /// <summary>
    /// Create new chunk generator. Spawns the thread.
    /// </summary>
    /// <param name="chunksToGenerate">World's queue of chunks to generate</param>
    /// <param name="chunksToBuild">World's queue of chunks to build</param>
    /// <param name="height">World height in chunks</param>
    public ChunkGenerator(Queue chunksToGenerate, Queue chunksToBuild, int height) {
      mChunksToGenerate = chunksToGenerate;
      mChunksToBuild = chunksToBuild;
      mHeight = height;

      mPerlin.Seed = (int)(Random.Value * uint.MaxValue);
      mPerlin.OctaveCount = 4;
      mPerlin.Frequency = 1 / 128f;

      Console.WriteLine("Starting the chunk generator thread");
      Thread thread = new Thread(Start);
      thread.Start();
    }

    #endregion Constructor

    #region Methods

    /// <summary>
    /// Generate a chunk.
    /// </summary>
    /// <param name="chunk">Chunk to generate</param>
    private void Generate(Chunk chunk) {
      Console.WriteLine("Generating chunk [{0}, {1}, {2}]", chunk.Position.X, chunk.Position.Y, chunk.Position.Z);

    }

    /// <summary>
    /// Thread entry point.
    /// </summary>
    private void Start() {
      Console.WriteLine("Chunk generator thread started");

      while (World.Current != null) {
        if (mChunksToGenerate.Count <= 0)
          continue;

        Chunk chunk;
        lock (mChunksToGenerate) {
          chunk = mChunksToGenerate.Dequeue() as Chunk;
        }

        Generate(chunk);

        lock (mChunksToBuild) {
          mChunksToBuild.Enqueue(chunk);
        }
      }

      Console.WriteLine("Chunk generator thread ending");
    }

    #endregion Methods

    #region Fields

    /// <summary>
    /// World height in chunks.
    /// </summary>
    private int mHeight;

    /// <summary>
    /// World's queue of chunks to build.
    /// </summary>
    private Queue mChunksToBuild;

    /// <summary>
    /// World's queue of chunks to generate.
    /// </summary>
    private Queue mChunksToGenerate;

    /// <summary>
    /// Perlin noise for terrain generation.
    /// </summary>
    private FastNoise mPerlin = new FastNoise();

    #endregion Fields

  }

}