using Rhino.Geometry;

namespace SheetPanelAutomation.Rhino
{
    public class SlotSpec : HoleSpec
    {
        public Point3d EndLocation { get; set; }
        public double Length { get; set; }
    }
}