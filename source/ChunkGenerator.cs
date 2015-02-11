using LibNoise;
using System.Collections.Generic;
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
    private bool Generate(Chunk chunk) {
      uint generatedBlocks = 0;

      // chunk 2D position
      Vector2i position2d = new Vector2i(chunk.Position.X, chunk.Position.Z);

      // world heights
      int[,] worldHeights;
      lock (mWorldHeights) {
        if (mWorldHeights.ContainsKey(position2d))
          worldHeights = mWorldHeights[position2d];
        else {
          worldHeights = new int[Chunk.Size, Chunk.Size];
          mWorldHeights[position2d] = worldHeights;
        }
      }

      // block lights
      float[,] blockLights;
      lock (mWorldLights) {
        if (mWorldLights.ContainsKey(position2d))
          blockLights = mWorldLights[position2d];
        else if (chunk.Position.Y == mHeight - 1) {
          blockLights = new float[Chunk.Size, Chunk.Size];
          mWorldLights[position2d] = blockLights;
        }
        else
          return false;
      }

      float worldHeight = mHeight * Chunk.Size;
      for (int x = 0; x < Chunk.Size; ++x)
        for (int z = 0; z < Chunk.Size; ++z) {

          // get height
          float height = (float)mHeightmap.GetValue(x + chunk.Position.X * Chunk.Size, 0, z + chunk.Position.Z * Chunk.Size);

          // normalize height
          height = (height + 1) / 2;
          height = 0.3f + height * 0.2f;

          // get dirt depth
          float dirtDepth = (float)mDirtHeightmap.GetValue(x + chunk.Position.X * Chunk.Size, 0, z + chunk.Position.Z * Chunk.Size);

          // normalize dirt depth
          dirtDepth = (dirtDepth + 1) / 2;
          dirtDepth = Chunk.Size / 8 + dirtDepth * Chunk.Size;

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
              detail *= (yGlob - worldHeight * height) / (worldHeight * 0.1f);

            if (detail > 0.2f) {
              if (yGlob > worldHeight * height && worldHeights[x, z] > yGlob) {
                worldHeights[x, z] -= 6;
                if (worldHeights[x, z] < yGlob)
                  worldHeights[x, z] = (int)yGlob - 1;
              }

              continue;
            }

            // get or set world height
            float depth;

            if (worldHeights[x, z] == 0 && yGlob > worldHeight * height) {
              worldHeights[x, z] = (int)yGlob;
              depth = 0;
            }
            else
              depth = worldHeights[x, z] - yGlob;

            // cave
            float cave = (float)mCaves.GetValue(
                (x + chunk.Position.X * Chunk.Size),
                ((y + chunk.Position.Y * Chunk.Size) * 1.5f),
                (z + chunk.Position.Z * Chunk.Size));

            float depthClapmed = (depth < 4f ? 4f : depth > Chunk.Size * 2 ? Chunk.Size * 2 : depth) / (Chunk.Size * 2);
            cave *= 0.9f + depthClapmed * 0.1f;

            if (cave > 0.8f)
              continue;

            // adjust light
            float light = blockLights[x, z];
            if (light > 1f) {
              light = 1f;
              blockLights[x, z] = 1f;
            }
            else if (light < 1f)
              blockLights[x, z] += 0.1f;

            // block type
            Block.Type type = Block.Type.Stone;

            if (depth < 1)
              type = Block.Type.Grass;
            else if (depth < dirtDepth + Random.Range(-2, 2))
              type = Block.Type.Dirt;

            // add block
            chunk.Blocks.Add(new Vector3i(x, y, z), new Block(type, 1f - light));
            ++generatedBlocks;
          }
        }

      return true;
    }

    /// <summary>
    /// Thread entry point.
    /// </summary>
    private void Start() {
      Transform camera = State.Current.Camera["Transform"] as Transform;

      while (World.Current != null) {
        if (mChunksToGenerate.Count <= 0) {
          Thread.Sleep(500);
          continue;
        }

        Chunk chunk;
        lock (mChunksToGenerate) {
          if (mChunksToGenerate.Count <= 0)
            continue;

          chunk = mChunksToGenerate.Dequeue() as Chunk;
        }

        Vector2i cameraPosition = new Vector2i((int)(camera.Position.X / (Chunk.Size * Block.Size)), (int)(camera.Position.Z / (Chunk.Size * Block.Size)));
        if (Generate(chunk))
          lock (mChunksToBuild) {
            mChunksToBuild.Enqueue((int)(new Vector3i(chunk.Position.X - cameraPosition.X, chunk.Position.Y - mHeight / 2, chunk.Position.Z - cameraPosition.Y).Length * 120), chunk);
          }
        else
          lock (mChunksToGenerate) {
            mChunksToGenerate.Enqueue((int)(new Vector3i(chunk.Position.X - cameraPosition.X, chunk.Position.Y - mHeight / 2, chunk.Position.Z - cameraPosition.Y).Length * 120), chunk);
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
    /// Dictionary of world's grass levels.
    /// </summary>
    private static Dictionary<Vector2i, int[,]> mWorldHeights = new Dictionary<Vector2i, int[,]>();

    /// <summary>
    /// Dictionary of block lights.
    /// </summary>
    private static Dictionary<Vector2i, float[,]> mWorldLights = new Dictionary<Vector2i, float[,]>();

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