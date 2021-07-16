using Rhino;
using Rhino.Display;
using Rhino.DocObjects;
using Rhino.DocObjects.Custom;
using Rhino.Geometry;
using System.Drawing;

namespace CustomObject.PlugIn
{
    public class CustomGeo : CustomCurveObject
    {
        private double Width;
        private double Height;
        private Line DatumLine;

        public CustomGeo() : base()
        {
        }

        public CustomGeo(Line datumLine, double width, double height) : base(datumLine.ToNurbsCurve())
        {
            Width = width;
            DatumLine = datumLine;
            Height = height;
            Attributes.SetUserString("Width", width.ToString());
            Attributes.SetUserString("Height", height.ToString());
        }

        protected override void OnDraw(DrawEventArgs e)
        {
            SetRhinoEvent();
            var material = new DisplayMaterial(Color.FromArgb(230, 180, 60), 0.2);
            material.BackDiffuse = Color.FromArgb(230, 180, 60);
            material.IsTwoSided = true;

            Plane pl = new Plane(DatumLine.From, DatumLine.Direction);
            Rectangle3d rec = new Rectangle3d(pl, Width, Height);
            Brep[] planarBrep = Brep.CreatePlanarBreps(rec.ToNurbsCurve(), Rhino.RhinoMath.ZeroTolerance);
            var box = planarBrep[0].Faces[0].CreateExtrusion(DatumLine.ToNurbsCurve(), true);

            var m = new Mesh();
            var meshs = Mesh.CreateFromBrep(box, MeshingParameters.Default);
            m.Append(meshs);
            e.Display.DrawBrepWires(box, this.Attributes.DrawColor(e.RhinoDoc), this.Attributes.WireDensity);
            e.Display.DrawMeshShaded(m, material);
            base.OnDraw(e);
        }

        protected override void OnDuplicate(RhinoObject source)
        {
            RhinoApp.WriteLine("OnDuplicate");
            base.OnDuplicate(source);
            if (source is CustomGeo rhe)
            {
                DatumLine = rhe.DatumLine;
                Width = rhe.Width;
                Height = rhe.Height;
                this.SetCurve(rhe.DatumLine.ToNurbsCurve());
            }
        }
        protected override void OnTransform(Transform transform)
        {
            RhinoApp.WriteLine("OnTransform");
            base.OnTransform(transform);
            DatumLine.Transform(transform);
            this.CurveGeometry.Transform(transform);
        }

        private void SetRhinoEvent()
        {
            RhinoDoc.ModifyObjectAttributes += OnModifyObjectAttributes;
            RhinoDoc.BeforeTransformObjects += OnBeforeTransformObjects;
        }

        private void OnModifyObjectAttributes(object sender, RhinoModifyObjectAttributesEventArgs e)
        {
            if (e.NewAttributes == e.OldAttributes) return;
            // Is this the correct way to filter the object?
            if (e.RhinoObject.RuntimeSerialNumber != this.RuntimeSerialNumber) return;
            // Assigning the new values.
            string width = e.NewAttributes.GetUserString("Width");
            Width = double.Parse(width);
            string height = e.NewAttributes.GetUserString("Height");
            Height = double.Parse(height);
        }

        public void OnBeforeTransformObjects(object sender, RhinoTransformObjectsEventArgs e)
        {
            // It changes the point position but doesn't fire the OnDraw.
            RhinoApp.WriteLine($"OnTransformation");
            if (e.GripOwnerCount <= 0) return;
            foreach (GripObject grip in e.Grips)
            {
                if (grip.OriginalLocation == DatumLine.From)
                {
                    DatumLine.From = grip.CurrentLocation;
                }

                if (grip.OriginalLocation == DatumLine.To)
                {
                    DatumLine.To = grip.CurrentLocation;
                }
            }
        }
    }
}
