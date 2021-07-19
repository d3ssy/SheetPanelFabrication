using System.Runtime.InteropServices;
using Rhino;
using Rhino.Commands;
using Rhino.DocObjects;
using Rhino.Geometry;
using Rhino.PlugIns;

namespace CustomObject.PlugIn
{
    ///<summary>
    /// <para>Every RhinoCommon .rhp assembly must have one and only one PlugIn-derived
    /// class. DO NOT create instances of this class yourself. It is the
    /// responsibility of Rhino to create an instance of this class.</para>
    /// <para>To complete plug-in information, please also see all PlugInDescription
    /// attributes in AssemblyInfo.cs (you might need to click "Project" ->
    /// "Show All Files" to see it in the "Solution Explorer" window).</para>
    ///</summary>
    [Guid("61055a13-cd6d-4c5d-8978-0ee9a0560837")]
    public class CreatesExtrusionPlugIn : Rhino.PlugIns.PlugIn
    {
        public CreatesExtrusionPlugIn()
        {
            Instance = this;
            RhinoDoc.ModifyObjectAttributes += OnModifyObjectAttributes;
            RhinoDoc.BeforeTransformObjects += OnBeforeTransformObjects;
            Command.EndCommand += EndCommand;
        }

        ///<summary>Gets the only instance of the RhinoCommonTestPlugIn plug-in.</summary>
        public static CreatesExtrusionPlugIn Instance { get; private set; }

        protected override LoadReturnCode OnLoad(ref string errorMessage)
        {
            return LoadReturnCode.Success;
        }

        // You can override methods here to change the plug-in behavior on
        // loading and shut down, add options pages to the Rhino _Option command
        // and maintain plug-in wide options in a document.

        private void OnModifyObjectAttributes(object sender, RhinoModifyObjectAttributesEventArgs e)
        {
        }

        private void OnBeforeTransformObjects(object sender, RhinoTransformObjectsEventArgs e)
        {
            foreach (RhinoObject rhinoObject in e.GripOwners)
            {
                if (!(rhinoObject is CustomGeo customGeo)) continue;

                var grips = customGeo.GetGrips();
                foreach (GripObject grip in grips)
                {
                    if (grip.OriginalLocation == customGeo.Centreline.From)
                    {
                        customGeo.Centreline = new Line(grip.CurrentLocation, customGeo.Centreline.To);
                        RhinoApp.WriteLine($"Transformed the start point -> {grip.CurrentLocation}");
                    }

                    if (grip.OriginalLocation == customGeo.Centreline.To)
                    {
                        customGeo.Centreline = new Line(customGeo.Centreline.From, grip.CurrentLocation);
                        RhinoApp.WriteLine($"Transformed the end point -> {grip.CurrentLocation}");
                    }
                }
            }
        }

        private void EndCommand(object sender, CommandEventArgs e)
        {
            var doc = e.Document;
            var objs = doc.Objects.FindByObjectType(ObjectType.Curve);
            foreach (RhinoObject rhinoObject in objs)
            {
                if (!(rhinoObject.Geometry is Curve crv)) continue;
                Line datumLine = new Line(crv.PointAtStart, crv.PointAtEnd);
                double width = double.Parse(rhinoObject.Attributes.GetUserString("Width"));
                double height = double.Parse(rhinoObject.Attributes.GetUserString("Height"));

                ObjRef geoRef = new ObjRef(rhinoObject);
                CustomGeo geo = new CustomGeo(datumLine, width, height);
                _ = doc.Objects.Replace(geoRef, geo);
            }
        }
    }
}
