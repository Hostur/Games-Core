using System;
using System.Collections.Generic;

namespace Editor.WorkbookUtils
{
  public static class WorkbookProxyExtension
  {
    /// <summary>
    /// Merge two <see cref="Workbook"/> structures.
    /// </summary>
    /// <param name="workbook">First workbook.</param>
    /// <param name="workbook2">Second workbook</param>
    /// <param name="softMerge">False mean that both workbook should be mergeable as self and structure of the sheets should be the same.</param>
    /// <returns>New merged workbook which contains sheets from both of the structures(softMerge) or one sheet with all the data(hardMerge).</returns>
    public static Workbook Merge(
      this Workbook workbook,
      Workbook workbook2, bool softMerge = true)
    {
      if (softMerge)
      {
        foreach (WorkbookSheet sheet in workbook2.Sheets)
        {
          workbook.Sheets.Add(sheet);
        }
        return workbook;
      }

      if (!workbook.Merge() || !workbook2.Merge())
      {
        throw new Exception("In hard workbook merge each workbook must be mergeable as self.");
      }

      if (!workbook.Sheets[0].Merge(workbook2.Sheets[0]))
      {
        throw new Exception("Can't merge these two workbook workbooks. Sheets structure is different.");
      }
      return workbook;
    }

    public static Workbook Merge(this Workbook workbook, WorkbookSheet sheet, bool softMerge = true)
    {
      return Merge(workbook, new Workbook(string.Empty, new List<WorkbookSheet>() { sheet }), softMerge);
    }
  }
}
