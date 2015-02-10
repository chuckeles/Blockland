using System;
using System.Collections;
using System.Collections.Generic;

namespace Blockland {

  /// <summary>
  /// The world is a container for chunks. It spawns worker threads to manage the chunks.
  /// </summary>
  public class World {

    #region Constructor

    /// <summary>
    /// Create new world.
    /// </summary>
    /// <exception cref="Exception">When there already is a world</exception>
    public World() {
      if (mCurrent != null)
        throw new Exception("The world already exists");

      mCurrent = this;
    }

    #endregion Constructor

    #region Methods

    /// <summary>
    /// Start creating chunks and building the world.
    /// </summary>
    /// <param name="renderDistance">How many chunks to build around the player</param>
    /// <param name="height">World height in chunks</param>
    public void Create(int renderDistance, int height) {
      mHeight = height;

      new ChunkAllocator(mChunksToGenerate, mChunks, mChunksToRemove, renderDistance, height);
      new ChunkGenerator(mChunksToGenerate, mChunksToBuild, height);
      new ChunkGenerator(mChunksToGenerate, mChunksToBuild, height);
      new ChunkBuilder(mChunksToBuild, mChunksToBuildMain, mChunks);
      new ChunkBuilder(mChunksToBuild, mChunksToBuildMain, mChunks);
      new ChunkBuilder(mChunksToBuild, mChunksToBuildMain, mChunks);
      new ChunkBuilder(mChunksToBuild, mChunksToBuildMain, mChunks);

      Program.Events.OnUpdate += Update;
    }

    /// <summary>
    /// Destroy the world.
    /// </summary>
    public void Destroy() {
      Program.Events.OnUpdate -= Update;

      mCurrent = null;
    }

    /// <summary>
    /// Update the world. The world will process chunks that have to be processed in the main thread.
    /// </summary>
    /// <param name="deltaTime">Not used</param>
    public void Update(float deltaTime) {
      // remove chunks
      lock (mChunksToRemove) {
        while (mChunksToRemove.Count > 0) {
          Chunk chunk = mChunksToRemove.Dequeue() as Chunk;

          chunk.GameObject.RemoveComponent("Chunk");

          State.Current.RemoveGameObject(chunk.GameObject);
          lock (mChunks) {
            mChunks.Remove(chunk.Position);
          }
        }
      }

      Transform camera;
      if (!State.Current.Camera.HasComponent("Transform"))
        camera = new Transform();
      else
        camera = State.Current.Camera["Transform"] as Transform;
      Vector2i cameraPosition = new Vector2i((int)(camera.Position.X / (Chunk.Size * Block.Size)), (int)(camera.Position.Z / (Chunk.Size * Block.Size)));

      while (mChunksToBuildMain.Count > 0) {

        ChunkBuilder.BuiltChunk builtChunk;
        lock (mChunksToBuildMain) {
          builtChunk = (ChunkBuilder.BuiltChunk)mChunksToBuildMain.Dequeue();
        }

        ProcessChunk(builtChunk);

        if (!mChunks.ContainsKey(builtChunk.Chunk.Position)) {
          // the chunk is new! let also neighbors know

          // neighbor chunks
          Chunk chunkFront;
          Chunk chunkBack;
          Chunk chunkRight;
          Chunk chunkLeft;
          Chunk chunkTop;
          Chunk chunkBottom;

          lock (mChunks) {
            mChunks.TryGetValue(new Vector3i(builtChunk.Chunk.Position.X, builtChunk.Chunk.Position.Y, builtChunk.Chunk.Position.Z + 1), out chunkFront);
            mChunks.TryGetValue(new Vector3i(builtChunk.Chunk.Position.X, builtChunk.Chunk.Position.Y, builtChunk.Chunk.Position.Z - 1), out chunkBack);
            mChunks.TryGetValue(new Vector3i(builtChunk.Chunk.Position.X + 1, builtChunk.Chunk.Position.Y, builtChunk.Chunk.Position.Z), out chunkRight);
            mChunks.TryGetValue(new Vector3i(builtChunk.Chunk.Position.X - 1, builtChunk.Chunk.Position.Y, builtChunk.Chunk.Position.Z), out chunkLeft);
            mChunks.TryGetValue(new Vector3i(builtChunk.Chunk.Position.X, builtChunk.Chunk.Position.Y + 1, builtChunk.Chunk.Position.Z), out chunkTop);
            mChunks.TryGetValue(new Vector3i(builtChunk.Chunk.Position.X, builtChunk.Chunk.Position.Y - 1, builtChunk.Chunk.Position.Z), out chunkBottom);
          }

          // add chunks to build queue
          lock (mChunksToBuild) {
            if (chunkFront != null)
              mChunksToBuild.Enqueue((int)(new Vector3i(chunkFront.Position.X - cameraPosition.X, chunkFront.Position.Y - mHeight / 2, chunkFront.Position.Z - cameraPosition.Y).Length * 100), chunkFront);
            if (chunkBack != null)
              mChunksToBuild.Enqueue((int)(new Vector3i(chunkBack.Position.X - cameraPosition.X, chunkBack.Position.Y - mHeight / 2, chunkBack.Position.Z - cameraPosition.Y).Length * 100), chunkBack);
            if (chunkRight != null)
              mChunksToBuild.Enqueue((int)(new Vector3i(chunkRight.Position.X - cameraPosition.X, chunkRight.Position.Y - mHeight / 2, chunkRight.Position.Z - cameraPosition.Y).Length * 100), chunkRight);
            if (chunkLeft != null)
              mChunksToBuild.Enqueue((int)(new Vector3i(chunkLeft.Position.X - cameraPosition.X, chunkLeft.Position.Y - mHeight / 2, chunkLeft.Position.Z - cameraPosition.Y).Length * 100), chunkLeft);
            if (chunkTop != null)
              mChunksToBuild.Enqueue((int)(new Vector3i(chunkTop.Position.X - cameraPosition.X, chunkTop.Position.Y - mHeight / 2, chunkTop.Position.Z - cameraPosition.Y).Length * 100), chunkTop);
            if (chunkBottom != null)
              mChunksToBuild.Enqueue((int)(new Vector3i(chunkBottom.Position.X - cameraPosition.X, chunkBottom.Position.Y - mHeight / 2, chunkBottom.Position.Z - cameraPosition.Y).Length * 100), chunkBottom);
          }

          lock (mChunks) {
            mChunks.Add(builtChunk.Chunk.Position, builtChunk.Chunk);
          }

          GameObject gameObject = new GameObject();
          gameObject.AddComponent(new Transform(builtChunk.Chunk.Position.X * Chunk.Size * Block.Size, builtChunk.Chunk.Position.Y * Chunk.Size * Block.Size, builtChunk.Chunk.Position.Z * Chunk.Size * Block.Size));
          gameObject.AddComponent(builtChunk.Chunk);

          State.Current.AddGameObject(gameObject);
        }
      }
    }

