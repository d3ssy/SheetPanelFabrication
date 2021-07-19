using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.DocObjects.Custom;
using Rhino.Geometry;

namespace CustomObject
{
    public class RectangleObject : CustomCurveObject, IGH_PreviewData
    {
        public RectangleObject()
        {
        }
        public RectangleObject(UserDictionary data, Curve curve) : base(curve)
        {
            Attributes.UserData.Add(data);
        }

        public void DrawViewportWires(GH_PreviewWireArgs args)
        {
            args.Pipeline.DrawCurve(CurveGeometry, args.Color, args.Thickness);
        }

        public void DrawViewportMeshes(GH_PreviewMeshArgs args)
        {
            args.Pipeline.DrawMeshShaded(GetMesh(), args.Material);
        }

        private Mesh GetMesh()
        {
            return Mesh.CreateFromCurveExtrusion(CurveGeometry, Vector3d.ZAxis * 10,
                MeshingParameters.Default, GetBoundingBox());
        }

        private BoundingBox GetBoundingBox()
        {
            return CurveGeometry.GetBoundingBox(Plane.WorldXY);
        }

        public BoundingBox ClippingBox => GetBoundingBox();
    }
}
