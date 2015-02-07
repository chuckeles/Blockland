using System;
using System.Collections.Generic;

namespace Blockland {

  /// <summary>
  /// Game object is a container for components.
  /// </summary>
  public class GameObject {

    #region Constructor

    /// <summary>
    /// Constructor, registers for update and render events.
    /// </summary>
    public GameObject() {
      Program.Events.OnUpdate += Update;
      Program.Events.OnRender += Render;
    }

    /// <summary>
    /// Destructor
    /// </summary>
    ~GameObject() {
      Program.Events.OnRender -= Render;
      Program.Events.OnUpdate -= Update;
    }

    #endregion Constructor

    #region Methods

    /// <summary>
    /// Add a new component to the game object.
    /// </summary>
    /// <param name="component">Component to add</param>
    /// <exception cref="ArgumentException">When it already contains component</exception>
    public void AddComponent(Component component) {
      if (HasComponent(component.Name))
        throw new ArgumentException("Component already added");

      mComponents.Add(component.Name, component);
      component.Attached(this);
    }

    /// <summary>
    /// Add transform component if the game object doesn't already have one.
    /// </summary>
    public void EnsureTransform() {
      if (!HasComponent("Transform"))
        AddComponent(new Transform());
    }

    /// <summary>
    /// Get a component by name.
    /// </summary>
    /// <param name="name">Component name</param>
    /// <returns>Requested component</returns>
    public Component GetComponent(string name) {
      return mComponents[name];
    }

    /// <summary>
    /// Check if the game object has a component.
    /// </summary>
    /// <param name="name">Component name</param>
    /// <returns>True if the game object has a component</returns>
    public bool HasComponent(string name) {
      return mComponents.ContainsKey(name);
    }

    /// <summary>
    /// Remove component from the game object.
    /// </summary>
    /// <param name="name">Component name</param>
    public void RemoveComponent(string name) {
      if (HasComponent(name)) {
        Component component = GetComponent(name);
        component.Detached();

        mComponents.Remove(name);
      }
    }

    /// <summary>
    /// Fires the render event.
    /// </summary>
    private void Render() {
      if (OnRender != null)
        OnRender();
    }

    /// <summary>
    /// Fires the update event.
    /// </summary>
    /// <param name="deltaTime">Time since last update</param>
    private void Update(float deltaTime) {
      if (OnUpdate != null)
        OnUpdate(deltaTime);
    }

    #endregion Methods

    #region Events

    /// <summary>
    /// Fired once per frame after the update. Only for this game object's components.
    /// </summary>
    public event Events.Event OnRender;

    /// <summary>
    /// Fired once per frame. Only for this game objects's components.
    /// </summary>
    public event Events.UpdateEvent OnUpdate;

    #endregion Events

    #region Indexers

    /// <summary>
    /// Get a component by name.
    /// </summary>
    /// <param name="name">Component name</param>
    /// <returns>Requested component</returns>
    public Component this[string name] {
      get {
        return GetComponent(name);
      }
    }

    #endregion Indexers

    #region Fields

    /// <summary>
    /// Dictionary of components.
    /// </summary>
    private Dictionary<string, Component> mComponents = new Dictionary<string, Component>();

    #endregion Fields
  }

}