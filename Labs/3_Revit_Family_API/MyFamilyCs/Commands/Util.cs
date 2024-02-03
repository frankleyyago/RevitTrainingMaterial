#region Namespaces
using System;
using System.Diagnostics;
using WinForms = System.Windows.Forms;
#endregion

namespace MyFamilyCs
{
  public class Util
  {
    #region Formatting and message handlers
    public const string Caption = "Revit Family API Labs";

    /// <summary>
    /// MessageBox wrapper for informational message.
    /// </summary>
    public static void InfoMsg(string msg)
    {
      Debug.WriteLine(msg);
      WinForms.MessageBox.Show(msg, Caption, WinForms.MessageBoxButtons.OK, WinForms.MessageBoxIcon.Information);
    }

    /// <summary>
    /// MessageBox wrapper for error message.
    /// </summary>
    public static void ErrorMsg(string msg)
    {
      WinForms.MessageBox.Show(msg, Caption, WinForms.MessageBoxButtons.OK, WinForms.MessageBoxIcon.Error);
    }
    #endregion // Formatting and message handlers
  }
}
