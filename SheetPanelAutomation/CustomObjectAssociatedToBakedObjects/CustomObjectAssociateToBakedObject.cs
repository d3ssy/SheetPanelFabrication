using Rhino.DocObjects.Custom;
using System;
using System.Collections.Generic;
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

        public Guid ObjId;

        private List<Curve> _dashedCurves;

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
            ApplyDashPattern(centreLine.ToNurbsCurve(), new[] {1.0});
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
                ApplyDashPattern(_centreLine.ToNurbsCurve(), new[] { 1.0 });
            }
        }

        private double Width => double.Parse(Attributes.GetUserString("Width"));

        private double Height => double.Parse(Attributes.GetUserString("Height"));

        protected override void OnDraw(DrawEventArgs e)
        {
            base.OnDraw(e);
            if (this.IsSelected(false) < 0) return;

            e.RhinoDoc.Views.Redraw();
            _dashedCurves.ForEach(c => e.Display.DrawCurve(c, Color.FromArgb(230, 180, 60), 5));
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
            UpdateBox(ExtrudedProfile);
        }

        public void AddBBox(object sender, CalculateBoundingBoxEventArgs e)
        {
            e.IncludeBoundingBox(ExtrudedProfile.GetBoundingBox(false));
        }

        private void UpdateBox(Brep box)
        {
            RhinoDoc doc = RhinoDoc.ActiveDoc;
            if (doc == null) return;
            doc.Objects.Replace(ObjId, box);
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
            UpdateBox(ExtrudedProfile);
        }

        private void ApplyDashPattern(Curve curve, double[] pattern)
        {
            if (pattern == null || pattern.Length == 0)
                _dashedCurves = new List<Curve>{curve};

            double curveLength = curve.GetLength();
            List<Curve> dashes = new List<Curve>();

            double offset0 = 0.0;
            int index = 0;
            while (true)
            {
                // Get the current dash length.
                double dashLength = pattern[index++];
                if (index >= pattern.Length)
                    index = 0;

                // Compute the offset of the current dash from the curve start.
                double offset1 = offset0 + dashLength;
                if (offset1 > curveLength)
                    offset1 = curveLength;

                // Solve the curve parameters at the current dash start and end.
                double t0, t1;
                curve.LengthParameter(offset0, out t0);
                curve.LengthParameter(offset1, out t1);

                Curve dash = curve.Trim(t0, t1);
                if (dash != null)
                {
                    dashes.Add(dash);
                }

                // Get the current gap length.
                double gapLength = pattern[index++];
                if (index >= pattern.Length)
                    index = 0;

                // Set the start of the next dash to be the end of the current
                // dash + the length of the adjacent gap.
                offset0 = offset1 + gapLength;

                // Abort when we've reached the end of the curve.
                if (offset0 >= curveLength)
                    break;
            }

            _dashedCurves = dashes;
        }
    }
}
