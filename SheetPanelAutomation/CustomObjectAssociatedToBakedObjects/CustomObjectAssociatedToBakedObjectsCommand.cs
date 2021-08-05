using Rhino;
using Rhino.Commands;
using Rhino.Geometry;
using Rhino.Input;

namespace CustomObjectAssociatedToBakedObjects
{
    public class CustomObjectAssociatedToBakedObjectsCommand : Command
    {
        public CustomObjectAssociatedToBakedObjectsCommand()
        {
            // Rhino only creates one instance of each command class defined in a
            // plug-in, so it is safe to store a refence in a static property.
            Instance = this;
        }

        ///<summary>The only instance of this command.</summary>
        public static CustomObjectAssociatedToBakedObjectsCommand Instance
        {
            get; private set;
        }

        ///<returns>The command name as it appears on the Rhino command line.</returns>
        public override string EnglishName
        {
            get { return "CustomObjectAssociatedToBakedObjectsCommand"; }
        }

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            RhinoGet.GetLine(out Line line);
            var geo = new CustomObjectAssociateToBakedObject(line, 10, 10);
            doc.Objects.AddRhinoObject(geo, null);

            return Result.Success;
        }
    }
}
