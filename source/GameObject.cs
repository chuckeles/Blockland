using System;
using System.Collections.Generic;

namespace Blockland {

  public delegate void UpdateEventHandler(float deltaTime);
  public delegate void DrawEventHandler();

  public class GameObject {

    public GameObject() {
      mComponents = new Dictionary<string, Component>();
    }

    public void AddComponent(Component component) {
      if (mComponents.ContainsKey(component.Name))
        throw new Exception("Component already added");

      mComponents.Add(component.Name, component);
      component.Attached(this);
    }

    public bool HasComponent(string name) {
      return mComponents.ContainsKey(name);
    }

    public Component GetComponent(string name) {
      return mComponents[name];
    }

    public void RemoveComponent(string name) {
      mComponents.Remove(name);
    }

    public void EnsureTransform() {
      if (!HasComponent("Transform"))
        AddComponent(new Transform());
    }

    public void Update(float deltaTime) {
      if (OnUpdate != null)
        OnUpdate(deltaTime);
    }

    public void Draw() {
      if (OnDraw != null)
        OnDraw();
    }

    public Component this[string name] {
      get {
        return GetComponent(name);
      }
    }

    public event UpdateEventHandler OnUpdate;
    public event DrawEventHandler OnDraw;

    private Dictionary<string, Component> mComponents;

  }

}