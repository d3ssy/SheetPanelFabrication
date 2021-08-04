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

        private Brep _extrudedProfile;

        public Guid _objId;

        public CustomObjectAssociateToBakedObject()
        {
            DisplayPipeline.CalculateBoundingBox += AddBBox;
        }

        public CustomObjectAssociateToBakedObject(Line centreLine, double width, double height) : base(centreLine.ToNurbsCurve())
        {
            _centreLine = centreLine;
            Attributes.SetUserString("Width", width.ToString());
            Attributes.SetUserString("Height", height.ToString());
            _extrudedProfile = CreateExtrudedProfile(width, height);
            AddBox(_extrudedProfile);
            DisplayPipeline.CalculateBoundingBox += AddBBox;
        }

        public Line CentreLine
        {
            get => _centreLine;
            set
            {
                if (!value.IsValid) throw new ArgumentException("Line is not valid.", nameof(CentreLine));

                _centreLine = value;
                _extrudedProfile = CreateExtrudedProfile(Width, Height);
            }
        }

        private double Width => double.Parse(Attributes.GetUserString("Width"));

        private double Height => double.Parse(Attributes.GetUserString("Height"));

        protected override void OnDraw(Rhino.Display.DrawEventArgs e)
        {
            base.OnDraw(e);
            if (this.IsSelected(false) < 0) return;

            e.RhinoDoc.Views.Redraw();
            e.Display.DrawDottedLine(_centreLine, Color.FromArgb(230, 180, 60));
        }

        protected override void OnDuplicate(RhinoObject source)
        {
            RhinoApp.WriteLine("OnDuplicate");
            base.OnDuplicate(source);
            if (source is CustomObjectAssociateToBakedObject src)
            {
                _centreLine = src._centreLine;
                _extrudedProfile = src._extrudedProfile.DuplicateBrep();
                Attributes.SetUserString("Width", src.Width.ToString());
                Attributes.SetUserString("Height", src.Height.ToString());
                SetCurve(src._centreLine.ToNurbsCurve());
                AddBox(_extrudedProfile);
            }
        }
        protected override void OnTransform(Transform transform)
        {
            RhinoApp.WriteLine("OnTransform");
            base.OnTransform(transform);
            _centreLine.Transform(transform);
            CurveGeometry.Transform(transform);
            _extrudedProfile.Transform(transform);
            UpdateBox(_extrudedProfile);
        }

        public void AddBBox(object sender, CalculateBoundingBoxEventArgs e)
        {
            e.IncludeBoundingBox(_extrudedProfile.GetBoundingBox(false));
        }

        private void AddBox(Brep box)
        {
            RhinoDoc doc = RhinoDoc.ActiveDoc;

            if (doc == null) return;
            _objId = doc.Objects.AddBrep(box);
        }

        private void UpdateBox(Brep box)
        {
            RhinoDoc doc = RhinoDoc.ActiveDoc;
            if (doc == null) return;
            doc.Objects.Replace(_objId, box);
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
            _extrudedProfile = CreateExtrudedProfile(Width, Height);
            UpdateBox(_extrudedProfile);
        }
    }
}
