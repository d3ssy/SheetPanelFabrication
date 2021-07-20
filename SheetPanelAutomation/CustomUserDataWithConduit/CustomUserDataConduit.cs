using System.Drawing;
using Rhino;
using Rhino.ApplicationSettings;
using Rhino.Display;
using Rhino.DocObjects.Custom;
using Rhino.Geometry;

namespace RhinoPlugin_00
{
  /// <summary>
  /// MobilePlaneConduit
  /// </summary>
  public class CustomUserDataConduit : DisplayConduit
  {
    public Surface Extrusion { get; set; }

    /// <summary>
    /// Draw 
    /// </summary>
    /// <param name="e"></param>
    protected override void PostDrawObjects(DrawEventArgs e)
    {
        e.Display.DrawSurface(Extrusion, Color.Aqua, 2);
    }
  }
}
