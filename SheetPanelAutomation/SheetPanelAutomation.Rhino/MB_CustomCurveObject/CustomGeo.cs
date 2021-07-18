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
        private double Width => double.Parse(this.Attributes.GetUserString("Width"));
        private double Height => double.Parse(this.Attributes.GetUserString("Height"));
        public Line DatumLine;

        public CustomGeo() : base()
        {
        }

        public CustomGeo(Line datumLine, double width, double height) : base(datumLine.ToNurbsCurve())
        {
            DatumLine = datumLine;
            Attributes.SetUserString("Width", width.ToString());
            Attributes.SetUserString("Height", height.ToString());
        }

        protected override void OnDraw(Rhino.Display.DrawEventArgs e)
        {
            base.OnDraw(e);
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
            e.Display.DrawBrepWires(box, Attributes.DrawColor(e.RhinoDoc), Attributes.WireDensity);
            e.Display.DrawMeshShaded(m, material);
        }

        protected override void OnDuplicate(RhinoObject source)
        {
            RhinoApp.WriteLine("OnDuplicate");
            base.OnDuplicate(source);
            if (source is CustomGeo rhe)
            {
                DatumLine = rhe.DatumLine;
                Attributes.SetUserString("Width", rhe.Width.ToString());
                Attributes.SetUserString("Height", rhe.Height.ToString());
                SetCurve(rhe.DatumLine.ToNurbsCurve());
            }
        }
        protected override void OnTransform(Transform transform)
        {
            RhinoApp.WriteLine("OnTransform");
            base.OnTransform(transform);
            DatumLine.Transform(transform);
            CurveGeometry.Transform(transform);
        }
    }
}
