using System;

namespace Blockland {

  /// <summary>
  /// Represents one piece of game mechanic or game object technology.
  /// </summary>
  public class Component {

    #region Constructor

    /// <summary>
    /// Create a new game object component.
    /// </summary>
    /// <param name="name">Component name</param>
    /// <exception cref="ArgumentNullException">If the name is empty</exception>
    public Component(string name) {
      if (name == "")
        throw new ArgumentNullException("Component needs a name");

      mName = name;
    }

    #endregion Constructor

    #region Methods

    /// <summary>
    /// Called by game object when the component is added to it.
    /// </summary>
    /// <param name="gameObject">Game object to which the component was added</param>
    public virtual void Attached(GameObject gameObject) {
      mGameObject = gameObject;
    }

    /// <summary>
    /// Called by game object when the component is detached from it.
    /// </summary>
    public virtual void Detached() {
      mGameObject = null;
    }

    #endregion Methods

    #region Properties

    /// <summary>
    /// Get the game object to which this component is attached.
    /// </summary>
    public GameObject GameObject {
      get {
        return mGameObject;
      }
    }

    /// <summary>
    /// Get component name.
    /// </summary>
    public string Name {
      get {
        return mName;
      }
    }

    #endregion Properties

    #region Fields

    /// <summary>
    /// Game object to which this component is currently attached.
    /// </summary>
    protected GameObject mGameObject;

    /// <summary>
    /// Component name.
    /// </summary>
    protected string mName;

    #endregion Fields
  }

}