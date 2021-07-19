﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Rhino;
using Rhino.Commands;
using Rhino.Geometry;
using Rhino.Input;

namespace CustomObject.PlugIn
{
    [Guid("DC72A496-E674-4547-A5CE-9D1FE75B414E")]
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
