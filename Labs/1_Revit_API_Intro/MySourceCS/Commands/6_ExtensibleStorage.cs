#region Namespaces
using System;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB.Structure;
using System.Collections.Generic;
using Autodesk.Revit.DB.ExtensibleStorage;
using Autodesk.Revit.UI.Selection;
#endregion

namespace MyIntroCs
{
    [Transaction(TransactionMode.Manual)]
    internal class ExtensibleStorage : IExternalCommand
    {
        //Member variables.
        Application _app;
        Document _doc;
        UIDocument _uidoc;

        //Create a new guid.
        Guid _guid = new Guid(Guid.NewGuid().ToString());

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            _uidoc = uiapp.ActiveUIDocument;
            _app = uiapp.Application;
            _doc = _uidoc.Document;

            Transaction tx = new Transaction(_doc, "Extensible Storage");
            tx.Start();

            //Pick a wall.
            Reference r = _uidoc.Selection.PickObject(ObjectType.Element, new WallSelectionFilter());
            Wall w = _doc.GetElement(r) as Wall;

            //Create a schema builder.
            SchemaBuilder builder = new SchemaBuilder(_guid);

            //Set read and write access levels.
            builder.SetReadAccessLevel(AccessLevel.Public);
            builder.SetWriteAccessLevel(AccessLevel.Public);

            //Set name to this schema builder.
            builder.SetSchemaName("WallSocketLocation");
            builder.SetDocumentation("Data store for socket info in a wall");

            //Create field1.
            FieldBuilder fieldBuilder1 = builder.AddSimpleField("SocketLocation", typeof(XYZ));

            //Set unit type.
            fieldBuilder1.SetSpec(SpecTypeId.Length);

            //Create field2.
            FieldBuilder fieldBuilder2 = builder.AddSimpleField("SocketNumber", typeof(string));

            //Register the schema object.
            Schema schema = builder.Finish();

            //Create an entity (object) for this schema (class).
            Entity ent = new Entity(schema);

            Field socketLocation = schema.GetField("SocketLocation");
            ent.Set<XYZ>(socketLocation, new XYZ(2, 0, 0), UnitTypeId.Meters);

            Field sockeNumber = schema.GetField("SocketNumber");
            ent.Set<string>(sockeNumber, "200");

            w.SetEntity(ent);

            Entity ent2 = new Entity(schema);
            Field socketNumber1 = schema.GetField("SocketNumber");
            ent2.Set<String>(socketNumber1, "400");
            w.SetEntity(ent2);

            // List all schemas in the document

            string s = string.Empty;
            IList<Schema> schemas = Schema.ListSchemas();
            foreach (Schema sch in schemas)
            {
                s += "\r\nSchema Name: " + sch.SchemaName;
            }
            TaskDialog.Show("Schema details", s);

            // List all Fields for our schema

            s = string.Empty;
            Schema ourSchema = Schema.Lookup(_guid);
            IList<Field> fields = ourSchema.ListFields();
            foreach (Field fld in fields)
            {
                s += "\r\nField Name: " + fld.FieldName;
            }
            TaskDialog.Show("Field details", s);

            // Extract the value for the field we created

            Entity wallSchemaEnt = w.GetEntity(Schema.Lookup(_guid));

            XYZ wallSocketPos = wallSchemaEnt.Get<XYZ>(
              Schema.Lookup(_guid).GetField("SocketLocation"),
              //DisplayUnitType.DUT_METERS ); // 2020
              UnitTypeId.Meters); // 2021

            s = "SocketLocation: " + PointToString(wallSocketPos);

            string wallSocketNumber = wallSchemaEnt.Get<String>(
              Schema.Lookup(_guid).GetField("SocketNumber"));

            s += "\r\nSocketNumber: " + wallSocketNumber;

            TaskDialog.Show("Field values", s);

            tx.Commit();
            return Result.Succeeded;
        }

        #region WallSelectionFilter()
        /// <summary>
        /// Restrict selection to just select walls.
        /// </summary>
        class WallSelectionFilter : ISelectionFilter
        {
            public bool AllowElement(Element e)
            {
                return e is Wall;
            }

            public bool AllowReference(Reference r, XYZ p)
            {
                return true;
            }
        }
        #endregion

        #region PointToString()
        // Helper Function: returns XYZ in a string form. 
        // 
        public static string PointToString(XYZ pt)
        {
            if (pt == null)
            {
                return "";
            }
            return string.Format("({0},{1},{2})",
            pt.X.ToString("F2"), pt.Y.ToString("F2"), pt.Z.ToString("F2"));
        }
        #endregion

    }
}
