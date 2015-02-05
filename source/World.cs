using SimplexNoise;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace Blockland {

  public class World {

    public struct ChunkToBuild {
      public Chunk Chunk;
      public float[] Vertices;
      public uint[] Elements;

      public ChunkToBuild(Chunk chunk, float[] vertices, uint[] elements) {
        Chunk = chunk;
        Vertices = vertices;
        Elements = elements;
      }
    }

    public void Create(int size, int height) {
      mHeight = height;

      // create chunks
      for (int x = 0; x < size; ++x)
        for (int z = 0; z < size; ++z)
          for (int y = 0; y < height; ++y) {
            Chunk chunk = new Chunk(x, y, z);
            mChunks.Add(chunk.Position, chunk);
            mChunksToProcess.Enqueue(chunk);
          }

      // spawn worker
      uint workers = 3;
      for (uint i = 0; i < workers; ++i)
        (new Thread(Worker)).Start(this);
    }

    public void Update() {
      lock (mChunksToBuild) {
        if (mChunksToBuild.Count > 0) {
          ChunkToBuild buildInfo = mChunksToBuild.Dequeue();
          Chunk chunk = buildInfo.Chunk;

          // copy data to buffer
          chunk.ArrayObject.Bind();
          chunk.Vertices.CopyData(buildInfo.Vertices, true);
          chunk.Elements.CopyData(buildInfo.Elements, true);

          ShaderProgram.Current.Attribute("inPosition", 3, sizeof(float) * 6, 0);
          ShaderProgram.Current.Attribute("inNormal", 3, sizeof(float) * 6, sizeof(float) * 3);

          // create game object
          GameObject chunkObject = new GameObject();

          chunkObject.AddComponent(new Transform(chunk.Position.X * Chunk.Size * Block.Size, chunk.Position.Y * Chunk.Size * Block.Size, chunk.Position.Z * Chunk.Size * Block.Size));
          chunkObject.AddComponent(chunk);

          State.Current.AddGameObject(chunkObject);
        }
      }
    }

    public static void Worker(object parameter) {
      World world = parameter as World;

      while (!Interlocked.Equals(State.Current, null)) {
        Chunk chunk;

        lock (world.mChunksToProcess) {
          if (world.mChunksToProcess.Count > 0)
            chunk = world.mChunksToProcess.Dequeue();
          else
            continue;
        }

        // what to do
        switch (chunk.CurrentState) {
          case Chunk.State.Empty:
            // generate
            GenerateChunk(chunk, world.mHeight);

            // put to queue for processing
            lock (world.mChunksToProcess) {
              world.mChunksToProcess.Enqueue(chunk);
            }

            break;

          case Chunk.State.Generated:
          case Chunk.State.Dirty:
            // build
            BuildChunk(chunk, world.mChunks, world.mChunksToBuild);

            break;
        }
      }

    }

    public static void GenerateChunk(Chunk chunk, int height) {

      float noiseFrequency = 1 / 256f;

      for (int x = 0; x < Chunk.Size; ++x)
        for (int z = 0; z < Chunk.Size; ++z) {
          float noiseHeight = Random.Simplex((x + chunk.Position.X * Chunk.Size) * noiseFrequency, (z + chunk.Position.Z * Chunk.Size) * noiseFrequency, 4, height * Chunk.Size / 8, height * Chunk.Size / 8 * 7);
          float localHeight = noiseHeight - chunk.Position.Y * Chunk.Size;

          for (int y = 0; y < Chunk.Size; ++y) {
            if (y < localHeight)
              chunk.Blocks.Add(new Vector3i(x, y, z), new Block());
          }
        }

      chunk.CurrentState = Chunk.State.Generated;
    }

    public static void BuildChunk(Chunk chunk, Dictionary<Vector3i, Chunk> chunks, Queue<ChunkToBuild> buildQueue) {

      ArrayList vertexData = new ArrayList();
      ArrayList elementData = new ArrayList();

      // neighbor chunks
      Chunk chunkFront;
      Chunk chunkBack;
      Chunk chunkRight;
      Chunk chunkLeft;
      Chunk chunkTop;
      Chunk chunkBottom;

      chunks.TryGetValue(new Vector3i(chunk.Position.X, chunk.Position.Y, chunk.Position.Z + 1), out chunkFront);
      chunks.TryGetValue(new Vector3i(chunk.Position.X, chunk.Position.Y, chunk.Position.Z - 1), out chunkBack);
      chunks.TryGetValue(new Vector3i(chunk.Position.X + 1, chunk.Position.Y, chunk.Position.Z), out chunkRight);
      chunks.TryGetValue(new Vector3i(chunk.Position.X - 1, chunk.Position.Y, chunk.Position.Z), out chunkLeft);
      chunks.TryGetValue(new Vector3i(chunk.Position.X, chunk.Position.Y + 1, chunk.Position.Z), out chunkTop);
      chunks.TryGetValue(new Vector3i(chunk.Position.X, chunk.Position.Y - 1, chunk.Position.Z), out chunkBottom);

      uint count = 0;
      foreach (var block in chunk.Blocks) {
        // position
        float x = (block.Key.X - Chunk.Size / 2) * Block.Size;
        float y = (block.Key.Y - Chunk.Size / 2) * Block.Size;
        float z = (block.Key.Z - Chunk.Size / 2) * Block.Size;

        // sides
        bool front = !chunk.Blocks.ContainsKey(new Vector3i(block.Key.X, block.Key.Y, block.Key.Z + 1));
        bool back = !chunk.Blocks.ContainsKey(new Vector3i(block.Key.X, block.Key.Y, block.Key.Z - 1));
        bool right = !chunk.Blocks.ContainsKey(new Vector3i(block.Key.X + 1, block.Key.Y, block.Key.Z));
        bool left = !chunk.Blocks.ContainsKey(new Vector3i(block.Key.X - 1, block.Key.Y, block.Key.Z));
        bool top = !chunk.Blocks.ContainsKey(new Vector3i(block.Key.X, block.Key.Y + 1, block.Key.Z));
        bool bottom = !chunk.Blocks.ContainsKey(new Vector3i(block.Key.X, block.Key.Y - 1, block.Key.Z));

        if (block.Key.Z == Chunk.Size - 1) {
          if (chunkFront != null) {
            if (chunkFront.Blocks.ContainsKey(new Vector3i(block.Key.X, block.Key.Y, 0)))
              front = false;
          }
          else
            front = false;
        }
        if (block.Key.Z == 0) {
          if (chunkBack != null) {
            if (chunkBack.Blocks.ContainsKey(new Vector3i(block.Key.X, block.Key.Y, Chunk.Size - 1)))
              back = false;
          }
          else
            back = false;
        }
        if (block.Key.X == Chunk.Size - 1) {
          if (chunkRight != null) {
            if (chunkRight.Blocks.ContainsKey(new Vector3i(0, block.Key.Y, block.Key.Z)))
              right = false;
          }
          else
            right = false;
        }
        if (block.Key.X == 0) {
          if (chunkLeft != null) {
            if (chunkLeft.Blocks.ContainsKey(new Vector3i(Chunk.Size - 1, block.Key.Y, block.Key.Z)))
              left = false;
          }
          else
            left = false;
        }
        if (block.Key.Y == Chunk.Size - 1) {
          if (chunkTop != null) {
            if (chunkTop.Blocks.ContainsKey(new Vector3i(block.Key.X, 0, block.Key.Z)))
              top = false;
          }
          else
            top = false;
        }
        if (block.Key.Y == 0) {
          if (chunkBottom != null) {
            if (chunkBottom.Blocks.ContainsKey(new Vector3i(block.Key.X, Chunk.Size - 1, block.Key.Z)))
              bottom = false;
          }
          else
            bottom = false;
        }

        // front
        if (front) {
          float[] frontVertices = {
                                  x, y, z+Block.Size,                       0f, 0f, 1f,
                                  x+Block.Size, y, z+Block.Size,            0f, 0f, 1f,
                                  x+Block.Size, y+Block.Size, z+Block.Size, 0f, 0f, 1f,
                                  x, y+Block.Size, z+Block.Size,            0f, 0f, 1f
                                };
          vertexData.AddRange(frontVertices);

          uint[] frontElements = {
                                 count, count+1, count+2,
                                 count, count+2, count+3
                               };

          elementData.AddRange(frontElements);
          count += 4;
        }

        // back
        if (back) {
          float[] backVertices = {
                                  x+Block.Size, y, z,            0f, 0f, -1f,
                                  x, y, z,                       0f, 0f, -1f,
                                  x, y+Block.Size, z,            0f, 0f, -1f,
                                  x+Block.Size, y+Block.Size, z, 0f, 0f, -1f
                                };
          vertexData.AddRange(backVertices);

          uint[] backElements = {
                                 count, count+1, count+2,
                                 count, count+2, count+3
                               };

          elementData.AddRange(backElements);
          count += 4;
        }

        // right
        if (right) {
          float[] rightVertices = {
                                  x+Block.Size, y, z+Block.Size,            1f, 0f, 0f,
                                  x+Block.Size, y, z,                       1f, 0f, 0f,
                                  x+Block.Size, y+Block.Size, z,            1f, 0f, 0f,
                                  x+Block.Size, y+Block.Size, z+Block.Size, 1f, 0f, 0f
                                };
          vertexData.AddRange(rightVertices);

          uint[] rightElements = {
                                 count, count+1, count+2,
                                 count, count+2, count+3
                               };

          elementData.AddRange(rightElements);
          count += 4;
        }

        // left
        if (left) {
          float[] leftVertices = {
                                  x, y, z,                       -1f, 0f, 0f,
                                  x, y, z+Block.Size,            -1f, 0f, 0f,
                                  x, y+Block.Size, z+Block.Size, -1f, 0f, 0f,
                                  x, y+Block.Size, z,            -1f, 0f, 0f
                                };
          vertexData.AddRange(leftVertices);

          uint[] leftElements = {
                                 count, count+1, count+2,
                                 count, count+2, count+3
                               };

          elementData.AddRange(leftElements);
          count += 4;
        }

        // top
        if (top) {
          float[] topVertices = {
                                  x, y+Block.Size, z+Block.Size,            0f, 1f, 0f,
                                  x+Block.Size, y+Block.Size, z+Block.Size, 0f, 1f, 0f,
                                  x+Block.Size, y+Block.Size, z,            0f, 1f, 0f,
                                  x, y+Block.Size, z,                       0f, 1f, 0f
                                };
          vertexData.AddRange(topVertices);

          uint[] topElements = {
                                 count, count+1, count+2,
                                 count, count+2, count+3
                               };

          elementData.AddRange(topElements);
          count += 4;
        }

        // bottom
        if (bottom) {
          float[] bottomVertices = {
                                  x+Block.Size, y, z+Block.Size, 0f, -1f, 0f,
                                  x, y, z+Block.Size,            0f, -1f, 0f,
                                  x, y, z,                       0f, -1f, 0f,
                                  x+Block.Size, y, z,            0f, -1f, 0f
                                };
          vertexData.AddRange(bottomVertices);

          uint[] bottomElements = {
                                 count, count+1, count+2,
                                 count, count+2, count+3
                               };

          elementData.AddRange(bottomElements);
          count += 4;
        }
      }

      float[] vertices = vertexData.ToArray(typeof(float)) as float[];
      uint[] elements = elementData.ToArray(typeof(uint)) as uint[];

      chunk.CurrentState = Chunk.State.Ready;

      lock (buildQueue) {
        buildQueue.Enqueue(new ChunkToBuild(chunk, vertices, elements));
      }
    }

    private Dictionary<Vector3i, Chunk> mChunks = new Dictionary<Vector3i, Chunk>();
    private Queue<Chunk> mChunksToProcess = new Queue<Chunk>();
    private Queue<ChunkToBuild> mChunksToBuild = new Queue<ChunkToBuild>();
    private int mHeight;

  }

}