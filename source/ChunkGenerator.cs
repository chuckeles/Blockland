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

        mTerrain = new FastNoise();
        mTerrain.Seed = (int)(Random.Value * uint.MaxValue);
        mTerrain.OctaveCount = 4;
        mTerrain.Frequency = 1 / 86f;

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

      float worldHeight = mHeight * Chunk.Size;
      for (int x = 0; x < Chunk.Size; ++x)
        for (int z = 0; z < Chunk.Size; ++z) {

          // get height
          float height = (float)mHeightmap.GetValue(x + chunk.Position.X * Chunk.Size, 0, z + chunk.Position.Z * Chunk.Size);

          // normalize height
          height = (height + 1) / 2;
          height = 0.3f + height * 0.2f;

          // fill blocks
          for (int y = Chunk.Size - 1; y >= 0; --y) {
            // get detail value
            float detail = (float)mTerrain.GetValue(x + chunk.Position.X * Chunk.Size, y + chunk.Position.Y * Chunk.Size, z + chunk.Position.Z * Chunk.Size);

            // normalize it
            detail = (detail + 1) / 2;

            // apply gradient
            float yGlob = y + chunk.Position.Y * Chunk.Size;

            if (yGlob < worldHeight * height)
              detail = 0;
            else
              detail *= (yGlob - worldHeight * height) / (worldHeight * 0.2f) * 2;

            // calculate depth
            float depth = height * worldHeight - (y + chunk.Position.Y * Chunk.Size);

            // cave
            float cave = (float)mCaves.GetValue(
                (x + chunk.Position.X * Chunk.Size),
                ((y + chunk.Position.Y * Chunk.Size) * 1.5f),
                (z + chunk.Position.Z * Chunk.Size));

            float depthClapmed = (depth < 4f ? 4f : depth > Chunk.Size * 2 ? Chunk.Size * 2 : depth) / (Chunk.Size * 2);
            cave *= 0.9f + depthClapmed * 0.1f;

            if (cave > 0.8f)
              continue;

            // add block
            if (detail < 0.2f) {
              chunk.Blocks.Add(new Vector3i(x, y, z), new Block(Block.Type.Stone));
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
        if (mChunksToGenerate.Count <= 0) {
          Thread.Sleep(1000);
          continue;
        }

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
    /// Perlin noise for terrain detail generation.
    /// </summary>
    private static FastNoise mTerrain;

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