    /// <summary>
    /// Copy data generated by ChunkBuilder to chunk's buffer object.
    /// </summary>
    /// <param name="builtChunk">Chunk with vertex and element arrays</param>
    private void ProcessChunk(ChunkBuilder.BuiltChunk builtChunk) {
      Chunk chunk = builtChunk.Chunk;

      if (chunk.ArrayObject.Id == 0)
        chunk.ArrayObject.Create();

      if (chunk.Vertices.Id == 0)
        chunk.Vertices.Create();

      if (chunk.Elements.Id == 0)
        chunk.Elements.Create();

      chunk.ArrayObject.Bind();
      chunk.Vertices.CopyData(builtChunk.Vertices, true);
      chunk.Elements.CopyData(builtChunk.Elements, true);

      ShaderProgram.Current.Attribute("inPosition", 3, 9, 0);
      ShaderProgram.Current.Attribute("inNormal", 3, 9, 3);
      ShaderProgram.Current.Attribute("inTexCoord", 3, 9, 6);
    }

    #endregion Methods

    #region Properties

    /// <summary>
    /// Get the current world instance.
    /// </summary>
    public static World Current {
      get {
        return mCurrent;
      }
    }

    /// <summary>
    /// Get world height.
    /// </summary>
    public int Height {
      get {
        return mHeight;
      }
    }

    /// <summary>
    /// Get chunks to build.
    /// </summary>
    public PriorityQueue<Chunk> ChunksToBuild {
      get {
        return mChunksToBuild;
      }
    }

    /// <summary>
    /// Get chunks to generate.
    /// </summary>
    public PriorityQueue<Chunk> ChunksToGenerate {
      get {
        return mChunksToGenerate;
      }
    }

    /// <summary>
    /// Get queue of chunks to remove.
    /// </summary>
    public Queue ChunksToRemove {
      get {
        return mChunksToRemove;
      }
    }

    #endregion Properties

    #region Fields

    /// <summary>
    /// Static instance.
    /// </summary>
    private static World mCurrent;

    /// <summary>
    /// World height in chunks.
    /// </summary>
    private int mHeight = 0;

    /// <summary>
    /// Dictionary of loaded and ready chunks.
    /// </summary>
    private Dictionary<Vector3i, Chunk> mChunks = new Dictionary<Vector3i, Chunk>();

    /// <summary>
    /// Queue of chunks waiting to be built.
    /// </summary>
    private PriorityQueue<Chunk> mChunksToBuild = new PriorityQueue<Chunk>();

    /// <summary>
    /// Chunks that need to be processed in the main thread.
    /// </summary>
    private Queue mChunksToBuildMain = new Queue();

    /// <summary>
    /// Queue of chunks waiting to be generated.
    /// </summary>
    private PriorityQueue<Chunk> mChunksToGenerate = new PriorityQueue<Chunk>();

    /// <summary>
    /// Queue of chunks to remove.
    /// </summary>
    private Queue mChunksToRemove = new Queue();

    #endregion Fields

  }

}