using OpenTK.Input;

namespace Blockland {

  /// <summary>
  /// Structure that holds global events.
  /// </summary>
  public struct Events {

    #region Delegates

    /// <summary>
    /// Generic event handler without parameters.
    /// </summary>
    public delegate void Event();

    /// <summary>
    /// Key event is fired when a key is pressed or released.
    /// </summary>
    /// <param name="key"></param>
    public delegate void KeyEvent(KeyboardKeyEventArgs key);

    /// <summary>
    /// Mouse button event is fired when a mouse button is pressed or released.
    /// </summary>
    /// <param name="button"></param>
    public delegate void MouseButtonEvent(MouseButtonEventArgs button);

    /// <summary>
    /// Update event is fired once per frame.
    /// </summary>
    /// <param name="deltaTime">Time that has passed since the last frame</param>
    public delegate void UpdateEvent(float deltaTime);

    #endregion Delegates

    #region Methods

    /// <summary>
    /// Fire the clear event.
    /// </summary>
    public void Clear() {
      if (OnClear != null)
        OnClear();
    }

    /// <summary>
    /// Fire the display event.
    /// </summary>
    public void Display() {
      if (OnDisplay != null)
        OnDisplay();
    }

    /// <summary>
    /// Fire the end event.
    /// </summary>
    public void End() {
      if (OnEnd != null)
        OnEnd();
    }

    /// <summary>
    /// Fire the key down event.
    /// </summary>
    /// <param name="key">Key event</param>
    public void KeyDown(KeyboardKeyEventArgs key) {
      if (OnKeyDown != null)
        OnKeyDown(key);
    }

    /// <summary>
    /// Fire the key up event.
    /// </summary>
    /// <param name="key">Key event</param>
    public void KeyUp(KeyboardKeyEventArgs key) {
      if (OnKeyUp != null)
        OnKeyUp(key);
    }

    /// <summary>
    /// Fire the mouse button down event.
    /// </summary>
    /// <param name="button">Mouse button event.</param>
    public void MouseButtonDown(MouseButtonEventArgs button) {
      if (OnMouseButtonDown != null)
        OnMouseButtonDown(button);
    }

    /// <summary>
    /// Fire the mouse button up event.
    /// </summary>
    /// <param name="button">Mouse button event.</param>
    public void MouseButtonUp(MouseButtonEventArgs button) {
      if (OnMouseButtonUp != null)
        OnMouseButtonUp(button);
    }

    /// <summary>
    /// Fire the render event.
    /// </summary>
    public void Render() {
      if (OnRender != null)
        OnRender();
    }

    /// <summary>
    /// Fire the start event.
    /// </summary>
    public void Start() {
      if (OnStart != null)
        OnStart();
    }

    /// <summary>
    /// Fire the update event.
    /// </summary>
    /// <param name="deltaTime">Delta time since last update</param>
    public void Update(float deltaTime) {
      if (OnUpdate != null)
        OnUpdate(deltaTime);
    }

    #endregion Methods

    #region Events

    /// <summary>
    /// Clear event is fired before any rendering. Only the rendering window should respond to this event.
    /// </summary>
    public event Event OnClear;

    /// <summary>
    /// Display event is fired after all the rendering. Only the rendering window should respond to
    /// this event.
    /// </summary>
    public event Event OnDisplay;

    /// <summary>
    /// Fired when current state is ending.
    /// </summary>
    public event Event OnEnd;

    /// <summary>
    /// Fired when a key is pressed.
    /// </summary>
    public event KeyEvent OnKeyDown;

    /// <summary>
    /// Fired when a key is released.
    /// </summary>
    public event KeyEvent OnKeyUp;

    /// <summary>
    /// Fired when a mouse button is pressed.
    /// </summary>
    public event MouseButtonEvent OnMouseButtonDown;

    /// <summary>
    /// Fired when a mouse button is released.
    /// </summary>
    public event MouseButtonEvent OnMouseButtonUp;

    /// <summary>
    /// Render event is fired once per frame after the update event. Objects can draw to the window.
    /// </summary>
    public event Event OnRender;

    /// <summary>
    /// Fired when a new state is starting.
    /// </summary>
    public event Event OnStart;

    /// <summary>
    /// Update event is fired once per frame.
    /// </summary>
    public event UpdateEvent OnUpdate;

    #endregion Events

  }

}