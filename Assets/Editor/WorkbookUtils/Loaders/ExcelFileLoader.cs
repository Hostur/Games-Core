using System;
using System.IO;
using System.Threading.Tasks;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace Editor.WorkbookUtils.Loaders
{
  internal class ExcelFileLoader
  {
    private const string XLS = ".xls";
    private const string XLSX = ".xlsx";
    private const string TMP_PREFIX = "~$";

    internal static bool IsExcelFile(string path)
    {
      return !Path.GetFileName(path).StartsWith(TMP_PREFIX) && (path.EndsWith(XLS) || path.EndsWith(XLSX));
    }

    /// <summary>
    /// Function which load excel file into the <see cref="IWorkbook"/> proxy.
    /// Async is a fake to make it easily callable as a task.
    /// </summary>
    /// <param name="path">VALIDATED path to the excel file.</param>
    /// <returns>Async result with <see cref="IWorkbook"/>.</returns>
    internal async Task<IWorkbook> GetWorkBook(string path)
    {
      await Task.Delay(0);
      IWorkbook result;
      using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
      {
        if (path.EndsWith(XLS))
        {
          result = new HSSFWorkbook(fileStream);
        }
        else if (path.EndsWith(XLSX))
        {
          result = new XSSFWorkbook(fileStream);
        }
        else
        {
          throw new Exception("Bad excel file extension.");
        }
      }

      return result;
    }
  }
}
