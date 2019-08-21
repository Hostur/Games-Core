using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Editor.WorkbookUtils.Converters;
using Editor.WorkbookUtils.Loaders;
using NPOI.SS.UserModel;

namespace Editor.WorkbookUtils
{
  public partial struct Workbook
  {
    /// <summary>
    /// Creates <see cref="Workbook"/> from the path to the existing excel file.
    /// </summary>
    /// <param name="workbookName">Workbook name if you want it to be different than file name.</param>
    /// <param name="filePath">Path to the existing excel file.</param>
    /// <returns>Async result with <see cref="Workbook"/>.</returns>
    public static async Task<Workbook> CreateFromExcel(string workbookName, string filePath)
    {
      workbookName = string.IsNullOrEmpty(workbookName)
          ? Path.GetFileNameWithoutExtension(filePath)
          : workbookName;

      IWorkbook workbook = await new ExcelFileLoader().GetWorkBook(filePath).ConfigureAwait(false);
      return await new ExcelWorkbookConverter().AsyncCreate(workbookName, workbook).ConfigureAwait(false);
    }

    /// <summary>
    /// Creates <see cref="Workbook"/> from the paths to the existing txt files.
    /// </summary>
    /// <param name="files">Paths to the existing txt files.</param>
    /// <returns>Async result with <see cref="Workbook"/>.</returns>
    public static async Task<Workbook> CreateFromTxtFiles(string workbookName, string[] files)
    {
      return await TxtToWorkbookConverter.AsyncCreate(workbookName, files);
    }

    /// <summary>
    /// Creates <see cref="Workbook"/> from the txt files in given directory.
    /// </summary>
    /// <param name="workbookName">Workbook name.</param>
    /// <param name="directory">Paths to the existing directory which contains txt files.</param>
    /// <returns>Async result with <see cref="Workbook"/>.</returns>
    public static async Task<Workbook> CreateFromTxtFiles(string workbookName, string directory)
    {
      if (!Directory.Exists(directory))
      {
        throw new Exception($"There is no such directory '{directory}'.");
      }

      var files = Directory.GetFiles(directory).Where(TextFileLoader.IsTxtFile).ToArray();
      if (files.Length == 0)
      {
        throw new Exception($"There is no .txt files in given directory '{directory}'");
      }
      return await CreateFromTxtFiles(workbookName, files.ToArray());
    }
  }
}
