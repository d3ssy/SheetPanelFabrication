using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino;
using Rhino.DocObjects;
using Rhino.Geometry;
using System;
using System.Drawing;

namespace CustomObject
{
    public class CustomGeometryGoo : GH_GeometricGoo<Curve>, IGH_PreviewData, IGH_BakeAwareData
    {
        private readonly UserDataOperation _data;

        public CustomGeometryGoo()
        {

        }
        public CustomGeometryGoo(UserDataOperation data, Curve curve)
        {
            m_value = curve;
            _data = data;
            m_value.UserData.Add(data);
        }

        public override IGH_Goo Duplicate()
        {
            return _data.CreateCustomObject();
        }

        public override IGH_GeometricGoo DuplicateGeometry()
        {
            return _data.CreateCustomObject();
        }

        public override BoundingBox GetBoundingBox(Transform xform)
        {
            var Bbox = m_value.GetBoundingBox(Plane.WorldXY);
            Bbox.Transform(xform);
            return Bbox;
        }

        public override IGH_GeometricGoo Transform(Transform xform)
        {
            CustomGeometryGoo deepcopy = (CustomGeometryGoo)DuplicateGeometry();
            deepcopy.m_value.Transform(xform);
            return deepcopy;
        }

        public override IGH_GeometricGoo Morph(SpaceMorph xmorph)
        {
            return DuplicateGeometry();
        }

        public override string ToString()
        {
            return "CustomGeoGoo";
        }

        public bool BakeGeometry(RhinoDoc doc, ObjectAttributes att, out Guid obj_guid)
        {
            if (Value == null)
            {
                obj_guid = Guid.Empty;
                return false;
            }

            var geo = GetBrep();
            geo.UserData.Add(_data);
            obj_guid = doc.Objects.AddBrep(geo, att);
            return true;
        }

        public override bool IsValid => true;
        public override BoundingBox Boundingbox => m_value.GetBoundingBox(Plane.WorldXY);
        public override string TypeName => "CustomGeoGoo";
        public override string TypeDescription => "CustomGeoGoo";
        public void DrawViewportWires(GH_PreviewWireArgs args)
        {
            if (Value == null) return;
            args.Pipeline.DrawCurve(Value, args.Color, args.Thickness);
            args.Pipeline.DrawCircle(_data.Holes[0], args.Color, args.Thickness);
            args.Pipeline.DrawCircle(_data.Holes[1], args.Color, args.Thickness);
        }

        public void DrawViewportMeshes(GH_PreviewMeshArgs args)
        {
            if (Value == null) return;

            Mesh[] meshes = Mesh.CreateFromBrep(GetBrep(), new MeshingParameters(0.0));

            if (meshes == null || meshes.Length == 0) return;

            Mesh displayMesh = new Mesh();
            foreach (Mesh mesh in meshes)
            {
                displayMesh.Append(mesh);
            }

            var mat = new Rhino.Display.DisplayMaterial();
            mat.Diffuse = Color.FromArgb(237, 211, 64);
            args.Pipeline.DrawMeshShaded(displayMesh, mat);
        }

        private Mesh GetMesh()
        {
            return Mesh.CreateFromCurveExtrusion(m_value, Vector3d.ZAxis * 10,
                MeshingParameters.Default, Boundingbox);
        }

        private Brep GetBrep()
        {
            return Extrusion.Create(Value, 1, true).ToBrep();
        }

        public BoundingBox ClippingBox => Boundingbox;
    }
}
