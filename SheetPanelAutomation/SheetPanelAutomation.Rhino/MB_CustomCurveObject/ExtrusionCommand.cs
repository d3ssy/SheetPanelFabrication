using Rhino;
using Rhino.Commands;
using Rhino.Geometry;
using Rhino.Input;

namespace CustomObject.PlugIn
{
    [System.Runtime.InteropServices.Guid("61055a13-cd6d-4c5d-8978-0ee9a0560837")]
    public class ExtrusionCommand : Command
    {
        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            RhinoGet.GetLine(out Line line);
            var geo = new CustomGeo(line, 10, 10);
            doc.Objects.AddRhinoObject(geo, null);
            return Result.Success;
        }

        public override string EnglishName => "CreatesExtrusion";
    }
}
