using Rhino;
using Rhino.DocObjects;
using Rhino.DocObjects.Custom;
using Rhino.FileIO;
using Rhino.Geometry;

namespace RhinoPlugin_00
{
    /// <summary>
    /// Extruded profile data
    /// </summary>
    [System.Runtime.InteropServices.Guid("98EA4A7D-3EFB-4182-9604-5ED04AE46658")]
  public class CustomUserData : UserData
  {
      private Surface _extrusion;

      private CustomUserDataConduit _customDisplayConduit;

      public CustomUserData()
      {
          _customDisplayConduit = new CustomUserDataConduit();
      }

      public Surface Extrusion
      {
          get => _extrusion;
          set
          {
              _extrusion = value;
              _customDisplayConduit.Extrusion = _extrusion;
          } 
      }

      /// <summary>
      /// Descriptive name of the user data.
      /// </summary>
      public override string Description => "Mobile Plane User Data";

      /// <summary>
      /// Return true to serialize to 3dm files.
      /// </summary>
      public override bool ShouldWrite => true;

      /// <summary>
    /// Called when the object is being duplicated.
    /// </summary>
    protected override void OnDuplicate(UserData source)
    {
            if (source is CustomUserData src)
            {
                Extrusion = src.Extrusion.Duplicate() as Surface;
                _customDisplayConduit.Extrusion = Extrusion;
            }
        }

    /// <summary>
    /// Called when the object associated with this data is transformed.
    /// </summary>
    protected override void OnTransform(Transform xform)
    {
        base.OnTransform(xform); //call to update xform on base. is this really necessary?
        Extrusion.Transform(xform);
    }

    /// <summary>
    /// Reads the content of this data from a stream archive.
    /// </summary>
    protected override bool Read(BinaryArchiveReader archive)
    {
      archive.Read3dmChunkVersion(out int major, out int minor);
      if (1 == major && 0 == minor)
      {
        Extrusion = (Extrusion) archive.ReadGeometry();
        _customDisplayConduit.Enabled = archive.ReadBool();
      }
      return !archive.ReadErrorOccured;
    }

    /// <summary>
    /// Writes the content of this data to a stream archive.
    /// </summary>
    protected override bool Write(BinaryArchiveWriter archive)
    {
      archive.Write3dmChunkVersion(1, 0);
      archive.WriteGeometry(Extrusion);
      archive.WriteBool(_customDisplayConduit.Enabled);
      return !archive.WriteErrorOccured;
    }

    /// <summary>
    /// Toggles display of custom geometry.
    /// </summary>
    public void ToggleDisplay()
    {
        var toggle = !_customDisplayConduit.Enabled;
        _customDisplayConduit.Enabled = toggle;
    }

    /// <summary>
    /// Verifies data this type of data is attached to a Rhino object.
    /// </summary>
    public static bool IsAttached(RhinoObject obj)
    {
        bool rc = false;
        if (null != obj)
        {
            CustomUserData data = DataFromObject(obj);
            rc = null != data;
        }
        return rc;
    }

    /// <summary>
    /// Attaches a mobile plane to a Rhino object
    /// </summary>
    public static bool Attach(RhinoObject obj, Surface extrusion)
    {
        bool rc = false;
        if (null != obj)
        {
            if (IsAttached(obj))
            {
                CustomUserData data = DataFromObject(obj);
                data.Extrusion = extrusion;
                rc = true;
            }
            else
            { 
                CustomUserData data = new CustomUserData {Extrusion = extrusion};
                rc = obj.Geometry.UserData.Add(data);
            }
        }
        return rc;
    }

    ///// <summary>
    ///// Refreshes conduit
    ///// </summary>
    //public void RefreshConduit()
    //{
    //    _customDisplayConduit.Enabled = false;
    //    RhinoDoc.ActiveDoc.Views.Redraw();
    //    _customDisplayConduit.Enabled = true;
    //    RhinoDoc.ActiveDoc.Views.Redraw();
    //}

    /// <summary>
    /// Gets the mobile plane user data from a Rhino object
    /// </summary>
        public static CustomUserData DataFromObject(RhinoObject obj)
    {
      CustomUserData rc = null;
      if (null != obj)
      {
          rc = obj.Geometry.UserData.Find(typeof(CustomUserData)) as CustomUserData;
      }

      return rc;
    }
  }
}
