using Rhino;
using Rhino.Display;
using Rhino.DocObjects;
using Rhino.DocObjects.Custom;
using Rhino.Geometry;
using System;
using System.Drawing;
using System.Linq;

namespace CustomObject.PlugIn
{
    public class CustomGeo : CustomCurveObject
    {
        private Line _centreline;

        private Brep _extrudedProfile;

        public CustomGeo()
        {
            DisplayPipeline.CalculateBoundingBox += AddBBox;
        }

        public CustomGeo(Line centreline, double width, double height) : base(centreline.ToNurbsCurve())
        {
            _centreline = centreline;
            Attributes.SetUserString("Width", width.ToString());
            Attributes.SetUserString("Height", height.ToString());
            _extrudedProfile = CreateExtrudedProfile(width, height);
            DisplayPipeline.CalculateBoundingBox += AddBBox;
        }

        public Line Centreline
        {
            get => _centreline;
            set
            {
                if (!value.IsValid) throw new ArgumentException("Line is not valid.", nameof(Centreline));

                _centreline = value;
                _extrudedProfile = CreateExtrudedProfile(Width, Height);
            }
        }

        private double Width => double.Parse(Attributes.GetUserString("Width"));

        private double Height => double.Parse(Attributes.GetUserString("Height"));

        private static DisplayMaterial Material
        {
            get
            {
                var material = new DisplayMaterial(Color.FromArgb(230, 180, 60), 0.2);
                material.BackDiffuse = Color.FromArgb(230, 180, 60);
                material.IsTwoSided = true;
                return material;
            }
        }

        protected override void OnDraw(Rhino.Display.DrawEventArgs e)
        {
            base.OnDraw(e);
            if (this.IsSelected(false) < 0) return;

            e.Display.DrawBrepWires(_extrudedProfile, Attributes.DrawColor(e.RhinoDoc), Attributes.WireDensity);
            e.Display.DrawBrepShaded(_extrudedProfile, Material);
        }

        protected override void OnDuplicate(RhinoObject source)
        {
            RhinoApp.WriteLine("OnDuplicate");
            base.OnDuplicate(source);
            if (source is CustomGeo src)
            {
                _centreline = src._centreline;
                _extrudedProfile = src._extrudedProfile;
                Attributes.SetUserString("Width", src.Width.ToString());
                Attributes.SetUserString("Height", src.Height.ToString());
                SetCurve(src._centreline.ToNurbsCurve());
            }
        }
        protected override void OnTransform(Transform transform)
        {
            RhinoApp.WriteLine("OnTransform");
            base.OnTransform(transform);
            Centreline.Transform(transform);
            CurveGeometry.Transform(transform);
        }

        public void AddBBox(object sender, CalculateBoundingBoxEventArgs e)
        {
            e.IncludeBoundingBox(_extrudedProfile.GetBoundingBox(false));
        }

        private Brep CreateExtrudedProfile(double width, double height)
        {
            Plane pl = new Plane(_centreline.From, _centreline.Direction);
            Rectangle3d rec = new Rectangle3d(pl, width, height);
            Brep[] planarBrep = Brep.CreatePlanarBreps(rec.ToNurbsCurve(), Rhino.RhinoMath.ZeroTolerance);
            return planarBrep.First().Faces.First().CreateExtrusion(_centreline.ToNurbsCurve(), true);
        }
    }
}
