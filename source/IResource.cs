using System;

namespace Blockland {

  /// <summary>
  /// Represents a resource that must be loaded from the harddrive.
  /// </summary>
  public interface IResource
    : IDisposable {

    #region Methods

    /// <summary>
    /// Load the resource.
    /// </summary>
    /// <param name="file">Name of the file from which to load</param>
    void Load(string file);

    #endregion Methods

    #region Properties

    /// <summary>
    /// Check if the resource is loaded.
    /// </summary>
    bool Loaded {
      get;
    }

    /// <summary>
    /// Get the resource name.
    /// </summary>
    string Name {
      get;
    }

    #endregion Properties

  }

}