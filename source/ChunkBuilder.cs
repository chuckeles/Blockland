using System.Collections.Generic;
using System.Threading;

namespace Blockland {

  /// <summary>
  /// Thread that builds chunk geometry from it's blocks.
  /// </summary>
  public class ChunkBuilder {

    #region Types

    /// <summary>
    /// Chunk assiciated with vertex and element arrays.
    /// </summary>
    public struct BuiltChunk {

      public BuiltChunk(Chunk chunk, float[] vertices, uint[] elements) {
        Chunk = chunk;
        Vertices = vertices;
        Elements = elements;
      }

      public uint[] Elements;
      public Chunk Chunk;
      public float[] Vertices;
    }

    #endregion Types

    #region Contructor

    /// <summary>
    /// Create new chunk builder. Spawns the thread.
    /// </summary>
    /// <param name="chunksToBuild">World's queue of chunks to build</param>
    /// <param name="chunksToBuildMain">World's queue of chunks to build in the main thread</param>
    /// <param name="chunks">List of ready chunks</param>
    public ChunkBuilder(PriorityQueue<Chunk> chunksToBuild, Queue<BuiltChunk> chunksToBuildMain, Dictionary<Vector3i, Chunk> chunks) {
      mChunksToBuild = chunksToBuild;
      mChunksToBuildMain = chunksToBuildMain;
      mChunks = chunks;

      Thread thread = new Thread(Start);
      thread.Start();
    }

    #endregion Contructor

    #region Methods

    /// <summary>
    /// Add a side of a block to the arrays.
    /// </summary>
    /// <param name="x">Block x</param>
    /// <param name="y">Block y</param>
    /// <param name="z">Block z</param>
    /// <param name="side">Which side to add</param>
    /// <param name="texture">Which texture to use</param>
    /// <param name="vertices">Vertex array</param>
    /// <param name="elements">Element array</param>
    /// <param name="elementOffset">
    /// How much to offset the side's elements (how much elements there already are)
    /// </param>
    /// <param name="light">Light value to add to the side</param>
    public void AddSide(float x, float y, float z, Block.Side side, float texture, List<float> vertices, List<uint> elements, uint elementOffset, float light) {
      switch (side) {
        case Block.Side.Front:
          float[] frontVertices = {
                                  x, y, z+Block.Size,                       0f, 0f, 1f, 0f, 0f, texture, light,
                                  x+Block.Size, y, z+Block.Size,            0f, 0f, 1f, 1f, 0f, texture, light,
                                  x+Block.Size, y+Block.Size, z+Block.Size, 0f, 0f, 1f, 1f, 1f, texture, light,
                                  x, y+Block.Size, z+Block.Size,            0f, 0f, 1f, 0f, 1f, texture, light
                                };
          vertices.AddRange(frontVertices);

          uint[] frontElements = {
                                 elementOffset, elementOffset+1, elementOffset+2,
                                 elementOffset, elementOffset+2, elementOffset+3
                               };

          elements.AddRange(frontElements);
          break;

        case Block.Side.Back:
          float[] backVertices = {
                                  x+Block.Size, y, z,            0f, 0f, -1f, 0f, 0f, texture, light,
                                  x, y, z,                       0f, 0f, -1f, 1f, 0f, texture, light,
                                  x, y+Block.Size, z,            0f, 0f, -1f, 1f, 1f, texture, light,
                                  x+Block.Size, y+Block.Size, z, 0f, 0f, -1f, 0f, 1f, texture, light
                                };
          vertices.AddRange(backVertices);

          uint[] backElements = {
                                 elementOffset, elementOffset+1, elementOffset+2,
                                 elementOffset, elementOffset+2, elementOffset+3
                               };

          elements.AddRange(backElements);
          break;

        case Block.Side.Right:
          float[] rightVertices = {
                                  x+Block.Size, y, z+Block.Size,            1f, 0f, 0f, 0f, 0f, texture, light,
                                  x+Block.Size, y, z,                       1f, 0f, 0f, 1f, 0f, texture, light,
                                  x+Block.Size, y+Block.Size, z,            1f, 0f, 0f, 1f, 1f, texture, light,
                                  x+Block.Size, y+Block.Size, z+Block.Size, 1f, 0f, 0f, 0f, 1f, texture, light
                                };
          vertices.AddRange(rightVertices);

          uint[] rightElements = {
                                 elementOffset, elementOffset+1, elementOffset+2,
                                 elementOffset, elementOffset+2, elementOffset+3
                               };

          elements.AddRange(rightElements);
          break;

        case Block.Side.Left:
          float[] leftVertices = {
                                  x, y, z,                       -1f, 0f, 0f, 0f, 0f, texture, light,
                                  x, y, z+Block.Size,            -1f, 0f, 0f, 1f, 0f, texture, light,
                                  x, y+Block.Size, z+Block.Size, -1f, 0f, 0f, 1f, 1f, texture, light,
                                  x, y+Block.Size, z,            -1f, 0f, 0f, 0f, 1f, texture, light
                                };
          vertices.AddRange(leftVertices);

          uint[] leftElements = {
                                 elementOffset, elementOffset+1, elementOffset+2,
                                 elementOffset, elementOffset+2, elementOffset+3
                               };

          elements.AddRange(leftElements);
          break;

        case Block.Side.Top:
          float[] topVertices = {
                                  x, y+Block.Size, z+Block.Size,            0f, 1f, 0f, 0f, 0f, texture, light,
                                  x+Block.Size, y+Block.Size, z+Block.Size, 0f, 1f, 0f, 1f, 0f, texture, light,
                                  x+Block.Size, y+Block.Size, z,            0f, 1f, 0f, 1f, 1f, texture, light,
                                  x, y+Block.Size, z,                       0f, 1f, 0f, 0f, 1f, texture, light
                                };
          vertices.AddRange(topVertices);

          uint[] topElements = {
                                 elementOffset, elementOffset+1, elementOffset+2,
                                 elementOffset, elementOffset+2, elementOffset+3
                               };

          elements.AddRange(topElements);
          break;

        case Block.Side.Bottom:
          float[] bottomVertices = {
                                  x+Block.Size, y, z+Block.Size, 0f, -1f, 0f, 0f, 0f, texture, light,
                                  x, y, z+Block.Size,            0f, -1f, 0f, 1f, 0f, texture, light,
                                  x, y, z,                       0f, -1f, 0f, 1f, 1f, texture, light,
                                  x+Block.Size, y, z,            0f, -1f, 0f, 0f, 1f, texture, light
                                };
          vertices.AddRange(bottomVertices);

          uint[] bottomElements = {
                                 elementOffset, elementOffset+1, elementOffset+2,
                                 elementOffset, elementOffset+2, elementOffset+3
                               };

          elements.AddRange(bottomElements);
          break;

        default:
          break;
      }
    }

