﻿#region Namespace
using System;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI.Selection;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
#endregion

namespace MyIntroCs
{
    [Transaction(TransactionMode.Manual)]
    internal class ElementFiltering : IExternalCommand
    {
        //Member variables.
        Application _app;
        Document _doc;
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //Get the access to the top most objects.
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            _app = uiapp.Application;
            _doc = uidoc.Document;

            ListFamilyTypes();

            FindFamilyType_Wall_v2("Basic Wall: ", "Test");

            return Result.Succeeded;
        }

        #region ListFamilyType()
        /// <summary>
        /// Filter and list elements (walls and doors).
        /// </summary>
        public void ListFamilyTypes()
        {

            //Filter the document looking for element of type: WallType.
            FilteredElementCollector wCollector = new FilteredElementCollector(_doc)
                .WherePasses(new ElementClassFilter(typeof(WallType)));

            //Create a list of elements with filtered elements, in this casse walls.
            IList<Element> wallTypes = wCollector.ToElements();

            //Filter the document looking for elements of type: FamilySymbol and category: OST_Doors.
            FilteredElementCollector dCollector = new FilteredElementCollector(_doc)
                .OfClass(typeof(FamilySymbol))
                .OfCategory(BuiltInCategory.OST_Doors);

            //Create a list of elements with filtered elements, in this case doors.
            IList<Element> doorTypes = dCollector.ToElements();

            //Filter the document looking for elements of type: OST_Window.
            FilteredElementCollector wiCollector = new FilteredElementCollector(_doc)
                .OfClass(typeof(FamilySymbol))
                .OfCategory(BuiltInCategory.OST_Windows);

            //Create a list of elements with filtered elements, in this case windows.
            IList<Element> windowTypes = wiCollector.ToElements();

            //Filter the document looking for elements that are places in the model of type: OST_Doors.
            FilteredElementCollector doorCollector = new FilteredElementCollector(_doc)
                .OfClass(typeof(FamilyInstance))
                .OfCategory(BuiltInCategory.OST_Doors);

            //Create a list of elements with filtered elements, in this case doors.
            IList<Element> doorInstances = doorCollector.ToElements();

            ShowElementList(wallTypes, "Walls types (by filter): ");
            ShowElementList(doorTypes, "Doors types (by filter): ");
            ShowElementList(windowTypes, "Windows types (by filter): ");
            ShowElementList(doorInstances, "Door Instances (by filter): ");            
        }

        /// <summary>
        /// Helper function 1: Show a list of elements (walls and doors).
        /// </summary>
        /// <param name="e">elements</param>
        /// <param name="h">header</param>
        public void ShowElementList(IList<Element> e, string h)
        {
            string s = " - Class - Category - Name (or Family: Type Name) - Id - \r\n";
            int num = 1;
            foreach (Element el in e)
            {
                s += $"{num} - {ElementToString(el)}";
                num++;
            }

            TaskDialog.Show($"{h} ({e.Count.ToString()}):", s);
        }

        /// <summary>
        /// Helper function 2: Summarize elements informatin as a line of text.
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public string ElementToString(Element e)
        {
            if (e == null)
            {
                return "none";
            }

            string name = "";

            if (e is ElementType)
            {
                Parameter p = e.get_Parameter(BuiltInParameter.SYMBOL_FAMILY_AND_TYPE_NAMES_PARAM);
                if (p != null)
                {
                    name = p.AsString();
                }
            }
            else
            {
                name = e.Name;
            }

            return $"{e.GetType().Name}; {e.Category.Name}; {name}; {e.Id.IntegerValue.ToString()}\r\n";
        }
        #endregion

        #region FindFamilyType_Wall_v1()
        /// <summary>
        /// Find a specific family type for a wall with a given family and type name. Use LINQ.
        /// </summary>
        /// <param name="wFamilyName">Family name of the desired wall</param>
        /// <param name="wTypeName">Type name of the desired wall</param>
        /// <returns></returns>
        public Element FindFamilyType_Wall_v1(string wFamilyName, string wTypeName)
        {
            //Narrow down a collector by class.
            FilteredElementCollector wCollector = new FilteredElementCollector(_doc)
                .OfClass(typeof(WallType));

            //Use LINQ query.
            var wTypeElems =
                from element in wCollector
                where element.Name.Equals(wTypeName)
                select element;

            //Get the result.
            Element wType = null;

            //Create a list of elements to get the result of the query.
            IList<Element> wTypeList = wTypeElems.ToList();
            if (wTypeList.Count > 0)
            {
                wType = wTypeList[0];
            }

            return wType;
        }
        #endregion

        #region FindFamilyType_Wall_v2()
        /// <summary>
        /// Find a specific family type for a wall with a given family and type name. Use iteration.
        /// </summary>
        /// <param name="wFamilyName"></param>
        /// <param name="wTypeName"></param>
        /// <returns></returns>
        public Element FindFamilyType_Wall_v2(string wFamilyName, string wTypeName)
        {
            //Narrow down the collector by class.
            FilteredElementCollector wCollector = new FilteredElementCollector(_doc)
                .OfClass(typeof(WallType));

            //Use iterator.
            FilteredElementIterator wTypeItr = wCollector.GetElementIterator();
            wTypeItr.Reset();

            Element wType = null;

            while (wTypeItr.MoveNext())
            {
                WallType wallType = (WallType)wTypeItr.Current;
                //Check two names: type name and family name.
                if ((wallType.Name == wTypeName) & (wallType.get_Parameter(BuiltInParameter.SYMBOL_FAMILY_NAME_PARAM).AsString().Equals(wFamilyName)))
                { 
                    wType = wallType;
                    TaskDialog.Show("Title", "I found your wall!");
                    break;
                }
                else
                {
                    TaskDialog.Show("Title", "I don't found your wall!");
                    break;
                }
            }

            return wType;
        }
        #endregion
    }
}
