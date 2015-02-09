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

      mHeightmap.Seed = (int)(Random.Value * uint.MaxValue);
      mHeightmap.OctaveCount = 4;
      mHeightmap.Frequency = 1 / 128f;

      mDirtHeightmap.Seed = (int)(Random.Value * uint.MaxValue);
      mDirtHeightmap.OctaveCount = 4;
      mDirtHeightmap.Frequency = 1 / 64f;

      mCaves.Seed = (int)(Random.Value * uint.MaxValue);
      mCaves.OctaveCount = 4;
      mCaves.Frequency = 1 / 64f;

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
      uint generatedBlocks = 0;

      for (int x = 0; x < Chunk.Size; ++x)
        for (int z = 0; z < Chunk.Size; ++z) {

          // get height
          float height = (float)mHeightmap.GetValue(x + chunk.Position.X * Chunk.Size, 0, z + chunk.Position.Z * Chunk.Size);

          // normalize height
          height = (height + 1) / 2;
          height = Chunk.Size * mHeight * (0.4f + height / 2);

          // fill blocks
          for (int y = 0; y < Chunk.Size; ++y) {
            // calculate depth
            float depth = height - (y + chunk.Position.Y * Chunk.Size);

            // cave
            float cave = (float)mCaves.GetValue(
                (x + chunk.Position.X * Chunk.Size),
                ((y + chunk.Position.Y * Chunk.Size) * 1.5f),
                (z + chunk.Position.Z * Chunk.Size));

            float depthClapmed = (depth < 4f ? 4f : depth > Chunk.Size * 2 ? Chunk.Size * 2 : depth) / (Chunk.Size * 2);
            cave *= 0.9f + depthClapmed * 0.1f;

            if (cave > 0.8f)
              continue;

            // block type
            Block.Type type = Block.Type.Stone;

            // get dirt depth
            float dirtDepth = (float)mDirtHeightmap.GetValue(x + chunk.Position.X * Chunk.Size, 0, z + chunk.Position.Z * Chunk.Size);

            // normalize it
            dirtDepth = (dirtDepth + 1) / 2;
            dirtDepth = Chunk.Size / 8 + dirtDepth * Chunk.Size / 2;

            // grass at the top
            if (depth <= 1)
              type = Block.Type.Grass;

            // is this dirt?
            else if (depth < dirtDepth)
              type = Block.Type.Dirt;

            // add block
            if (depth > 0) {
              chunk.Blocks.Add(new Vector3i(x, y, z), new Block(type));
              ++generatedBlocks;
            }
          }
        }

      Console.WriteLine("Generated {0} blocks", generatedBlocks);
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
    /// Ridged multifractal noise for cave generation.
    /// </summary>
    private FastRidgedMultifractal mCaves = new FastRidgedMultifractal();

    /// <summary>
    /// Perlin noise for dirt layer heightmap generation.
    /// </summary>
    private FastNoise mDirtHeightmap = new FastNoise();

    /// <summary>
    /// World height in chunks.
    /// </summary>
    private int mHeight;

    /// <summary>
    /// Perlin noise for terrain heightmap generation.
    /// </summary>
    private FastNoise mHeightmap = new FastNoise();

    /// <summary>
    /// World's queue of chunks to build.
    /// </summary>
    private Queue mChunksToBuild;

    /// <summary>
    /// World's queue of chunks to generate.
    /// </summary>
    private Queue mChunksToGenerate;

    #endregion Fields

  }

}