    /// <summary>
    /// Build a chunk.
    /// </summary>
    /// <param name="chunk">Chunk to build</param>
    /// <param name="vertices">Vertex array</param>
    /// <param name="elements">Element array</param>
    private void Build(Chunk chunk, out float[] vertices, out uint[] elements) {
      // list of vertices
      List<float> vertexData = new List<float>();
      // list of elements
      List<uint> elementData = new List<uint>();

      // neighbor chunks
      Chunk chunkFront;
      Chunk chunkBack;
      Chunk chunkRight;
      Chunk chunkLeft;
      Chunk chunkTop;
      Chunk chunkBottom;

      lock (mChunks) {
        mChunks.TryGetValue(new Vector3i(chunk.Position.X, chunk.Position.Y, chunk.Position.Z + 1), out chunkFront);
        mChunks.TryGetValue(new Vector3i(chunk.Position.X, chunk.Position.Y, chunk.Position.Z - 1), out chunkBack);
        mChunks.TryGetValue(new Vector3i(chunk.Position.X + 1, chunk.Position.Y, chunk.Position.Z), out chunkRight);
        mChunks.TryGetValue(new Vector3i(chunk.Position.X - 1, chunk.Position.Y, chunk.Position.Z), out chunkLeft);
        mChunks.TryGetValue(new Vector3i(chunk.Position.X, chunk.Position.Y + 1, chunk.Position.Z), out chunkTop);
        mChunks.TryGetValue(new Vector3i(chunk.Position.X, chunk.Position.Y - 1, chunk.Position.Z), out chunkBottom);
      }

      bool[,,] blockCache = new bool[Chunk.Size + 2, Chunk.Size + 2, Chunk.Size + 2];

      // loop through blocks
      uint count = 0;
      foreach (var blockPair in chunk.Blocks) {
        Vector3i position = blockPair.Key;
        Block block = blockPair.Value;

        // local position
        float x = position.X * Block.Size;
        float y = position.Y * Block.Size;
        float z = position.Z * Block.Size;

        // sides
        bool front = !blockCache[position.X + 1, position.Y + 1, position.Z + 2] && !chunk.Blocks.ContainsKey(new Vector3i(position.X, position.Y, position.Z + 1));
        bool back = !blockCache[position.X + 1, position.Y + 1, position.Z] && !chunk.Blocks.ContainsKey(new Vector3i(position.X, position.Y, position.Z - 1));
        bool right = !blockCache[position.X + 2, position.Y + 1, position.Z + 1] && !chunk.Blocks.ContainsKey(new Vector3i(position.X + 1, position.Y, position.Z));
        bool left = !blockCache[position.X, position.Y + 1, position.Z + 1] && !chunk.Blocks.ContainsKey(new Vector3i(position.X - 1, position.Y, position.Z));
        bool top = !blockCache[position.X + 1, position.Y + 2, position.Z + 1] && !chunk.Blocks.ContainsKey(new Vector3i(position.X, position.Y + 1, position.Z));
        bool bottom = !blockCache[position.X + 1, position.Y, position.Z + 1] && !chunk.Blocks.ContainsKey(new Vector3i(position.X, position.Y - 1, position.Z));

        // update cache
        blockCache[position.X + 1, position.Y + 1, position.Z + 1] = true;
        if (!front)
          blockCache[position.X + 1, position.Y + 1, position.Z + 2] = true;
        if (!back)
          blockCache[position.X + 1, position.Y + 1, position.Z] = true;
        if (!right)
          blockCache[position.X + 2, position.Y + 1, position.Z + 1] = true;
        if (!left)
          blockCache[position.X, position.Y + 1, position.Z + 1] = true;
        if (!top)
          blockCache[position.X + 1, position.Y + 2, position.Z + 1] = true;
        if (!bottom)
          blockCache[position.X + 1, position.Y, position.Z + 1] = true;

        // consider neighbor chunks

        // front
        if (position.Z == Chunk.Size - 1) {
          if (chunkFront != null) {
            if (chunkFront.Blocks.ContainsKey(new Vector3i(position.X, position.Y, 0)))
              front = false;
          }
          else if (chunkFront == null)
            front = mWorldBoundaries;
        }

        // back
        if (position.Z == 0) {
          if (chunkBack != null) {
            if (chunkBack.Blocks.ContainsKey(new Vector3i(position.X, position.Y, Chunk.Size - 1)))
              back = false;
          }
          else if (chunkBack == null)
            back = mWorldBoundaries;
        }

        // right
        if (position.X == Chunk.Size - 1) {
          if (chunkRight != null) {
            if (chunkRight.Blocks.ContainsKey(new Vector3i(0, position.Y, position.Z)))
              right = false;
          }
          else if (chunkRight == null)
            right = mWorldBoundaries;
        }

        // left
        if (position.X == 0) {
          if (chunkLeft != null) {
            if (chunkLeft.Blocks.ContainsKey(new Vector3i(Chunk.Size - 1, position.Y, position.Z)))
              left = false;
          }
          else if (chunkLeft == null)
            left = mWorldBoundaries;
        }

        // top
        if (position.Y == Chunk.Size - 1) {
          if (chunkTop != null) {
            if (chunkTop.Blocks.ContainsKey(new Vector3i(position.X, 0, position.Z)))
              top = false;
          }
          else
            top = false;
        }

        // bottom
        if (position.Y == 0) {
          if (chunkBottom != null) {
            if (chunkBottom.Blocks.ContainsKey(new Vector3i(position.X, Chunk.Size - 1, position.Z)))
              bottom = false;
          }
          else if (chunkBottom == null)
            bottom = mWorldBoundaries;
        }

        // block texture
        float texture = 0f;
        switch (block.BlockType) {
          case Block.Type.Grass:
            texture = 2f;
            break;

          case Block.Type.Dirt:
            texture = 1f;
            break;
        }

        // add sides
        if (front) {
          AddSide(x, y, z, Block.Side.Front, texture, vertexData, elementData, count, block.Light);
          count += 4;
        }
        if (back) {
          AddSide(x, y, z, Block.Side.Back, texture, vertexData, elementData, count, block.Light);
          count += 4;
        }
        if (right) {
          AddSide(x, y, z, Block.Side.Right, texture, vertexData, elementData, count, block.Light);
          count += 4;
        }
        if (left) {
          AddSide(x, y, z, Block.Side.Left, texture, vertexData, elementData, count, block.Light);
          count += 4;
        }
        if (top) {
          AddSide(x, y, z, Block.Side.Top, texture, vertexData, elementData, count, block.Light);
          count += 4;
        }
        if (bottom) {
          AddSide(x, y, z, Block.Side.Bottom, texture, vertexData, elementData, count, block.Light);
          count += 4;
        }
      }

      // convert to array
      vertices = vertexData.ToArray();
      elements = elementData.ToArray();
    }

