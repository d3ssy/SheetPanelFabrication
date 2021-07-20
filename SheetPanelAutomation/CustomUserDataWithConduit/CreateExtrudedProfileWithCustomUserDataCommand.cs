using Rhino;
using Rhino.Commands;
using Rhino.DocObjects;
using Rhino.Geometry;
using Rhino.Input;
using Rhino.Input.Custom;
using System.Collections.Generic;
using System.Linq;

namespace RhinoPlugin_00
{
    [System.Runtime.InteropServices.Guid("98EA4A7D-3EFB-4182-9604-5ED04AE46658")]
  public class CreateExtrudedProfileWithCustomUserDataCommand : Command
  {
    /// <summary>
    /// The command name as it appears on the Rhino command line.
    /// </summary>
    public override string EnglishName => "CreateCustomExtrudedProfileWithAttachedUserData";

    /// <summary>
    /// Called by Rhino when the user runs the command.
    /// </summary>
    protected override Result RunCommand(RhinoDoc doc, RunMode mode)
    {
        GetLine go = new GetLine();
        go.Get(out Line line);

        System.Guid id = RhinoDoc.ActiveDoc.Objects.Add(line.ToNurbsCurve(), new ObjectAttributes());
        ObjRef objRef = new ObjRef(RhinoDoc.ActiveDoc, id);
        RhinoObject obj = objRef.Object();

        GetOption gt = new GetOption();
        gt.SetCommandPrompt("Choose profile option");

        int shs_index = gt.AddOption("SHS");
        int chs_index = gt.AddOption("CHS");

        for (;;)
        {
            GetResult res = gt.Get();
            if (res != GetResult.Option)
            {
                break;
            }

            Result rc = Result.Cancel;
            int index = gt.OptionIndex();

            if (index == shs_index)
            {
                rc = ShsOption(line, obj)? Result.Success : Result.Failure;
            }
            else if (index == chs_index)
            {
                rc = ChsOption(line, obj) ? Result.Success : Result.Failure;
            }

            if (rc != Result.Success)
            {
                break;
            }
        }
  
        doc.Views.Redraw();
        return Result.Success;
    }

    /// <summary>
    /// Attaches a mobile plane to a Rhino object
    /// </summary>
    private bool ShsOption(Line line, RhinoObject obj)
    {
            Surface extrusion = CreateExtrusion(line, ExtrusionProfile.Shs);
            bool rc = CustomUserData.Attach(obj, extrusion);

            return extrusion.IsValid;
    }

    /// <summary>
    /// Detaches a mobile plane from a Rhino object
    /// </summary>
    private bool ChsOption(Line line, RhinoObject obj)
    {
        Surface extrusion = CreateExtrusion(line, ExtrusionProfile.Chs);
        bool rc = CustomUserData.Attach(obj, extrusion);
        return extrusion.IsValid;
    }

    private Surface CreateExtrusion(Line line, ExtrusionProfile profile)
    {
        Curve crv = line.ToNurbsCurve();
        crv.PerpendicularFrameAt(0, out var plane);

        Circle circle = new Circle(plane, 50);
        Rectangle3d rectangle = new Rectangle3d(plane, new Interval(-100, 100), new Interval(-100, 100));
        Curve profileCrv = new NurbsCurve(default);

        if (profile == ExtrusionProfile.Chs)
            {
                profileCrv = circle.ToNurbsCurve();
            }

            if (profile == ExtrusionProfile.Shs)
            {
                profileCrv = rectangle.ToNurbsCurve();
            }

            Surface extrusion = Surface.CreateExtrusion(profileCrv, line.Direction);
        return extrusion;
    }
  }
}
