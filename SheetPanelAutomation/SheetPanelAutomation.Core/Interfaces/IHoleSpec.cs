using Rhino.Geometry;
using SheetPanelAutomation.Core.Enums;

namespace SheetPanelAutomation.Core.Interfaces
{
    public interface IHoleSpec
    {
        HoleType HoleType { get; set; }
        HoleProfile HoleProfile { get; set; }
        Plane Location { get; set; }
        double Diameter { get; set; }
    }
}