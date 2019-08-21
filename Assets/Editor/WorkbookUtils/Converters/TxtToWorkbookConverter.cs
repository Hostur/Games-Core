using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Editor.WorkbookUtils.Loaders;

namespace Editor.WorkbookUtils.Converters
{
  internal static class TxtToWorkbookConverter
  {
    private const char TAB_SEPARATOR = '\t';

    /// <summary>
    /// Creates cached structure with contains all the data from the given text file paths.
    /// </summary>
    /// <param name="workbookName">Workbook name.</param>
    /// <param name="files">File paths with text workbooks.</param>
    /// <returns>Async task with <see cref="Workbook"/>.</returns>
    internal static async Task<Workbook> AsyncCreate(string workbookName, string[] files)
    {
      int sheetsNumber = files.Length;
      List<WorkbookSheet> sheets = new List<WorkbookSheet>(sheetsNumber);
      for (int i = 0; i < sheetsNumber; i++)
      {
        WorkbookSheet workbookSheet = await CreateSheet(files[i]).ConfigureAwait(false);
        sheets.Add(workbookSheet);
      }

      return new Workbook(workbookName, sheets);
    }

    /// <summary>
    /// Single file is a single workbook sheet.
    /// </summary>
    /// <param name="filePath">Path to single txt file.</param>
    /// <returns>Async tack with <see cref="WorkbookSheet"/>.</returns>
    private static async Task<WorkbookSheet> CreateSheet(string filePath)
    {
      string[] sheetLines = await TextFileLoader.LoadTxtFile(filePath).ConfigureAwait(false);

      WorkbookHeader header = new WorkbookHeader(GetRow(sheetLines[0]));
      List<WorkbookColumn> columns = new List<WorkbookColumn>(header.Length);
      List<WorkbookRow> rows = new List<WorkbookRow>(sheetLines.Length);

      // starts from 1 because line 0 is a header
      for (int i = 0; i < sheetLines.Length; i++)
      {
        WorkbookRow workbookRow = GetRow(sheetLines[i]);
        rows.Add(workbookRow);
      }

      for (int i = 0; i < header.Length; i++)
      {
        WorkbookColumn tmpWorkbookColumn = GetColumn(i, header[i], ref rows);
        columns.Add(tmpWorkbookColumn);
      }

      return new WorkbookSheet(Path.GetFileName(filePath), header, columns, rows);
    }

    private static WorkbookColumn GetColumn(int index, string title, ref List<WorkbookRow> rows)
    {
      List<WorkbookCell> cells = new List<WorkbookCell>(rows.Count);
      for (int i = 0; i < rows.Count; i++)
      {
        cells.Add(rows[i][index]);
      }

      return new WorkbookColumn(title, cells);
    }

    private static WorkbookRow GetRow(string row)
    {
      string[] sCells = row.Split(TAB_SEPARATOR);
      List<WorkbookCell> cells = new List<WorkbookCell>(sCells.Length);
      for (int i = 0; i < sCells.Length; i++)
      {
        cells.Add(new WorkbookCell(sCells[i]));
      }
      return new WorkbookRow(cells);
    }
  }
}
