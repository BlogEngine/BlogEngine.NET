#region Using

using System;
using System.IO;
using System.Collections.Generic;
using System.Xml;
using BlogEngine.Core.Providers;

#endregion

namespace BlogEngine.Core
{

  /// <summary>
  /// A dictionary for all Post categories.
  /// </summary>
  [Serializable]
  public class CategoryDictionary : Dictionary<Guid, string>
  {

    internal static string _Folder = System.Web.HttpContext.Current.Server.MapPath(BlogSettings.Instance.StorageLocation);

    #region Properties

    private static CategoryDictionary _Instance = Load();

    /// <summary>
    /// Gets the singleton instance of the class.
    /// </summary>
    /// <value>An instance of CategoryDictionary.</value>
    public static CategoryDictionary Instance
    {
      get { return _Instance; }
    }

    #endregion

    #region Methods

    /// <summary>
    /// Adds a new category to the dictionary.
    /// </summary>
    /// <returns>The id of the new category.</returns>
    public Guid Add(string title)
    {
      Guid id = Guid.NewGuid();
      this.Add(id, title);
      return id;
    }

    /// <summary>
    /// Save the categories.
    /// </summary>
    public void Save()
    {
      //BlogService.SaveCategories(this);
      OnSaved();
    }

    private static CategoryDictionary Load()
    {
     // return BlogService.SelectCategories(); 
        return null;
    }

    #endregion

    /// <summary>
    /// Occurs when the class is Saved
    /// </summary>
    public static event EventHandler<EventArgs> Saved;    
    private static void OnSaved()
    {
      if (Saved != null)
      {
        Saved(null, new EventArgs());
      }
    }
        

  }
}
