using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Editor
{
  [Serializable]
  public class AutocompleteSearchField
  {
    static class Styles
    {
      public const float RESULT_HEIGHT = 20f;
      public const float RESULTS_BORDER_WIDTH = 2f;
      public const float RESULTS_MARGIN = 15f;
      public const float RESULTS_LABEL_OFFSET = 2f;

      public static readonly GUIStyle EntryEven;
      public static readonly GUIStyle EntryOdd;
      public static readonly GUIStyle LabelStyle;
      public static readonly GUIStyle ResultsBorderStyle;

      static Styles()
      {
        EntryOdd = new GUIStyle("CN EntryBackOdd");
        EntryEven = new GUIStyle("CN EntryBackEven");
        ResultsBorderStyle = new GUIStyle("hostview");

        LabelStyle = new GUIStyle(EditorStyles.label)
        {
          alignment = TextAnchor.MiddleLeft,
          richText = true
        };
      }
    }

    public AutocompleteSearchField(string initialValue = null)
    {
      if (initialValue != null)
        SearchString = initialValue;
    }

    public Action<string> OnInputChanged;
    public Action<string> onConfirm;
    public string SearchString;
    public int MaxResults = 15;

    [SerializeField] private readonly List<string> _results = new List<string>();

    [SerializeField]
    private int _selectedIndex = -1;

    private SearchField _searchField;

    private Vector2 _previousMousePosition;
    private bool _selectedIndexByMouse;
    private bool _showResults;

    public void AddResult(string result)
    {
      _results.Add(result);
    }

    public void ClearResults()
    {
      _results.Clear();
    }

    public void OnToolbarGUI()
    {
      Draw(asToolbar: true);
    }

    public void OnGUI()
    {
      Draw(asToolbar: false);
    }

    private void Draw(bool asToolbar)
    {
      var rect = GUILayoutUtility.GetRect(1, 1, 18, 18, GUILayout.ExpandWidth(true));
      GUILayout.BeginHorizontal();
      DoSearchField(rect, asToolbar);
      GUILayout.EndHorizontal();
      rect.y += 18;
      DoResults(rect);
    }

    private void DoSearchField(Rect rect, bool asToolbar)
    {
      if (_searchField == null)
      {
        _searchField = new SearchField();
        _searchField.downOrUpArrowKeyPressed += OnDownOrUpArrowKeyPressed;
      }

      var result = asToolbar
       ? _searchField.OnToolbarGUI(rect, SearchString)
       : _searchField.OnGUI(rect, SearchString);

      if (result != SearchString && OnInputChanged != null)
      {
        RefreshResult(result);
      }

      SearchString = result;

      if (HasSearchBarFocused())
      {
        RepaintFocusedWindow();
      }
    }

    public void RefreshResult(string result)
    {
      OnInputChanged(result);
      _selectedIndex = -1;
      _showResults = true;
    }

    private void OnDownOrUpArrowKeyPressed()
    {
      var current = Event.current;

      if (current.keyCode == KeyCode.UpArrow)
      {
        current.Use();
        _selectedIndex--;
        _selectedIndexByMouse = false;
      }
      else
      {
        current.Use();
        _selectedIndex++;
        _selectedIndexByMouse = false;
      }

      if (_selectedIndex >= _results.Count) _selectedIndex = _results.Count - 1;
      else if (_selectedIndex < 0) _selectedIndex = -1;
    }

    private void DoResults(Rect rect)
    {
      if (_results.Count <= 0 || !_showResults) return;

      var current = Event.current;
      rect.height = Styles.RESULT_HEIGHT * Mathf.Min(MaxResults, _results.Count);
      rect.x = Styles.RESULTS_MARGIN;
      rect.width -= Styles.RESULTS_MARGIN * 2;

      var elementRect = rect;

      rect.height += Styles.RESULTS_BORDER_WIDTH;
      GUI.Label(rect, "", Styles.ResultsBorderStyle);

      var mouseIsInResultsRect = rect.Contains(current.mousePosition);

      if (mouseIsInResultsRect)
      {
        RepaintFocusedWindow();
      }

      var movedMouseInRect = _previousMousePosition != current.mousePosition;

      elementRect.x += Styles.RESULTS_BORDER_WIDTH;
      elementRect.width -= Styles.RESULTS_BORDER_WIDTH * 2;
      elementRect.height = Styles.RESULT_HEIGHT;

      var didJustSelectIndex = false;

      for (var i = 0; i < _results.Count && i < MaxResults; i++)
      {
        if (current.type == EventType.Repaint)
        {
          var style = i % 2 == 0 ? Styles.EntryOdd : Styles.EntryEven;

          style.Draw(elementRect, false, false, i == _selectedIndex, false);

          var labelRect = elementRect;
          labelRect.x += Styles.RESULTS_LABEL_OFFSET;
          GUI.Label(labelRect, _results[i], Styles.LabelStyle);
        }
        if (elementRect.Contains(current.mousePosition))
        {
          if (movedMouseInRect)
          {
            _selectedIndex = i;
            _selectedIndexByMouse = true;
            didJustSelectIndex = true;
          }
          if (current.type == EventType.MouseDown)
          {
            OnConfirm(_results[i]);
          }
        }
        elementRect.y += Styles.RESULT_HEIGHT;
      }

      if (current.type == EventType.Repaint && !didJustSelectIndex && !mouseIsInResultsRect && _selectedIndexByMouse)
      {
        _selectedIndex = -1;
      }

      if ((GUIUtility.hotControl != _searchField.searchFieldControlID && GUIUtility.hotControl > 0)
       || (current.rawType == EventType.MouseDown && !mouseIsInResultsRect))
      {
        _showResults = false;
      }

      if (current.type == EventType.KeyUp && current.keyCode == KeyCode.Return && _selectedIndex >= 0)
      {
        OnConfirm(_results[_selectedIndex]);
      }

      if (current.type == EventType.Repaint)
      {
        _previousMousePosition = current.mousePosition;
      }
    }

    private void OnConfirm(string result)
    {
      SearchString = result;
      onConfirm?.Invoke(result);
      OnInputChanged?.Invoke(result);
      RepaintFocusedWindow();
      GUIUtility.keyboardControl = 0; // To avoid Unity sometimes not updating the search field text
    }

    private bool HasSearchBarFocused()
    {
      return GUIUtility.keyboardControl == _searchField.searchFieldControlID;
    }

    private static void RepaintFocusedWindow()
    {
      if (EditorWindow.focusedWindow != null)
      {
        EditorWindow.focusedWindow.Repaint();
      }
    }
  }
}

