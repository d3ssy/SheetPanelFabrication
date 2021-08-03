using Rhino.DocObjects.Custom;
using System;
using System.Drawing;
using System.Linq;
using Rhino;
using Rhino.Display;
using Rhino.DocObjects;
using Rhino.Geometry;

namespace CustomObjectAssociatedToBakedObjects
{
    public class CustomObjectAssociateToBakedObject : CustomCurveObject
    {
        private Line _centreLine;

        public Brep ExtrudedProfile;

        public CustomObjectAssociateToBakedObject()
        {
            DisplayPipeline.CalculateBoundingBox += AddBBox;
        }

        public CustomObjectAssociateToBakedObject(Line centreLine, double width, double height) : base(centreLine.ToNurbsCurve())
        {
            _centreLine = centreLine;
            Attributes.SetUserString("Width", width.ToString());
            Attributes.SetUserString("Height", height.ToString());
            ExtrudedProfile = CreateExtrudedProfile(width, height);
            DisplayPipeline.CalculateBoundingBox += AddBBox;
        }

        public Line CentreLine
        {
            get => _centreLine;
            set
            {
                if (!value.IsValid) throw new ArgumentException("Line is not valid.", nameof(CentreLine));

                _centreLine = value;
                ExtrudedProfile = CreateExtrudedProfile(Width, Height);
            }
        }

        private double Width => double.Parse(Attributes.GetUserString("Width"));

        private double Height => double.Parse(Attributes.GetUserString("Height"));

        protected override void OnDraw(Rhino.Display.DrawEventArgs e)
        {
            base.OnDraw(e);
            if (this.IsSelected(false) < 0) return;

            e.Display.DrawDottedLine(_centreLine, Color.FromArgb(230, 180, 60));
        }

        protected override void OnDuplicate(RhinoObject source)
        {
            RhinoApp.WriteLine("OnDuplicate");
            base.OnDuplicate(source);
            if (source is CustomObjectAssociateToBakedObject src)
            {
                _centreLine = src._centreLine;
                ExtrudedProfile = src.ExtrudedProfile.DuplicateBrep();
                Attributes.SetUserString("Width", src.Width.ToString());
                Attributes.SetUserString("Height", src.Height.ToString());
                SetCurve(src._centreLine.ToNurbsCurve());
            }
        }
        protected override void OnTransform(Transform transform)
        {
            RhinoApp.WriteLine("OnTransform");
            base.OnTransform(transform);
            _centreLine.Transform(transform);
            CurveGeometry.Transform(transform);
            ExtrudedProfile.Transform(transform);
        }

        public void AddBBox(object sender, CalculateBoundingBoxEventArgs e)
        {
            e.IncludeBoundingBox(ExtrudedProfile.GetBoundingBox(false));
        }

        private Brep CreateExtrudedProfile(double width, double height)
        {
            Plane pl = new Plane(_centreLine.From, _centreLine.Direction);
            Rectangle3d rec = new Rectangle3d(pl, width, height);
            Brep[] planarBrep = Brep.CreatePlanarBreps(rec.ToNurbsCurve(), Rhino.RhinoMath.ZeroTolerance);
            return planarBrep.First().Faces.First().CreateExtrusion(_centreLine.ToNurbsCurve(), true);
        }

        public void UpdateExtrudedProfile()
        {
            ExtrudedProfile = CreateExtrudedProfile(Width, Height);
        }
    }
}
