using SimplexNoise;
using System.Collections;
using System.Collections.Generic;

namespace Blockland {

  public class World {

    public void Create(int size, int height) {
      mHeight = height;

      // create chunks
      for (int x = -(size - 1) / 2; x <= size / 2; ++x)
        for (int z = -(size - 1) / 2; z <= size / 2; ++z)
          for (int y = -(size - 1) / 2; y <= size / 2; ++y)
            mChunks.Add(new Vector3i(x, y, z), new Chunk(x, y, z));

      // generate chunks
      for (int x = -(size - 1) / 2; x <= size / 2; ++x)
        for (int z = -(size - 1) / 2; z <= size / 2; ++z)
          for (int y = -(size - 1) / 2; y <= size / 2; ++y) {
            Chunk chunk;
            mChunks.TryGetValue(new Vector3i(x, y, z), out chunk);
            GenerateChunk(chunk);
          }

      // build chunks
      for (int x = -(size - 1) / 2; x <= size / 2; ++x)
        for (int z = -(size - 1) / 2; z <= size / 2; ++z)
          for (int y = -(size - 1) / 2; y <= size / 2; ++y) {
            Chunk chunk;
            mChunks.TryGetValue(new Vector3i(x, y, z), out chunk);
            BuildChunk(chunk);
          }

      // create game objects
      for (int x = -(size - 1) / 2; x <= size / 2; ++x)
        for (int z = -(size - 1) / 2; z <= size / 2; ++z)
          for (int y = -(size - 1) / 2; y <= size / 2; ++y) {
            Chunk chunk;
            mChunks.TryGetValue(new Vector3i(x, y, z), out chunk);

            GameObject gameObject = new GameObject();
            gameObject.AddComponent(new Transform(x * Chunk.Size * Block.Size, y * Chunk.Size * Block.Size, z * Chunk.Size * Block.Size));
            gameObject.AddComponent(chunk);

            mReadyChunks.Enqueue(gameObject);
          }
    }

    public void Update() {
      while (mReadyChunks.Count > 0)
        State.Current.AddGameObject(mReadyChunks.Dequeue());
    }

    public void GenerateChunk(Chunk chunk) {
      float noiseScale = 40f;
      float noiseScaleHeight = 80f;

      for (int x = 0; x < Chunk.Size; ++x)
        for (int y = 0; y < Chunk.Size; ++y)
          for (int z = 0; z < Chunk.Size; ++z)
            if (Noise.Generate((x + chunk.Position.X * Chunk.Size) / noiseScale,
              (y + chunk.Position.Y * Chunk.Size) / noiseScaleHeight,
              (z + chunk.Position.Z * Chunk.Size) / noiseScale) - ((y + chunk.Position.Y * Chunk.Size - mHeight / 2 * Chunk.Size) / 16f) > 0)
              chunk.Blocks.Add(new Vector3i(x, y, z), new Block());

      chunk.CurrentState = Chunk.State.Generated;
    }

    public void BuildChunk(Chunk chunk) {
      chunk.ArrayObject.Bind();

      ArrayList vertexData = new ArrayList();
      ArrayList elementData = new ArrayList();

      uint count = 0;
      foreach (var block in chunk.Blocks) {
        float x = (block.Key.X - Chunk.Size / 2) * Block.Size;
        float y = (block.Key.Y - Chunk.Size / 2) * Block.Size;
        float z = (block.Key.Z - Chunk.Size / 2) * Block.Size;

        // front

        if (!chunk.Blocks.ContainsKey(new Vector3i(block.Key.X, block.Key.Y, block.Key.Z + 1))) {
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

        if (!chunk.Blocks.ContainsKey(new Vector3i(block.Key.X, block.Key.Y, block.Key.Z - 1))) {
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

        if (!chunk.Blocks.ContainsKey(new Vector3i(block.Key.X + 1, block.Key.Y, block.Key.Z))) {
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

        if (!chunk.Blocks.ContainsKey(new Vector3i(block.Key.X - 1, block.Key.Y, block.Key.Z))) {
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

        if (!chunk.Blocks.ContainsKey(new Vector3i(block.Key.X, block.Key.Y + 1, block.Key.Z))) {
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

        if (!chunk.Blocks.ContainsKey(new Vector3i(block.Key.X, block.Key.Y - 1, block.Key.Z))) {
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

      chunk.Vertices.CopyData(vertices, true);
      chunk.Elements.CopyData(elements, true);

      ShaderProgram.Current.Attribute("inPosition", 3, sizeof(float) * 6, 0);
      ShaderProgram.Current.Attribute("inNormal", 3, sizeof(float) * 6, sizeof(float) * 3);
    }

    private Dictionary<Vector3i, Chunk> mChunks = new Dictionary<Vector3i, Chunk>();
    private Queue<GameObject> mReadyChunks = new Queue<GameObject>();
    private int mHeight;

  }

}