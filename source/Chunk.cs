using System.Collections.Generic;

namespace Blockland {

  /// <summary>
  /// Chunk is a cubic part of the world. It is of a constant size. Chunk stores all the blocks,
  /// array object and buffer objects.
  /// </summary>
  public class Chunk
    : Component {

    #region Types

    /// <summary>
    /// The state of the chunk.
    /// </summary>
    public enum State {

      /// <summary>
      /// The chunk has just been created and is empty.
      /// </summary>
      Empty,

      /// <summary>
      /// The chunk has been generated and is waiting to be built.
      /// </summary>
      Generated,

      /// <summary>
      /// The chunk is build and up-to-date.
      /// </summary>
      Ready,

      /// <summary>
      /// Some blocks have been modified and the geometry needs to be rebuilt.
      /// </summary>
      Dirty
    }

    #endregion Types

    #region Constructor

    /// <summary>
    /// Create a new chunk at the position.
    /// </summary>
    /// <param name="x">X in chunk coordinates</param>
    /// <param name="y">Y in chunk coordinates</param>
    /// <param name="z">Z in chunk coordinates</param>
    public Chunk(int x, int y, int z)
      : base("Chunk") {
      mPosition = new Vector3i(x, y, z);
    }

    #endregion Constructor

    #region Methods

    /// <summary>
    /// Called by game object when the chunk is attached to it. Registers for render event.
    /// </summary>
    /// <param name="gameObject">Target game object</param>
    public override void Attached(GameObject gameObject) {
      base.Attached(gameObject);

      gameObject.EnsureTransform();
      gameObject.OnRender += Render;
    }

    /// <summary>
    /// Called by game object when the chunk is detached from it. Unregisters from render event.
    /// </summary>
    public override void Detached() {
      mGameObject.OnRender -= Render;

      base.Detached();
    }

    /// <summary>
    /// Render the chunk.
    /// </summary>
    public void Render() {
      mArrayObject.Bind();
      Window.Instance.RenderTriangles(mElements.Length);
    }

    #endregion Methods

    #region Properties

    /// <summary>
    /// Get the chunk's array object.
    /// </summary>
    public ArrayObject ArrayObject {
      get {
        return mArrayObject;
      }
    }

    /// <summary>
    /// Get the chunk's element buffer object.
    /// </summary>
    public BufferObject Elements {
      get {
        return mElements;
      }
    }

    /// <summary>
    /// Get chunk position in chunk coordinates.
    /// </summary>
    public Vector3i Position {
      get {
        return mPosition;
      }
    }

    /// <summary>
    /// Get chunk's vertex buffer object.
    /// </summary>
    public BufferObject Vertices {
      get {
        return mVertices;
      }
    }

    #endregion Properties

    #region Fields

    /// <summary>
    /// Chunk size in number of blocks.
    /// </summary>
    public static int Size = 16;

    /// <summary>
    /// Dictionary of blocks.
    /// </summary>
    public Dictionary<Vector3i, Block> Blocks = new Dictionary<Vector3i, Block>();

    /// <summary>
    /// Current chunk state.
    /// </summary>
    public State CurrentState = State.Empty;

    /// <summary>
    /// Array object.
    /// </summary>
    private ArrayObject mArrayObject = new ArrayObject();

    /// <summary>
    /// Element buffer object.
    /// </summary>
    private BufferObject mElements = new BufferObject(BufferObject.Type.Element);

    /// <summary>
    /// Position in chunk coordinates.
    /// </summary>
    private Vector3i mPosition;

    /// <summary>
    /// Vertex buffer object.
    /// </summary>
    private BufferObject mVertices = new BufferObject(BufferObject.Type.Vertex);

    #endregion Fields
  }

}