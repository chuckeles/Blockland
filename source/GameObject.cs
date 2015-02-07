using System;
using System.Collections.Generic;

namespace Blockland {

  /// <summary>
  /// Game object is a container for components.
  /// </summary>
  public class GameObject {

    #region Methods

    /// <summary>
    /// Add a new component to the game object.
    /// </summary>
    /// <param name="component">Component to add</param>
    /// <exception cref="Exception">When it already contains component</exception>
    public void AddComponent(Component component) {
      if (HasComponent(component.Name))
        throw new Exception("Component already added");

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

    #endregion Methods

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