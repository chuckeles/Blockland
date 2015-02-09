using LibNoise;
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
    public ChunkGenerator(PriorityQueue<Chunk> chunksToGenerate, PriorityQueue<Chunk> chunksToBuild, int height) {
      mChunksToGenerate = chunksToGenerate;
      mChunksToBuild = chunksToBuild;
      mHeight = height;

      if (mHeightmap == null) {
        mHeightmap = new FastNoise();
        mHeightmap.Seed = (int)(Random.Value * uint.MaxValue);
        mHeightmap.OctaveCount = 4;
        mHeightmap.Frequency = 1 / 128f;

        mDirtHeightmap = new FastNoise();
        mDirtHeightmap.Seed = (int)(Random.Value * uint.MaxValue);
        mDirtHeightmap.OctaveCount = 4;
        mDirtHeightmap.Frequency = 1 / 64f;

        mCaves = new FastRidgedMultifractal();
        mCaves.Seed = (int)(Random.Value * uint.MaxValue);
        mCaves.OctaveCount = 4;
        mCaves.Frequency = 1 / 64f;
      }

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
    }

    /// <summary>
    /// Thread entry point.
    /// </summary>
    private void Start() {
      while (World.Current != null) {
        Chunk chunk;
        lock (mChunksToGenerate) {
          if (mChunksToGenerate.Count <= 0)
            continue;

          chunk = mChunksToGenerate.Dequeue() as Chunk;
        }

        Generate(chunk);

        lock (mChunksToBuild) {
          mChunksToBuild.Enqueue((int)(new Vector3i(chunk.Position.X, chunk.Position.Y - mHeight / 2, chunk.Position.Z).Length * 90), chunk);
        }
      }
    }

    #endregion Methods

    #region Fields

    /// <summary>
    /// Ridged multifractal noise for cave generation.
    /// </summary>
    private static FastRidgedMultifractal mCaves;

    /// <summary>
    /// Perlin noise for dirt layer heightmap generation.
    /// </summary>
    private static FastNoise mDirtHeightmap;

    /// <summary>
    /// Perlin noise for terrain heightmap generation.
    /// </summary>
    private static FastNoise mHeightmap;

    /// <summary>
    /// World height in chunks.
    /// </summary>
    private int mHeight;

    /// <summary>
    /// World's queue of chunks to build.
    /// </summary>
    private PriorityQueue<Chunk> mChunksToBuild;

    /// <summary>
    /// World's queue of chunks to generate.
    /// </summary>
    private PriorityQueue<Chunk> mChunksToGenerate;

    #endregion Fields

  }

}