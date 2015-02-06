using System.Collections.Generic;

namespace Blockland {

  /// <summary>
  /// Resource loader and manager.
  /// </summary>
  public class Resources {

    #region Methods

    /// <summary>
    /// Get a resource.
    /// </summary>
    /// <param name="name">Name of the resource</param>
    /// <returns>Requested resource</returns>
    public static IResource Get(string name) {
      return mResources[name];
    }

    /// <summary>
    /// Loads all the necessary resources from the disk.
    /// </summary>
    public static void LoadAll() {
      // TODO
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