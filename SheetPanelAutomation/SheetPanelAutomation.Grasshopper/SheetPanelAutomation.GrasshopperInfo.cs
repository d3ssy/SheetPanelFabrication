using Grasshopper.Kernel;
using System;
using System.Drawing;

namespace SheetPanelAutomation.Grasshopper
{
    public class SheetPanelAutomation : GH_AssemblyInfo
  {
    public override string Name
    {
        get
        {
            return "SheetPanelAutomationGrasshopper";
        }
    }
    public override Bitmap Icon
    {
        get
        {
            //Return a 24x24 pixel bitmap to represent this GHA library.
            return null;
        }
    }
    public override string Description
    {
        get
        {
            //Return a short string describing the purpose of this GHA library.
            return "";
        }
    }
    public override Guid Id
    {
        get
        {
            return new Guid("544fcda1-bdc8-4698-88f3-9adf9ce9ad87");
        }
    }

    public override string AuthorName
    {
        get
        {
            //Return a string identifying you or your company.
            return "";
        }
    }
    public override string AuthorContact
    {
        get
        {
            //Return a string representing your preferred contact details.
            return "";
        }
    }
}
}
