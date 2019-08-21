using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Core.Balance;
using Core.Reflection;

namespace Editor.WorkbookUtils
{
  public class WorkbookClassMapper
  {
    private struct TypeData
    {
      public Type Type { get; }

      public bool Automapping { get; }

      public string WorkbookName { get; }

      public string SheetName { get; }

      public TypeData(Type type, bool automapping, string workbookName, string sheetName = "")
      {
        Type = type;
        Automapping = automapping;
        SheetName = sheetName;
        WorkbookName = workbookName;
      }
    }

    public async Task<KeyValuePair<Type, IList>> MapType(TypeWithAttribute<ClassMappingAttribute> typeToMap)
    {
      Workbook workbook = await Workbook.CreateFromExcel(
                              Path.GetFileNameWithoutExtension(typeToMap.Attribute.WorkbookPath),
                              typeToMap.Attribute.WorkbookPath).ConfigureAwait(false);

      // Try to merge sheets inside this workbook.
      // If one balance workbook is splited into separated sheets that share the same structure.
      workbook.Merge();
      // If ClassMappingAttribute contains sheet name than try to get it from the workbook else take first.
      WorkbookSheet sheet = string.IsNullOrEmpty(typeToMap.Attribute.SheetName)
          ? workbook.Sheets[0] : workbook.GetSheetByName(typeToMap.Attribute.SheetName);

      if (sheet.Columns.Count == 0 || sheet.Rows.Count == 0)
      {
        throw new Exception($"Sheet '{sheet.Name}' in workbook '{workbook.WorkbookName}' is empty");
      }

      return await MapType(typeToMap.Type, sheet, typeToMap.Attribute.Automapping).ConfigureAwait(false);
    }

    /// <summary>
    /// Map all the types decorated by <see cref="ClassMappingAttribute"/> from given workbooks files.
    /// </summary>
    /// <param name="workbookPaths">VALIDATED excel workbook paths.</param>
    /// <returns>IEnumerable dictionary with all the types and values.</returns>
    public async Task<WorkbookClassMapperResult> MapTypes(string[] workbookPaths)
    {
      Workbook[] workbooks = new Workbook[workbookPaths.Length];

      for (int i = 0; i < workbookPaths.Length; i++)
      {
        workbooks[i] = await Workbook.CreateFromExcel(workbookPaths[i], workbookPaths[i]).ConfigureAwait(false);
      }

      var decoratedClasses = AppDomain.CurrentDomain.GetAssemblies()
          .SelectMany(a => a.GetTypes())
          .Where(t => t.IsDefined(typeof(ClassMappingAttribute)))
          .Select(t =>
          {
            var attribute = t.GetCustomAttribute<ClassMappingAttribute>();
            return new TypeData(t, attribute.Automapping, attribute.WorkbookPath, attribute.SheetName);
          });

      Dictionary<Type, IList> resultInput = new Dictionary<Type, IList>(decoratedClasses.Count());
      foreach (TypeData typeData in decoratedClasses)
      {
        if (workbooks.All(t => t.WorkbookName != typeData.WorkbookName))
        {
          throw new Exception($"Workbook '{typeData.WorkbookName}' for given type '{typeData.Type.FullName}' not found.");
        }
        // Get workbook if match name with workbook name from ClassMappingAttribute.
        Workbook workbook = workbooks.FirstOrDefault(t => t.WorkbookName == typeData.WorkbookName);


        // If ClassMappingAttribute contains sheet name than try to get it from the workbook else take first.
        WorkbookSheet sheet = string.IsNullOrEmpty(typeData.SheetName)
            ? workbook.Sheets[0] : workbook.GetSheetByName(typeData.SheetName);

        if (sheet.Columns.Count == 0 || sheet.Rows.Count == 0)
        {
          throw new Exception($"Sheet '{sheet.Name}' in workbook '{workbook.WorkbookName}' is empty");
        }

        var singleTypeResult = await MapType(typeData.Type, sheet, typeData.Automapping).ConfigureAwait(false);
        resultInput.Add(singleTypeResult.Key, singleTypeResult.Value);
      }

      return new WorkbookClassMapperResult(resultInput);
    }

