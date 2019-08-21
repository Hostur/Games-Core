using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using NPOI.OpenXml4Net.Exceptions;
using NPOI.SS.UserModel;

namespace Editor.WorkbookUtils.Converters
{
  internal class ExcelWorkbookConverter
  {
    /// <summary>
    /// Creates cached structure with contains all the data from the given <see cref="IWorkbook"/>.
    /// </summary>
    /// <param name="workbookName">Workbook name.</param>
    /// <param name="workbook"><see cref="IWorkbook"/> that can read from the existing excel file.</param>
    /// <returns>Async task with <see cref="Workbook"/>. Async is a fake to make it easily callable as a task.</returns>
    internal async Task<Workbook> AsyncCreate(string workbookName, IWorkbook workbook)
    {
      int sheetsNumber = workbook.NumberOfSheets;
      List<WorkbookSheet> sheets = new List<WorkbookSheet>(sheetsNumber);
      for (int i = 0; i < sheetsNumber; i++)
      {
        try
        {
          WorkbookSheet workbookSheet = CreateSheet(workbookName, workbook.GetSheetAt(i));
          sheets.Add(workbookSheet);
        }
        catch (Exception e)
        {
          Trace.Write("Invalid format in one or more sheets in given file. " + e.Message, "Error");
        }
      }

      await Task.Delay(0);
      return new Workbook(workbookName, sheets);
    }

    private WorkbookSheet CreateSheet(string workbookName, ISheet sheet)
    {
      Assert(workbookName, sheet);
      if (!GetHeader(sheet.GetRow(0), out var header))
      {
        throw new InvalidFormatException($"WorkbookSheet '{sheet.SheetName}' in workbook '{workbookName}' has no workbookHeader. WorkbookHeader should be the first row with only text values.");
      }

      List<WorkbookColumn> columns = new List<WorkbookColumn>(header.Length);
      List<WorkbookRow> rows = new List<WorkbookRow>(sheet.LastRowNum);

      for (int i = 0; i < sheet.LastRowNum; i++)
      {
        WorkbookRow workbookRow = GetRow(sheet.GetRow(i), header.Length);
        rows.Add(workbookRow);
      }

      for (int i = 0; i < header.Length; i++)
      {
        WorkbookColumn tmpWorkbookColumn = GetColumn(i, header[i], ref rows);
        columns.Add(tmpWorkbookColumn);
      }

      return new WorkbookSheet(sheet.SheetName, header, columns, rows);
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

    private static WorkbookRow GetRow(IRow row, int headerLength)
    {
      List<WorkbookCell> cells = new List<WorkbookCell>(row.LastCellNum);
      for (int i = 0; i < headerLength; i++)
      {
        if (i < row.LastCellNum)
        {
          ICell tmpCell = row.GetCell(i);
          cells.Add(new WorkbookCell(GetCellValue(tmpCell)));
        }
        else
        {
          cells.Add(new WorkbookCell(null));
        }
      }
      return new WorkbookRow(cells);
    }

    private static object GetCellValue(ICell cell)
    {
      if (cell == null)
      {
        return null;
      }

      switch (cell.CellType)
      {
        case CellType.Boolean:
          return cell.BooleanCellValue;
        case CellType.Numeric:
          return cell.NumericCellValue;
        case CellType.String:
          return cell.StringCellValue;
        default:
          return null;
      }
    }

    private static void Assert(string workbookName, ISheet sheet)
    {
      if (sheet == null)
      {
        throw new InvalidFormatException($"Error occur during reading a sheet from workbook '{workbookName}'.");
      }

      if (sheet.LastRowNum == 0)
      {
        throw new InvalidFormatException($"WorkbookSheet '{sheet.SheetName}' from workbook '{workbookName}' has no rows.");
      }
    }

    private static bool GetHeader(IRow headingRow, out WorkbookHeader result)
    {
      try
      {
        int cellsQuantity = headingRow.LastCellNum;
        if (cellsQuantity == 0)
        {
          result = new WorkbookHeader();
          return false;
        }

        List<string> columnHeaders = new List<string>(cellsQuantity);
        for (int i = 0; i < cellsQuantity; i++)
        {
          string value = headingRow.GetCell(i).StringCellValue;
          columnHeaders.Add(value);
        }

        result = new WorkbookHeader(columnHeaders);
        return true;
      }
      catch
      {
        result = new WorkbookHeader();
        return false;
      }
    }
  }
}
