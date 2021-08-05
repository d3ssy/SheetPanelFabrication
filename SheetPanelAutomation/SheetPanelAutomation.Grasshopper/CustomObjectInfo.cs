using Grasshopper;
using Grasshopper.Kernel;
using System;
using System.Drawing;

namespace CustomObject
{
    public class CustomObjectInfo : GH_AssemblyInfo
    {
        public override string Name => "CustomObject";

        //Return a 24x24 pixel bitmap to represent this GHA library.
        public override Bitmap Icon => null;

        //Return a short string describing the purpose of this GHA library.
        public override string Description => "";

        public override Guid Id => new Guid("28982E8C-65CF-4813-BF6E-3F4BA9B257F1");

        //Return a string identifying you or your company.
        public override string AuthorName => "";

        //Return a string representing your preferred contact details.
        public override string AuthorContact => "";
    }
}