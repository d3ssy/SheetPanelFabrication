using Rhino.Geometry;

namespace SheetPanelAutomation.Rhino
{
    public class HoleSpec
    {
        public Point3d Location { get; set; }
        public string HoleType { get; set; }
        public double Diameter { get; set; }
    }
}