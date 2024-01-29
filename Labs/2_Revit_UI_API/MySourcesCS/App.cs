#region Namespace
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using System.Xml;
using System.Diagnostics;
using System.IO;
using System.Windows.Media.Imaging;
using Autodesk.Revit.UI.Events;
using System.Reflection;
#endregion

namespace MyUiCs
{
    public class App : IExternalApplication
    {
        //Assembly name and namespace of external command.
        const string _myUiCsName = "MyUiCs";
        const string _dllExtension = ".dll";

        //Name of subdirectory containing images.
        const string _imageFolderName = "Resources";

        //Location of external command dll.
        string _myUiCsPath;

        //Location of images for icons.
        string _imageFolder;

        public Result OnShutdown(UIControlledApplication app)
        {
            return Result.Succeeded;
        }

        public Result OnStartup(UIControlledApplication app)
        {
            //External application directory.
            string dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            //External command path.
            _myUiCsPath = Path.Combine(dir, $"{_myUiCsName}{_dllExtension}");

            if (!File.Exists(_myUiCsPath))
            {
                TaskDialog.Show("UIRibbon", $"External command assembly not found: {_myUiCsPath}");

                return Result.Failed;
            }

            //Image path.
            _imageFolder = FindFolderInParents(dir, _imageFolderName);

            if ( _imageFolder == null || !Directory.Exists(_imageFolder))
            {
                TaskDialog.Show("UIRibbon", string.Format($"No image folder name {_imageFolderName} found in the parent directories of {dir}"));
            }

            AddRibbonSampler(app);

            return Result.Succeeded;
        }

        #region FindFolderInParents()
        /// <summary>
        /// Starting at the given directory, search upwards for 
        /// a subdirectory with the given target name located
        /// in some parent directory. 
        /// </summary>
        /// <param name="path">Starting directory, e.g. GetDirectoryName(GetExecutingAssembly().Location ).</param>
        /// <param name="target">Target subdirectory name, e.g. "Images".</param>
        /// <returns>The full path of the target directory if found, else null.</returns>
        private string FindFolderInParents(string path, string target)
        {
            Debug.Assert(Directory.Exists(path), "Expected an existing directory to start search in");

            string s;

            do
            {
                s = Path.Combine(path, target);
                if (Directory.Exists(s))
                {
                    return s;
                }
                path = Path.GetDirectoryName(path);
            } while (null != path);

            return null;
        }
        #endregion

        #region newBitmapImage
        /// <summary>
        /// Load a new icon bitmap from image folder.
        /// </summary>
        /// <param name="imageName"></param>
        /// <returns></returns>
        private BitmapImage newBitmapImage(string imageName)
        {
            return new BitmapImage(new Uri(Path.Combine(_imageFolder, imageName)));
        }
        #endregion

        #region AddRibbonSampler()
        /// <summary>
        /// 
        /// </summary>
        /// <param name="app"></param>
        private void AddRibbonSampler(UIControlledApplication app)
        {
            app.CreateRibbonTab("MyUiCs");

            RibbonPanel panel = app.CreateRibbonPanel("MyUiCs", "Ribbon Sampler");

            AddPushButton(panel);

            AddSplitButton(panel);

            AddComboBox(panel);
        }
        #endregion

        #region AddPushButton()
        /// <summary>
        /// Create a button to the panel.
        /// </summary>
        /// <param name="panel">Panel to host button.</param>
        public void AddPushButton(RibbonPanel panel)
        {
            //Set information about the button command.
            PushButtonData pushButtonDataHello = new PushButtonData("PushButtonHello", "Hello World", _myUiCsPath, $"{_myUiCsName}.HelloWorldFull");

            //Add a button to the panel.
            PushButton pushButtonHello = panel.AddItem(pushButtonDataHello) as PushButton;

            //Add an icon.
            pushButtonHello.LargeImage = newBitmapImage("Icon.ico");

            //Add a tooltip.
            pushButtonHello.ToolTip = "Simple push button";
        }
        #endregion

