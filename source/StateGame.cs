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

      int blocks = 8;
      mWorld.Create(blocks, 8);

      float halfSize = blocks * Chunk.Size * Block.Size / 2;
      (mCamera["Transform"] as Transform).Move(halfSize, halfSize, halfSize);
    }

    public override void Frame() {
      base.Frame();

      mWorld.Update();
    }

    public void OnEscape(object sender, KeyboardKeyEventArgs e) {
      if (e.Key == Key.Escape)
        mWindow.Close();
    }

    public override void End() {
      base.End();
    }

    private World mWorld = new World();

  }

}