    /// <summary>
    /// Thread entry point.
    /// </summary>
    private void Start() {
      while (World.Current != null) {
        if (mChunksToBuild.Count <= 0) {
          Thread.Sleep(500);
          continue;
        }

        Chunk chunk;
        lock (mChunksToBuild) {
          if (mChunksToBuild.Count <= 0)
            continue;

          chunk = mChunksToBuild.Dequeue() as Chunk;
        }

        float[] vertices;
        uint[] elements;

        Build(chunk, out vertices, out elements);

        lock (mChunksToBuildMain) {
          mChunksToBuildMain.Enqueue(new BuiltChunk(chunk, vertices, elements));
        }
      }
    }

    #endregion Methods

    #region Fields

    /// <summary>
    /// If true, builder will build sides on world boundaries.
    /// </summary>
    private static bool mWorldBoundaries = false;

    /// <summary>
    /// World's list of ready chunks.
    /// </summary>
    private Dictionary<Vector3i, Chunk> mChunks;

    /// <summary>
    /// World's queue of chunks to build.
    /// </summary>
    private PriorityQueue<Chunk> mChunksToBuild;

    /// <summary>
    /// World's queue of chunks to build in the main thread.
    /// </summary>
    private Queue<BuiltChunk> mChunksToBuildMain;

    #endregion Fields
  }

}