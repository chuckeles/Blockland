using OpenTK.Input;

namespace Blockland {

  public class StateGame
    : State {

    public StateGame(Window window)
      : base(window) {
    }

    public override void Start() {
      base.Start();

      mWindow.NativeWindow.KeyDown += OnEscape;

      // create chunk
      Chunk chunk = new Chunk();
      chunk.Generate();
      chunk.Build();

      // create game object
      GameObject chunkObject = new GameObject();
      chunkObject.AddComponent(chunk);

      AddGameObject(chunkObject);

      // create second chunk
      Chunk chunk2 = new Chunk();
      chunk2.Generate();
      chunk2.Build();

      // create game object
      GameObject chunkObject2 = new GameObject();
      chunkObject2.AddComponent(chunk2);

      AddGameObject(chunkObject2);

      // move second chunk
      Transform transform = chunkObject2["Transform"] as Transform;
      transform.Move(Block.Size * Chunk.Size, 0f, 0f);
    }

    public void OnEscape(object sender, KeyboardKeyEventArgs e) {
      if (e.Key == Key.Escape)
        mWindow.Close();
    }

    public override void End() {
      base.End();
    }

  }

}