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

      int height = 4;
      int size = 3;

      for (int x = 0; x < size; ++x)
        for (int y = 0; y < height; ++y)
          for (int z = 0; z < size; ++z) {
            Chunk chunk = new Chunk();
            chunk.Generate(x, y, z, 40f, 100f);
            chunk.Build();

            GameObject chunkObject = new GameObject();
            chunkObject.AddComponent(chunk);

            AddGameObject(chunkObject);

            Transform transform = chunkObject["Transform"] as Transform;
            transform.Move(x * Block.Size * Chunk.Size, y * Block.Size * Chunk.Size, z * Block.Size * Chunk.Size);
          }
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