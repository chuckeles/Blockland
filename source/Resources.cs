using System.Collections.Generic;

namespace Blockland {

  /// <summary>
  /// Resource loader and manager.
  /// </summary>
  public class Resources {

    #region Methods

    /// <summary>
    /// Loads all the necessary resources from the disk.
    /// </summary>
    public void LoadAll() {
      // TODO
    }

    /// <summary>
    /// Get a resource.
    /// </summary>
    /// <param name="name">Name of the resource</param>
    /// <returns>Requested resource</returns>
    public IResource this[string name] {
      get {
        return mResources[name];
      }
    }

    #endregion Methods

    #region Fields

    /// <summary>
    /// Loaded resources.
    /// </summary>
    private static Dictionary<string, IResource> mResources = new Dictionary<string, IResource>();

    #endregion Fields

  }

}