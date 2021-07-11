using Rhino;
using Rhino.Commands;
using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.InteropServices;
using Rhino.Collections;
using Rhino.DocObjects;
using Rhino.DocObjects.Custom;
using Rhino.FileIO;
using Rhino.Geometry;

namespace SheetPanelAutomation.Rhino
{
    // You must define a Guid attribute for your user data derived class
    // in order to support serialization. Every custom user data class
    // needs a custom Guid
    [Guid("6AF74062-5696-4812-B7EA-618C3BB5D8B2")]
    public class PanelData : UserData
    {
        // Your UserData class must have a public parameterless constructor
        public PanelData() { }

        public override string Description => throw new NotImplementedException();

        public override string ToString() => "Some custom panel data.";

        protected override void OnDuplicate(UserData source)
        {
            if (source is PanelData src)
            {
                FacePlane = src.FacePlane;
                DatumPoint = src.DatumPoint;
            }
        }

        // return true if you have information to save
        public override bool ShouldWrite
        {
            get
            {
                // make up some rule as to if this should be saved in the 3dm file
                if (FacePlane != null && FacePlane.IsValid && DatumPoint != null && DatumPoint.IsValid)
                    return true;
                return false;
            }
        }

        protected override bool Read(BinaryArchiveReader archive)
        {
            ArchivableDictionary dict = archive.ReadDictionary();
            if (dict.ContainsKey("IntegerData") && dict.ContainsKey("StringData"))
            {
                FacePlane = (Plane) dict["FacePlane"];
                DatumPoint = (Point3d) dict["DatumPoint"];
            }
            return true;
        }
        protected override bool Write(BinaryArchiveWriter archive)
        {
            // you can implement File IO however you want... but the dictionary class makes
            // issues like versioning in the 3dm file a bit easier.  If you didn't want to use
            // the dictionary for writing, your code would look something like.
            //
            //  archive.Write3dmChunkVersion(1, 0);
            //  archive.WriteInt(IntegerData);
            //  archive.WriteString(StringData);
            var dict = new ArchivableDictionary(1, "PanelData");
            dict.Set("FacePlane", FacePlane);
            dict.Set("DatumPoint", DatumPoint);
            archive.WriteDictionary(dict);
            return true;
        }
        
        public Plane FacePlane { get; set; }
        public Point3d DatumPoint { get; set; }
        public Polyline ProfileBoundary { get; set; }
        public List<HoleSpec> HoleSpecifications { get; set; }
        public List<Curve> Cutouts { get; set; }
        public TextEntity Label { get; set; }
        public Point3d Centroid { get; set; }
        public double TotalToolPathLength { get; set; }
    }
}