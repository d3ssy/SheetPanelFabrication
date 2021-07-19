using System;
using System.Runtime.CompilerServices;
using Rhino.Collections;
using Rhino.DocObjects.Custom;
using Rhino.FileIO;
using Rhino.Geometry;

namespace CustomObject
{
    [System.Runtime.InteropServices.Guid("45E43414-A630-44A8-B98A-ACE549D1FC0C")]
    public class UserDataOperation : UserDictionary
    {
        public UserDataOperation(){}
        public UserDataOperation(Line inputDatum)
        {
            InputDatum = inputDatum;
            UpdateGeometry();
            CreateHoles();
        }
        public Point3d InsertionPt { get; set; }
        public Line InputDatum { get; set; }
        public Rectangle3d Boundary { get; set; }

        public Circle[] Holes { get; set; }

        #region Userdata overrides
        /// <summary>
        /// Called when the object is being duplicated.
        /// </summary>
        protected override void OnDuplicate(UserData source)
        {
            if (source is UserDataOperation src)
            {
                InputDatum = src.InputDatum;
                InsertionPt = src.InsertionPt;
                Boundary = src.Boundary;
                Holes = src.Holes;
            }
        }

        /// <summary>
        /// Descriptive name of the user data.
        /// </summary>
        public override string Description => "UserDataOperation";

        /// <summary>
        /// Return true to serialize to 3dm files.
        /// </summary>
        public override bool ShouldWrite => true;

        /// <summary>
        /// Reads the content of this data from a stream archive.
        /// </summary>
        protected override bool Read(BinaryArchiveReader archive)
        {
            //archive.Read3dmChunkVersion(out var major, out var minor);
            //if (1 == major && 0 == minor)
            //{
            //    var point = archive.ReadPoint3d();
            //    if (point.IsValid)
            //        InsertionPt = point;
            //    var line = archive.ReadLine();
            //    if (line.IsValid)
            //        InputDatum = line;
            //}
            //return !archive.ReadErrorOccured;

            ArchivableDictionary dict = archive.ReadDictionary();
            if (dict.ContainsKey("IntegerData") && dict.ContainsKey("StringData"))
            {
                InputDatum = (Line)dict["InputDatum"];
                InsertionPt = (Point3d)dict["DatumPoint"];
            }
            return true;
        }

        /// <summary>
        /// Writes the content of this data to a stream archive.
        /// </summary>
        protected override bool Write(BinaryArchiveWriter archive)
        {
            //archive.Write3dmChunkVersion(1, 0);
            //archive.WriteLine(InputDatum);
            //archive.WritePoint3d(InsertionPt);
            //return !archive.WriteErrorOccured;

            ArchivableDictionary dict = new ArchivableDictionary(1, "PanelData");
            dict.Set("InputDatum", InputDatum);
            dict.Set("DatumPoint", InsertionPt);
            archive.WriteDictionary(dict);
            return true;
        }

        /// <summary>
        /// Called when the object associated with this data is transformed.
        /// </summary>
        protected override void OnTransform(Transform xform)
        {
            base.OnTransform(xform);

            var line = InputDatum;
            line.Transform(xform);
            InputDatum = line;
        }
        #endregion

        #region Static helper methods
        public void UpdateGeometry()
        {
            Point3d middlePt = (InputDatum.From + InputDatum.To) / 2;
            double distance = InputDatum.From.DistanceTo(InputDatum.To) * 2;
            Vector3d perpendicularDir = Vector3d.CrossProduct(InputDatum.Direction, Vector3d.ZAxis);
            perpendicularDir.Unitize();

            Point3d pt3 = InputDatum.From + (perpendicularDir * distance);
            InsertionPt = middlePt + (perpendicularDir * distance);

            Polyline poly = new Polyline(5)
            {
                InputDatum.To,
                InputDatum.From,
                pt3,
                pt3,
                InputDatum.To
            };
            Boundary = Rectangle3d.CreateFromPolyline(poly);
        }

        public void CreateHoles()
        {
            Transform scale = Transform.Scale(Boundary.Center, 0.8);
            var pt0 = Boundary.Corner(0);
            var pt2 = Boundary.Corner(2);
            pt0.Transform(scale);
            pt2.Transform(scale);
            Holes = new Circle[]{new Circle(pt0, 1), new Circle(pt2, 10) };
        }

        //public RectangleObject CreateCustomObject()
        //{
        //    return new RectangleObject(this, this.Boundary.ToPolyline().ToPolylineCurve());
        //}
        public CustomGeometryGoo CreateCustomObject()
        {
            return new CustomGeometryGoo(this, this.Boundary.ToPolyline().ToPolylineCurve());
        }
        #endregion
    }
}