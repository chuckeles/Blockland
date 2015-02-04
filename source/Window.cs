using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;
using System;

namespace Blockland {

  public class Window {

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

      mInstance = this;
    }

    public void Close() {
      mWindow.Close();

      mInstance = null;
    }

    public void ProcessEvents() {
      mWindow.ProcessEvents();
    }

    public void Clear() {
      GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
    }

    public void Display() {
      mContext.SwapBuffers();
    }

    public void CenterMouse() {
      Mouse.SetPosition(mWindow.X + mWidth / 2, mWindow.Y + mHeight / 2);
    }

    public bool MouseVisible {
      get {
        return mWindow.CursorVisible;
      }
      set {
        mWindow.CursorVisible = value;
      }
    }

    public bool Open {
      get {
        return mWindow.Exists;
      }
    }

    public uint Width {
      get {
        return mWidth;
      }
    }

    public uint Height {
      get {
        return mHeight;
      }
    }

    public NativeWindow NativeWindow {
      get {
        return mWindow;
      }
    }

    public static Window Instance {
      get {
        return mInstance;
      }
    }

    private NativeWindow mWindow;
    private GraphicsContext mContext;

    private uint mWidth = 0;
    private uint mHeight = 0;

    private static Window mInstance;

  }

}