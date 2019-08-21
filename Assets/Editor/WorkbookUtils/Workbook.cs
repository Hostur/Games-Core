using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Editor.WorkbookUtils
{
  /// <summary>
  /// Structure that represents excel workbook.
  /// This structure contains all the values from the workbook as a copy so it doesn't read from the excel file while you are using it.
  /// </summary>

  #region Workbook
  public partial struct Workbook
  {
    public string WorkbookName { get; }

    public List<WorkbookSheet> Sheets { get; private set; }

    public WorkbookSheet GetSheetByName(string name) => Sheets.FirstOrDefault(s => s.Name == name);

    /// <summary>
    /// Gets number of sheets.
    /// </summary>
    public int Length => Sheets.Count;

    public bool Merge()
    {
      if (Sheets.Count < 2)
      {
        return true;
      }


      WorkbookSheet sheet = Sheets[0];
      for (int i = 1; i < Sheets.Count; i++)
      {
        if (!sheet.Merge(Sheets[i]))
        {
          return false;
        }
      }

      Sheets = new List<WorkbookSheet>() { sheet };
      return true;
    }

    public Workbook(string workbookName, List<WorkbookSheet> sheets)
    {
      WorkbookName = workbookName;
      Sheets = sheets;
    }
  }
  #endregion

  #region WorkbookSheet
  public struct WorkbookSheet : IEnumerable<WorkbookRow>
  {
    /// <summary>
    /// Gets sheet name.
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// Gets heading row.
    /// </summary>
    public WorkbookHeader WorkbookHeader { get; private set; }

    /// <summary>
    /// Gets columns.
    /// </summary>
    public List<WorkbookColumn> Columns { get; private set; }

    /// <summary>
    /// Gets rows.
    /// </summary>
    public List<WorkbookRow> Rows { get; private set; }

    public bool Merge(WorkbookSheet other)
    {
      if (!WorkbookHeader.Merge(other.WorkbookHeader))
      {
        return false;
      }

      WorkbookColumn[] backupColumns = new WorkbookColumn[Columns.Count];
      Columns.CopyTo(backupColumns);
      for (int i = 0; i < Columns.Count; i++)
      {
        var column = Columns[i];
        if (!column.Merge(other.Columns[i]))
        {
          Columns = backupColumns.ToList();
          return false;
        }

        Columns[i] = column;
      }

      Rows.AddRange(other.Rows);
      return true;
    }

    /// <summary>
    /// Gets column index if exists.
    /// </summary>
    /// <param name="name">Column name that you are searching for.</param>
    /// <param name="value">Out index value if column exists.</param>
    /// <returns>Value indicating whether given column exists.</returns>
    public bool TryToGetColumnIndexByName(string name, out int value)
    {
      if (WorkbookHeader.Titles.Contains(name))
      {
        value = WorkbookHeader.Titles.IndexOf(name);
        return true;
      }

      value = 0;
      return false;
    }

    public WorkbookSheet(string name, WorkbookHeader workbookHeader, List<WorkbookColumn> columns, List<WorkbookRow> rows)
    {
      Name = name;
      WorkbookHeader = workbookHeader;
      Columns = columns;
      Rows = rows;
    }

    public IEnumerator<WorkbookRow> GetEnumerator()
    {
      for (int i = 0; i < Rows.Count; i++)
      {
        yield return Rows[i];
      }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }
  }
  #endregion

  #region WorkbookHeader
  /// <summary>
  /// Heading row in sheet.
  /// </summary>
  public struct WorkbookHeader : IEnumerable<string>, IEquatable<WorkbookHeader>
  {
    public List<string> Titles;

    /// <summary>
    /// Gets given column title.
    /// </summary>
    /// <param name="index">WorkbookColumn index.</param>
    /// <returns>String with workbookHeader value.</returns>
    public string this[int index] => Titles[index];

    /// <summary>
    /// Columns count.
    /// </summary>
    public int Length => Titles.Count;

    public WorkbookHeader(List<string> titles)
    {
      Titles = titles;
    }

    public bool Merge(WorkbookHeader other)
    {
      if (!Equals(other))
      {
        return false;
      }

      return true;
    }

    public WorkbookHeader(WorkbookRow row)
    {
      Type stringType = typeof(string);
      List<string> titles = new List<string>(row.Length);
      foreach (WorkbookCell cell in row)
      {
        if (cell.CellType != stringType)
        {
          throw new InvalidCastException($"Each cell in WorkbookRow must be a string cell to convert into WorkbookHeader.");
        }
        titles.Add((string)cell.Value);
      }

      Titles = titles;
    }

    public IEnumerator<string> GetEnumerator()
    {
      foreach (string title in Titles)
      {
        yield return title;
      }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }

    public bool Equals(WorkbookHeader other)
    {
      if (Titles.Count != other.Length)
      {
        return false;
      }

      for (int i = 0; i < other.Length; i++)
      {
        if (Titles[i] != other.Titles[i])
        {
          return false;
        }
      }

      return true;
    }

    public override bool Equals(object obj)
    {
      if (ReferenceEquals(null, obj)) return false;
      return obj is WorkbookHeader && Equals((WorkbookHeader)obj);
    }

    public override int GetHashCode()
    {
      return (Titles != null ? Titles.GetHashCode() : 0);
    }
  }

  #endregion

  #region WorkbookCell
  /// <summary>
  /// Structure represents workbook cell.
  /// </summary>
  public struct WorkbookCell : IEquatable<WorkbookCell>
  {
    /// <summary>
    /// Type of cell.
    /// </summary>
    public Type CellType { get; private set; }

    /// <summary>
    /// Underlying cell value.
    /// </summary>
    public object Value { get; }

    public WorkbookCell(object value)
    {
      Value = value;
      CellType = (value != null) ? Value.GetType() : null;
    }

    public bool Equals(WorkbookCell other)
    {
      return (CellType == other.CellType && Value == other.Value);
    }

    public override bool Equals(object obj)
    {
      if (!(obj is WorkbookCell))
      {
        return false;
      }

      return Equals((WorkbookCell)obj);
    }

    public override int GetHashCode()
    {
      unchecked
      {
        return ((CellType != null ? CellType.GetHashCode() : 0) * 397) ^ (Value != null ? Value.GetHashCode() : 0);
      }
    }
  }
  #endregion

  #region WrokbookRow
  /// <summary>
  /// Horizontal set of <see cref="WorkbookCell"/>.
  /// </summary>
  public struct WorkbookRow : IEnumerable<WorkbookCell>, IEquatable<WorkbookRow>
  {
    private List<WorkbookCell> _cells;
    public WorkbookCell this[int index] => _cells[index];
    public int Length => _cells.Count;

    public WorkbookRow(List<WorkbookCell> cells)
    {
      _cells = cells;
    }

    public IEnumerator<WorkbookCell> GetEnumerator()
    {
      foreach (WorkbookCell cell in _cells)
      {
        yield return cell;
      }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }

    public bool Equals(WorkbookRow other)
    {
      if (Length != other.Length)
      {
        return false;
      }

      for (int i = 0; i < Length; i++)
      {
        if (!_cells[i].Equals(other[i]))
        {
          return false;
        }
      }

      return true;
    }

    public override bool Equals(object obj)
    {
      if (!(obj is WorkbookRow))
      {
        return false;
      }

      return Equals((WorkbookRow)obj);
    }

    public override int GetHashCode()
    {
      return (_cells != null ? _cells.GetHashCode() : 0);
    }
  }
  #endregion

  #region WorkbookColumn
  /// <summary>
  /// Vertical set of <see cref="WorkbookCell"/>.
  /// IMPORTANT!! Each column contains string header with title on the first <see cref="WorkbookCell"/>.
  /// </summary>
  public struct WorkbookColumn : IEnumerable<WorkbookCell>
  {
    public string Title { get; private set; }
    public List<WorkbookCell> Cells { get; private set; }
    public int Length => Cells.Count;

    public WorkbookColumn(string title, List<WorkbookCell> cells)
    {
      Title = title;
      Cells = cells;
    }

    public bool Merge(WorkbookColumn column)
    {
      if (Title == column.Title)
      {
        // To this.Cells add all the cells from the second column without header
        var cells = column.Cells.GetRange(1, column.Cells.Count - 1);
        Cells.AddRange(cells);
        return true;
      }

      return false;
    }

    public IEnumerator<WorkbookCell> GetEnumerator()
    {
      foreach (WorkbookCell cell in Cells)
      {
        yield return cell;
      }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }
  }
  #endregion
}