        #region AddSplitButton()
        /// <summary>
        /// Create a split button that group buttons together.
        /// </summary>
        /// <param name="panel">Panel to host split button.</param>
        public void AddSplitButton(RibbonPanel panel)
        {
            PushButtonData pushButtonData1 = new PushButtonData("SplitCommandData", "Command Data", _myUiCsPath, $"{_myUiCsName}.CommandData");
            pushButtonData1.LargeImage = newBitmapImage("ImgHelloWorld.png");

            PushButtonData pushButtonData2 = new PushButtonData("SplitDbElement", "DB Element", _myUiCsPath, $"{_myUiCsName}.DbElement");
            pushButtonData2.LargeImage = newBitmapImage("ImgHelloWorld.png");

            PushButtonData pushButtonData3 = new PushButtonData("SplitElementFiltering", "ElementFiltering", _myUiCsPath, $"{_myUiCsName}.ElementFiltering");
            pushButtonData3.LargeImage = newBitmapImage("ImgHelloWorld.png");

            SplitButtonData splitBtnData = new SplitButtonData("SplitButton", "Split button");

            SplitButton splitBtn = panel.AddItem(splitBtnData) as SplitButton;
            splitBtn.AddPushButton(pushButtonData1);
            splitBtn.AddPushButton(pushButtonData2);
            splitBtn.AddPushButton(pushButtonData3);
        }
        #endregion

        #region AddComboBox()
        public void AddComboBox(RibbonPanel panel)
        {
            //Create five combo box members with two groups.
            ComboBoxMemberData comboBoxMemberData1 = new ComboBoxMemberData("ComboCommandData", "Command Data");
            comboBoxMemberData1.Image = newBitmapImage("Basics.ico");
            comboBoxMemberData1.GroupName = "DB Basics";
            
            ComboBoxMemberData comboBoxMemberData2 = new ComboBoxMemberData("ComboDbElement", "DB Element");
            comboBoxMemberData2.Image = newBitmapImage("Basics.ico");
            comboBoxMemberData2.GroupName = "DB Basics";

            ComboBoxMemberData comboBoxMemberData3 = new ComboBoxMemberData("ComboElementFiltering", "Filtering");
            comboBoxMemberData3.Image = newBitmapImage("Basics.ico");
            comboBoxMemberData3.GroupName = "DB Basics";

            ComboBoxMemberData comboBoxMemberData4 = new ComboBoxMemberData("ComboElementModification", "Modify");
            comboBoxMemberData4.Image = newBitmapImage("Basics.ico");
            comboBoxMemberData4.GroupName = "Modeling";

            ComboBoxMemberData comboBoxMemberData5 = new ComboBoxMemberData("ComboModelCreation", "Create");
            comboBoxMemberData5.Image = newBitmapImage("Basics.ico");
            comboBoxMemberData5.GroupName = "Modeling";

            //Make a combo box
            ComboBoxData comboBxData = new ComboBoxData("ComboBox");
            ComboBox comboBx = panel.AddItem(comboBxData) as ComboBox;
            comboBx.ToolTip = "Select an Option";
            comboBx.LongDescription = "Select a  command you want to run";
            comboBx.AddItem(comboBoxMemberData1);
            comboBx.AddItem(comboBoxMemberData2);
            comboBx.AddItem(comboBoxMemberData3);
            comboBx.AddItem(comboBoxMemberData4);
            comboBx.AddItem(comboBoxMemberData5);

            comboBx.CurrentChanged += new EventHandler<Autodesk.Revit.UI.Events.ComboBoxCurrentChangedEventArgs>(comboBx_currentChanged);
        }

        private void comboBx_currentChanged(object sender, ComboBoxCurrentChangedEventArgs e)
        {
            // Cast sender as TextBox to retrieve text value
            ComboBox combodata = sender as ComboBox;
            ComboBoxMember member = combodata.Current;
            TaskDialog.Show("Combobox Selection", $"Your new selection: {member.ItemText}");
        }
        #endregion

    }

}
