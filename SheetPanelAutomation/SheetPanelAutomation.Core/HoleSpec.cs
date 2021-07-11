using Rhino.Geometry;

namespace SheetPanelAutomation.Core
{
    public class HoleSpec
    {
        public Point3d Location { get; set; }
        public string HoleType { get; set; }
        public double Diameter { get; set; }
    }
}