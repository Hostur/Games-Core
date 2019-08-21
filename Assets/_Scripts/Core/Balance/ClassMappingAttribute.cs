using System;

namespace Core.Balance
{
  public class ClassMappingAttribute : Attribute
  {
    /// <summary>
    /// Value indicating whether this class should be mapped automatic.
    /// If not you should use <see cref="FieldMappingAttribute"/> above each field that you want to map from excel file.
    /// </summary>
    public bool Automapping { get; }


    public Type ScriptableType { get; }

    public string WorkbookPath { get; }

    public string SheetName { get; }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="automapping"> Value indicating whether should try to map properties by name.
    /// If false than each field you want to map should be decorated by <see cref="FieldMappingAttribute"/>.</param>
    public ClassMappingAttribute(bool automapping, Type scriptableType, string workbookPath, string sheetName = "")
    {
      Automapping = automapping;
      ScriptableType = scriptableType;
      WorkbookPath = workbookPath;
      SheetName = sheetName;
    }

    private ClassMappingAttribute()
    {
    }
  }
}
