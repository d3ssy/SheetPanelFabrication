using Rhino;
using Rhino.Commands;
using Rhino.Geometry;
using Rhino.Input;
using Rhino.Input.Custom;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace CustomExtrudedProfile
{
    [Guid("96B580F4-6A3C-4F02-AED5-8D9911297AD2")]
    public class CustomExtrudedProfileCommand : Command
    {
        public CustomExtrudedProfileCommand()
        {
            // Rhino only creates one instance of each command class defined in a
            // plug-in, so it is safe to store a refence in a static property.
            Instance = this;
        }

        ///<summary>The only instance of this command.</summary>
        public static CustomExtrudedProfileCommand Instance
        {
            get; private set;
        }

        ///<returns>The command name as it appears on the Rhino command line.</returns>
        public override string EnglishName
        {
            get { return "CreateCustomExtrudedProfile"; }
        }

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            RhinoGet.GetLine(out Line line);
            var geo = new CustomGeo(line, 10, 10);
            doc.Objects.AddRhinoObject(geo, null);

            return Result.Success;
        }
    }
}
