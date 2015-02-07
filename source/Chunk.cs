using OpenTK.Graphics.OpenGL4;
using System.Collections;
using System.Collections.Generic;

namespace Blockland {

  public class Chunk
    : Component {

    public enum State {
      Empty,
      Generated,
      Ready,
      Dirty
    }

    public Chunk(int x, int y, int z)
      : base("Chunk") {
      mPosition = new Vector3i(x, y, z);
    }

    public override void Attached(GameObject gameObject) {
      base.Attached(gameObject);

      gameObject.EnsureTransform();
      gameObject.OnRender += Draw;
    }

    public void Draw() {
      mArrayObject.Bind();
      Window.Instance.DrawTriangles(mElements.Length);
    }

    public ArrayObject ArrayObject {
      get {
        return mArrayObject;
      }
    }

    public BufferObject Elements {
      get {
        return mElements;
      }
    }

    public Vector3i Position {
      get {
        return mPosition;
      }
    }

    public BufferObject Vertices {
      get {
        return mVertices;
      }
    }

    public static int Size = 16;
    public Dictionary<Vector3i, Block> Blocks = new Dictionary<Vector3i, Block>();
    public State CurrentState = State.Empty;

    private ArrayObject mArrayObject = new ArrayObject();
    private BufferObject mElements = new BufferObject(BufferObject.Type.Element);
    private Vector3i mPosition;
    private BufferObject mVertices = new BufferObject(BufferObject.Type.Vertex);
  }

  public struct Block {

    public enum Type {
      Grass = 1,
      Dirt = 2,
      Stone = 3
    }

    public Block(Type type) {
      BlockType = type;
    }

    public static float Size = 2f;
    public Type BlockType;
  }
}