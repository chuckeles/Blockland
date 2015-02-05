using OpenTK.Graphics.OpenGL4;
using SimplexNoise;
using System.Collections;
using System.Collections.Generic;

namespace Blockland {

  public struct Block {

    public static float Size = 2f;

  }

  public class Chunk
    : Component {

    public Chunk()
      : base("Chunk") {
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

    public static int Size = 16;

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

    public Dictionary<Vector3i, Block> Blocks = new Dictionary<Vector3i, Block>();

    private ArrayObject mArrayObject = new ArrayObject();
    private BufferObject mVertices = new BufferObject(BufferObject.Type.Vertex);
    private BufferObject mElements = new BufferObject(BufferObject.Type.Element);

  }

}