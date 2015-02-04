using System;

namespace Blockland {

  public class Component {

    public Component(string name) {
      if (name == "")
        throw new Exception("Component needs a name");

      mName = name;
    }

    public virtual void Attached(GameObject gameObject) {
      mGameObject = gameObject;
    }

    public string Name {
      get {
        return mName;
      }
    }

    protected string mName;
    protected GameObject mGameObject;

  }

}