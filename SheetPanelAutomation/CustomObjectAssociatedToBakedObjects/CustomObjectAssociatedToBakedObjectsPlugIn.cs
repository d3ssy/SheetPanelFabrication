﻿
using Rhino;
using Rhino.DocObjects;
using Rhino.Geometry;
using Rhino.PlugIns;

namespace CustomObjectAssociatedToBakedObjects
{
    ///<summary>
    /// <para>Every RhinoCommon .rhp assembly must have one and only one PlugIn-derived
    /// class. DO NOT create instances of this class yourself. It is the
    /// responsibility of Rhino to create an instance of this class.</para>
    /// <para>To complete plug-in information, please also see all PlugInDescription
    /// attributes in AssemblyInfo.cs (you might need to click "Project" ->
    /// "Show All Files" to see it in the "Solution Explorer" window).</para>
    ///</summary>
    public class CustomObjectAssociatedToBakedObjectsPlugIn : PlugIn
    {
        public CustomObjectAssociatedToBakedObjectsPlugIn()
        {
            Instance = this;
            RhinoDoc.ModifyObjectAttributes += OnModifyObjectAttributes;
            RhinoDoc.BeforeTransformObjects += OnBeforeTransformObjects;
            RhinoDoc.ReplaceRhinoObject += ReplaceRhinoObject;
            RhinoDoc.AddRhinoObject += AddBox;
        }

        ///<summary>Gets the only instance of the RhinoCommonTestPlugIn plug-in.</summary>
        public static CustomObjectAssociatedToBakedObjectsPlugIn Instance { get; private set; }

        public override PlugInLoadTime LoadTime => PlugInLoadTime.AtStartup;
        protected override LoadReturnCode OnLoad(ref string errorMessage)
        {
            return LoadReturnCode.Success;
        }

        // You can override methods here to change the plug-in behavior on
        // loading and shut down, add options pages to the Rhino _Option command
        // and maintain plug-in wide options in a document.

        private void OnModifyObjectAttributes(object sender, RhinoModifyObjectAttributesEventArgs e)
        {
            if (!(e.RhinoObject is CustomObjectAssociateToBakedObject customGeo)) return;
            customGeo.UpdateExtrudedProfile();
        }

        private void OnBeforeTransformObjects(object sender, RhinoTransformObjectsEventArgs e)
        {
            foreach (RhinoObject rhinoObject in e.GripOwners)
            {
                if (!(rhinoObject is CustomObjectAssociateToBakedObject customGeo)) continue;

                var grips = customGeo.GetGrips();
                foreach (GripObject grip in grips)
                {
                    if (grip.OriginalLocation == customGeo.CentreLine.From)
                    {
                        customGeo.CentreLine = new Line(grip.CurrentLocation, customGeo.CentreLine.To);
                        RhinoApp.WriteLine($"Transformed the start point -> {grip.CurrentLocation}");
                    }

                    if (grip.OriginalLocation == customGeo.CentreLine.To)
                    {
                        customGeo.CentreLine = new Line(customGeo.CentreLine.From, grip.CurrentLocation);
                        RhinoApp.WriteLine($"Transformed the end point -> {grip.CurrentLocation}");
                    }
                }
            }
        }

        private void ReplaceRhinoObject(object sender, RhinoReplaceObjectEventArgs e)
        {
            if (!(e.OldRhinoObject is CustomObjectAssociateToBakedObject)) return;
            var oldObj = e.OldRhinoObject as CustomObjectAssociateToBakedObject;
            var newLine = e.NewRhinoObject.Geometry as Curve;
            double width = double.Parse(e.OldRhinoObject.Attributes.GetUserString("Width"));
            double height = double.Parse(e.OldRhinoObject.Attributes.GetUserString("Height"));
            CustomObjectAssociateToBakedObject customObject = new CustomObjectAssociateToBakedObject(new Line(newLine.PointAtStart, newLine.PointAtEnd), width, height);
            ObjRef geoRef = new ObjRef(e.OldRhinoObject);
            e.Document.Objects.Delete(oldObj.ObjId, true);
            e.Document.Objects.Delete(geoRef, true, false);
            e.Document.Objects.AddRhinoObject(customObject, null);
        }

        private void AddBox(object sender, RhinoObjectEventArgs e)
        {
            if (!(e.TheObject is CustomObjectAssociateToBakedObject customGeo)) return;
            customGeo.ObjId = e.TheObject.Document.Objects.AddBrep(customGeo.ExtrudedProfile, null);
        }
    }
}