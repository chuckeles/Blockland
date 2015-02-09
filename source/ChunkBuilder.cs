using System.Collections;
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
    public ChunkBuilder(PriorityQueue<Chunk> chunksToBuild, Queue chunksToBuildMain, Dictionary<Vector3i, Chunk> chunks) {
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
    public void AddSide(float x, float y, float z, Block.Side side, float texture, ArrayList vertices, ArrayList elements, uint elementOffset) {
      switch (side) {
        case Block.Side.Front:
          float[] frontVertices = {
                                  x, y, z+Block.Size,                       0f, 0f, 1f, 0f, 0f, texture,
                                  x+Block.Size, y, z+Block.Size,            0f, 0f, 1f, 1f, 0f, texture,
                                  x+Block.Size, y+Block.Size, z+Block.Size, 0f, 0f, 1f, 1f, 1f, texture,
                                  x, y+Block.Size, z+Block.Size,            0f, 0f, 1f, 0f, 1f, texture
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
                                  x+Block.Size, y, z,            0f, 0f, -1f, 0f, 0f, texture,
                                  x, y, z,                       0f, 0f, -1f, 1f, 0f, texture,
                                  x, y+Block.Size, z,            0f, 0f, -1f, 1f, 1f, texture,
                                  x+Block.Size, y+Block.Size, z, 0f, 0f, -1f, 0f, 1f, texture
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
                                  x+Block.Size, y, z+Block.Size,            1f, 0f, 0f, 0f, 0f, texture,
                                  x+Block.Size, y, z,                       1f, 0f, 0f, 1f, 0f, texture,
                                  x+Block.Size, y+Block.Size, z,            1f, 0f, 0f, 1f, 1f, texture,
                                  x+Block.Size, y+Block.Size, z+Block.Size, 1f, 0f, 0f, 0f, 1f, texture
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
                                  x, y, z,                       -1f, 0f, 0f, 0f, 0f, texture,
                                  x, y, z+Block.Size,            -1f, 0f, 0f, 1f, 0f, texture,
                                  x, y+Block.Size, z+Block.Size, -1f, 0f, 0f, 1f, 1f, texture,
                                  x, y+Block.Size, z,            -1f, 0f, 0f, 0f, 1f, texture
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
                                  x, y+Block.Size, z+Block.Size,            0f, 1f, 0f, 0f, 0f, texture,
                                  x+Block.Size, y+Block.Size, z+Block.Size, 0f, 1f, 0f, 1f, 0f, texture,
                                  x+Block.Size, y+Block.Size, z,            0f, 1f, 0f, 1f, 1f, texture,
                                  x, y+Block.Size, z,                       0f, 1f, 0f, 0f, 1f, texture
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
                                  x+Block.Size, y, z+Block.Size, 0f, -1f, 0f, 0f, 0f, texture,
                                  x, y, z+Block.Size,            0f, -1f, 0f, 1f, 0f, texture,
                                  x, y, z,                       0f, -1f, 0f, 1f, 1f, texture,
                                  x+Block.Size, y, z,            0f, -1f, 0f, 0f, 1f, texture
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
      ArrayList vertexData = new ArrayList();
      // list of elements
      ArrayList elementData = new ArrayList();

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
        bool front = !chunk.Blocks.ContainsKey(new Vector3i(position.X, position.Y, position.Z + 1));
        bool back = !chunk.Blocks.ContainsKey(new Vector3i(position.X, position.Y, position.Z - 1));
        bool right = !chunk.Blocks.ContainsKey(new Vector3i(position.X + 1, position.Y, position.Z));
        bool left = !chunk.Blocks.ContainsKey(new Vector3i(position.X - 1, position.Y, position.Z));
        bool top = !chunk.Blocks.ContainsKey(new Vector3i(position.X, position.Y + 1, position.Z));
        bool bottom = !chunk.Blocks.ContainsKey(new Vector3i(position.X, position.Y - 1, position.Z));

        // consider neighbor chunks

        // front
        if (position.Z == Chunk.Size - 1) {
          if (chunkFront != null) {
            if (chunkFront.Blocks.ContainsKey(new Vector3i(position.X, position.Y, 0)))
              front = false;
          }
          else
            front = false;
        }

        // back
        if (position.Z == 0) {
          if (chunkBack != null) {
            if (chunkBack.Blocks.ContainsKey(new Vector3i(position.X, position.Y, Chunk.Size - 1)))
              back = false;
          }
          else
            back = false;
        }

        // right
        if (position.X == Chunk.Size - 1) {
          if (chunkRight != null) {
            if (chunkRight.Blocks.ContainsKey(new Vector3i(0, position.Y, position.Z)))
              right = false;
          }
          else
            right = false;
        }

        // left
        if (position.X == 0) {
          if (chunkLeft != null) {
            if (chunkLeft.Blocks.ContainsKey(new Vector3i(Chunk.Size - 1, position.Y, position.Z)))
              left = false;
          }
          else
            left = false;
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
          else
            bottom = false;
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
          AddSide(x, y, z, Block.Side.Front, texture, vertexData, elementData, count);
          count += 4;
        }
        if (back) {
          AddSide(x, y, z, Block.Side.Back, texture, vertexData, elementData, count);
          count += 4;
        }
        if (right) {
          AddSide(x, y, z, Block.Side.Right, texture, vertexData, elementData, count);
          count += 4;
        }
        if (left) {
          AddSide(x, y, z, Block.Side.Left, texture, vertexData, elementData, count);
          count += 4;
        }
        if (top) {
          AddSide(x, y, z, Block.Side.Top, texture, vertexData, elementData, count);
          count += 4;
        }
        if (bottom) {
          AddSide(x, y, z, Block.Side.Bottom, texture, vertexData, elementData, count);
          count += 4;
        }
      }

      // convert to array
      vertices = vertexData.ToArray(typeof(float)) as float[];
      elements = elementData.ToArray(typeof(uint)) as uint[];
    }

    /// <summary>
    /// Thread entry point.
    /// </summary>
    private void Start() {
      while (World.Current != null) {
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
    private Queue mChunksToBuildMain;

    #endregion Fields
  }

}