    private async Task<KeyValuePair<Type, IList>> MapType(Type type, WorkbookSheet sheet, bool automapping)
    {
      return automapping ? await AutoMapType(type, sheet) : await ManualMapType(type, sheet);
    }
    private async Task<KeyValuePair<Type, IList>> AutoMapType(Type type, WorkbookSheet sheet)
    {
      await Task.Delay(0).ConfigureAwait(false);
      var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance)
          .Where(w => sheet.WorkbookHeader.Titles.Contains(w.Name));

      var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
          .Where(w => sheet.WorkbookHeader.Titles.Contains(w.Name));

      // Iterate from 1 because header is on the index 0.
      List<object> values = new List<object>(sheet.Rows.Count - 1);
      for (int r = 1; r < sheet.Rows.Count; r++)
      {
        var row = sheet.Rows[r];
        object obj = Activator.CreateInstance(type);
        foreach (FieldInfo field in fields)
        {
          FieldInfo fieldInfo = type.GetField(field.Name);
          if (sheet.TryToGetColumnIndexByName(fieldInfo.Name, out int value))
          {
            // If we can cast type
            if (!ReflectionParser.SetValue(fieldInfo, obj, row[value].Value))
            {
              throw new Exception(
                  $"Field '{fieldInfo.Name}' in type '{type.FullName}' doesn't match the type of data in WorkbookSheet '{sheet.Name}' in column '{value}'");
            }
          }
        }

        foreach (PropertyInfo property in properties)
        {
          PropertyInfo p = type.GetProperty(property.Name);
          if (sheet.TryToGetColumnIndexByName(p.Name, out int value))
          {
            if (!ReflectionParser.SetValue(p, obj, row[value].Value))
            {
              throw new Exception(
                  $"Property '{p.Name}' in type '{type.FullName}' doesn't match the type of data in WorkbookSheet '{sheet.Name}' in column '{value}'");
            }
          }
        }

        values.Add(obj);
      }

      return new KeyValuePair<Type, IList>(type, values);
    }

    private async Task<KeyValuePair<Type, IList>> ManualMapType(Type type, WorkbookSheet sheet)
    {
      await Task.Delay(0).ConfigureAwait(false);
      var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance)
          .Where(f => f.IsDefined(typeof(FieldMappingAttribute)));

      var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
          .Where(f => f.IsDefined(typeof(FieldMappingAttribute)));


      List<object> values = new List<object>(sheet.Rows.Count - 1);
      for (int r = 1; r < sheet.Rows.Count; r++)
      {
        var row = sheet.Rows[r];
        object obj = Activator.CreateInstance(type);
        foreach (FieldInfo field in fields)
        {
          FieldInfo fieldInfo = type.GetField(field.Name);
          string lookingForColumnName =
              fieldInfo.GetCustomAttribute<FieldMappingAttribute>().WorkbookHeader;

          if (sheet.TryToGetColumnIndexByName(lookingForColumnName, out int value))
          {
            if (!ReflectionParser.SetValue(fieldInfo, obj, row[value].Value))
            {
              throw new Exception($"Field '{fieldInfo.Name}' in type '{type.FullName}' doesn't match the type of data in WorkbookSheet '{sheet.Name}' in column '{value}'");
            }
          }
          else
          {
            throw new Exception($"In workbooksheet '{sheet.Name}' there is no such a column like '{lookingForColumnName}' defined in '{type.FullName}' on field '{fieldInfo.Name}'");
          }
        }
        foreach (PropertyInfo property in properties)
        {
          PropertyInfo p = type.GetProperty(property.Name);
          string lookingForColumnName =
              p.GetCustomAttribute<FieldMappingAttribute>().WorkbookHeader;

          if (sheet.TryToGetColumnIndexByName(lookingForColumnName, out int value))
          {
            if (!ReflectionParser.SetValue(p, obj, row[value].Value))
            {
              throw new Exception($"Property '{p.Name}' in type '{type.FullName}' doesn't match the type of data in WorkbookSheet '{sheet.Name}' in column '{value}'");
            }
          }
          else
          {
            throw new Exception($"In workbooksheet '{sheet.Name}' there is no such a column like '{lookingForColumnName}' defined in '{type.FullName}' on property '{p.Name}'");
          }
        }
        values.Add(obj);
      }
      return new KeyValuePair<Type, IList>(type, values);
    }
  }
}
