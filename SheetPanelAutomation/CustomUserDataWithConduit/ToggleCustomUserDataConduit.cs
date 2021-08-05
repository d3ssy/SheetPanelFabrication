using Rhino;
using Rhino.Commands;
using System;
using System.CodeDom;
using System.Runtime.InteropServices;
using Rhino.Input;
using Rhino.Input.Custom;

namespace RhinoPlugin_00
{
    [Guid("4C3E6976-AC72-42E4-9ADC-A5D584D79177")]
    public class ToggleCustomUserDataConduit : Command
    {
        static ToggleCustomUserDataConduit _instance;
        public ToggleCustomUserDataConduit()
        {
            _instance = this;
        }

        ///<summary>The only instance of the ToggleExtrudedProfileDisplay command.</summary>
        public static ToggleCustomUserDataConduit Instance => _instance;

        public override string EnglishName => "ToggleExtrudedProfileDisplay";

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            doc.Objects.UnselectAll();
            var go = new GetObject();
            var getResult = go.GetMultiple(1, 0);
            if (getResult != GetResult.Object) return Result.Failure;

            foreach (var objRef in go.Objects())
            {
                var obj = objRef.Object();
                if (CustomUserData.IsAttached(obj))
                {
                    var ud = CustomUserData.DataFromObject(obj);
                    if (ud != null)
                    {
                        ud.ToggleDisplay();
                    }
                }
            }

            return Result.Success;
        }
    }
}