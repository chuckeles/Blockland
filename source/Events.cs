using System;

namespace Blockland {

  /// <summary>
  /// Structure that holds global events.
  /// </summary>
  public struct Events {

    #region Delegates

    /// <summary>
    /// Render event is fire once per frame after the update event. Objects can draw to the window.
    /// </summary>
    public delegate void RenderEvent();

    /// <summary>
    /// Update event is fired once per frame.
    /// </summary>
    /// <param name="deltaTime">Time that has passed since the last frame</param>
    public delegate void UpdateEvent(float deltaTime);

    #endregion Delegates

    #region Events

    /// <summary>
    /// Render event is fire once per frame after the update event. Objects can draw to the window.
    /// </summary>
    public event RenderEvent OnRender;

    /// <summary>
    /// Update event is fired once per frame.
    /// </summary>
    public event UpdateEvent OnUpdate;

    #endregion Events

  }

}