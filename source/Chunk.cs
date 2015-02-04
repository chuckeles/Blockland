using OpenTK.Graphics.OpenGL4;
using SimplexNoise;
using System.Collections;
using System.Collections.Generic;

namespace Blockland {

  public struct Block {

    public struct Position {
      public int X;
      public int Y;
      public int Z;

      public Position(int x, int y, int z) {
        X = x;
        Y = y;
        Z = z;
      }
    }

    public static float Size = 2f;

  }

  public class Chunk
    : Component {

    public Chunk()
      : base("Chunk") {
      mBlocks = new Dictionary<Block.Position, Block>();

      mArrayObject = new ArrayObject();
      mVertices = new BufferObject(BufferObject.Type.Vertex);
      mElements = new BufferObject(BufferObject.Type.Element);

      float noiseScale = 10f;

      for (int x = 0; x < Size; ++x)
        for (int y = 0; y < Size; ++y)
          for (int z = 0; z < Size; ++z)
            if (Noise.Generate(x / noiseScale, y / noiseScale, z / noiseScale) > 0)
              mBlocks.Add(new Block.Position(x, y, z), new Block());
    }

    public override void Attached(GameObject gameObject) {
      base.Attached(gameObject);

      gameObject.OnDraw += Draw;
    }

    public void Draw() {
      mArrayObject.Bind();
      Window.Instance.DrawTriangles(mElements.Length);
    }

    public void Build(ShaderProgram shader) {
      mArrayObject.Bind();

      ArrayList vertexData = new ArrayList();
      ArrayList elementData = new ArrayList();

      uint count = 0;
      foreach (var block in mBlocks) {
        float x = (block.Key.X - Size / 2) * Block.Size;
        float y = (block.Key.Y - Size / 2) * Block.Size;
        float z = (block.Key.Z - Size / 2) * Block.Size;

        // front

        if (!mBlocks.ContainsKey(new Block.Position(block.Key.X, block.Key.Y, block.Key.Z + 1))) {
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

        if (!mBlocks.ContainsKey(new Block.Position(block.Key.X, block.Key.Y, block.Key.Z - 1))) {
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

        if (!mBlocks.ContainsKey(new Block.Position(block.Key.X + 1, block.Key.Y, block.Key.Z))) {
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

        if (!mBlocks.ContainsKey(new Block.Position(block.Key.X - 1, block.Key.Y, block.Key.Z))) {
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

        if (!mBlocks.ContainsKey(new Block.Position(block.Key.X, block.Key.Y + 1, block.Key.Z))) {
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

        if (!mBlocks.ContainsKey(new Block.Position(block.Key.X, block.Key.Y - 1, block.Key.Z))) {
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

      mVertices.CopyData(vertices, true);
      mElements.CopyData(elements, true);

      shader.Attribute("inPosition", 3, sizeof(float) * 6, 0);
      shader.Attribute("inNormal", 3, sizeof(float) * 6, sizeof(float) * 3);
    }

    public static int Size = 16;

    private Dictionary<Block.Position, Block> mBlocks;

    private ArrayObject mArrayObject;
    private BufferObject mVertices;
    private BufferObject mElements;

  }

}