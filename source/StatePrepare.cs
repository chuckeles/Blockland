using OpenTK;
using System;

namespace Blockland {

  public class StatePrepare
    : State {

    public StatePrepare(Window window)
      : base(window) {
    }

    public override void Start() {
      base.Start();

      // set up projection
      Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView((float)Math.PI / 4, (float)mWindow.Width / mWindow.Height, .1f, 1000f);
      ShaderProgram.Current.Uniform("Projection", ref projection);

      End();

      State state = new StateGame(mWindow);
      state.Start();
    }

  }

}