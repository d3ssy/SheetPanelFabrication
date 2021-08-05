using Rhino;
using Rhino.DocObjects;
using Rhino.PlugIns;

namespace RhinoPlugin_00
{
  public class CustomUserDataWithConduitPlugIn : PlugIn
  {
    /// <summary>
    /// Constructor
    /// </summary>
    public CustomUserDataWithConduitPlugIn()
    {
      Instance = this;
    }

    /// <summary>
    /// Gets the only instance of the MobilePlanePlugIn plug-in.
    /// </summary>
    public static CustomUserDataWithConduitPlugIn Instance
    {
      get;
      private set;
    }

    /// <summary>
    /// Called by Rhino when loading this plug-in.
    /// </summary>
    protected override LoadReturnCode OnLoad(ref string errorMessage)
    {
      RhinoDoc.UndeleteRhinoObject += OnUndeleteRhinoObject;
      return LoadReturnCode.Success;
    }

    /// <summary>
    /// Called if an object is undeleted (e.g. Undo)
    /// </summary>
    void OnUndeleteRhinoObject(object sender, RhinoObjectEventArgs e)
    {
        var ud = CustomUserData.DataFromObject(e.TheObject);
    }
  }
}