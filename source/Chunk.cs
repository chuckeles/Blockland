using OpenTK.Graphics.OpenGL4;
using System.Collections;
using System.Collections.Generic;

namespace Blockland {

  public struct Block {

    public enum Type {
      Grass = 1,
      Dirt = 2,
      Stone = 3
    }

    public Block(Type type) {
      BlockType = type;
    }

    public Type BlockType;

    public static float Size = 2f;

  }

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
      gameObject.OnDraw += Draw;
    }

    public void Draw() {
      mArrayObject.Bind();
      Window.Instance.DrawTriangles(mElements.Length);
    }

    public Vector3i Position {
      get {
        return mPosition;
      }
    }

    public ArrayObject ArrayObject {
      get {
        return mArrayObject;
      }
    }

    public BufferObject Vertices {
      get {
        return mVertices;
      }
    }

    public BufferObject Elements {
      get {
        return mElements;
      }
    }

    public static int Size = 16;
    public Dictionary<Vector3i, Block> Blocks = new Dictionary<Vector3i, Block>();
    public State CurrentState = State.Empty;

    private Vector3i mPosition;
    private ArrayObject mArrayObject = new ArrayObject();
    private BufferObject mVertices = new BufferObject(BufferObject.Type.Vertex);
    private BufferObject mElements = new BufferObject(BufferObject.Type.Element);

  }

}