using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;
using System;

namespace Blockland {

  /// <summary>
  /// Represents OpenGL rendering window.
  /// </summary>
  public class Window {

    #region Constructor

    /// <summary>
    /// Create a new window.
    /// </summary>
    /// <param name="title">Window title</param>
    /// <param name="width">Window width, in pixels</param>
    /// <param name="height">Window height, in pixels</param>
    /// <exception cref="Exception">When the window already exists.</exception>
    public Window(string title, uint width = 800, uint height = 600) {
      if (mInstance != null)
        throw new Exception("Window instance already exists");

      mWidth = width;
      mHeight = height;

      mWindow = new NativeWindow((int)width, (int)height, "Blockland", GameWindowFlags.Default, GraphicsMode.Default, DisplayDevice.Default);
      mContext = new GraphicsContext(GraphicsMode.Default, mWindow.WindowInfo, 4, 4, GraphicsContextFlags.Default);
      mContext.MakeCurrent(mWindow.WindowInfo);
      mContext.LoadAll();

      mWindow.Visible = true;
      mWindow.WindowBorder = WindowBorder.Fixed;

      GL.Enable(EnableCap.CullFace);
      GL.Enable(EnableCap.DepthTest);
      GL.ClearColor(0f, 0f, 0f, 0f);

      Program.Events.OnClear += Clear;
      Program.Events.OnUpdate += Update;
      Program.Events.OnDisplay += Display;

      mInstance = this;
    }

    /// <summary>
    /// Destructor.
    /// </summary>
    ~Window() {
      Program.Events.OnDisplay -= Display;
      Program.Events.OnUpdate -= Update;
      Program.Events.OnClear -= Clear;
    }

    #endregion Constructor

    #region Methods

    /// <summary>
    /// Create a new window.
    /// </summary>
    /// <param name="title">Window title</param>
    /// <param name="width">Window width, in pixels</param>
    /// <param name="height">Window height, in pixels</param>
    /// <exception cref="Exception">When the window already exists.</exception>
    public static Window Create(string title, uint width = 800, uint height = 600) {
      return new Window(title, width, height);
    }

    /// <summary>
    /// Set the mouse position to the center of the window.
    /// </summary>
    public void CenterMouse() {
      Mouse.SetPosition(mWindow.X + mWidth / 2, mWindow.Y + mHeight / 2);
    }

    /// <summary>
    /// Clear the OpenGL buffers.
    /// </summary>
    public void Clear() {
      GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
    }

    /// <summary>
    /// Close the window and destroy the instance.
    /// </summary>
    public void Close() {
      mWindow.Close();

      Program.Events.OnUpdate -= Update;

      mInstance = null;
    }

    /// <summary>
    /// Swap the OpenGL buffers.
    /// </summary>
    public void Display() {
      mContext.SwapBuffers();
    }

    /// <summary>
    /// Tell OpenGL to render bound buffers.
    /// </summary>
    /// <param name="count">Number of elements</param>
    public void DrawTriangles(int count) {
      GL.DrawElements(BeginMode.Triangles, count, DrawElementsType.UnsignedInt, 0);
    }

    /// <summary>
    /// Process window events.
    /// </summary>
    /// <param name="deltaTime">Delta time</param>
    private void Update(float deltaTime) {
      mWindow.ProcessEvents();
    }

    #endregion Methods

    #region Properties

    /// <summary>
    /// Get the window instance.
    /// </summary>
    public static Window Instance {
      get {
        return mInstance;
      }
    }

    /// <summary>
    /// Get the window height.
    /// </summary>
    public uint Height {
      get {
        return mHeight;
      }
    }

    /// <summary>
    /// Get or set whether the mouse cursor is visible.
    /// </summary>
    public bool MouseVisible {
      get {
        return mWindow.CursorVisible;
      }
      set {
        mWindow.CursorVisible = value;
      }
    }

    /// <summary>
    /// Get underlying native window.
    /// </summary>
    public NativeWindow NativeWindow {
      get {
        return mWindow;
      }
    }

    /// <summary>
    /// Check if the window is open.
    /// </summary>
    public bool Open {
      get {
        return mWindow.Exists;
      }
    }

    /// <summary>
    /// Get window width.
    /// </summary>
    public uint Width {
      get {
        return mWidth;
      }
    }

    #endregion Properties

    #region Fields

    /// <summary>
    /// Global window instance.
    /// </summary>
    private static Window mInstance;

    /// <summary>
    /// OpenGL context.
    /// </summary>
    private GraphicsContext mContext;

    /// <summary>
    /// Window height.
    /// </summary>
    private uint mHeight = 0;

    /// <summary>
    /// Window width.
    /// </summary>
    private uint mWidth = 0;

    /// <summary>
    /// OpenTK native window.
    /// </summary>
    private NativeWindow mWindow;

    #endregion Fields

  }

}