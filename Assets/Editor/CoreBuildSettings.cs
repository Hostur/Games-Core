using System;
using System.Collections.Generic;
using System.Linq;
using Core.Collections;
using Development.Logging;
using UnityEditor;
using UnityEngine;

namespace Editor
{
  public class CoreBuildSettings : EditorWindow
  {
#if IS_CLIENT
    private const string TITLE = "(Client)";
#elif IS_SERVER
    private const string TITLE = "(Server");
#else
    private const string TITLE = "(Build undefined)";
#endif

    private static DefineSymbols[] _toggles;

    private static DefineSymbols[] Toggles
    {
      get
      {
        if (_toggles == null)
        {
          _toggles = new DefineSymbols[Symbols.Length];
          for (int i = 0; i < Symbols.Length; i++)
          {
            _toggles[i] = new DefineSymbols { Define = false, Symbol = Symbols[i] };
          }
        }
        return _toggles;
      }

      set { _toggles = value; }
    }

    public static readonly string[] Symbols =
    {
          "IS_SERVER",
          "IS_CLIENT",
          "DEVELOPMENT"
        };

    private class DefineSymbols
    {
      public string Symbol;
      public bool Define;
    }

    [MenuItem(TITLE + "/(switch) &%l")]
    public static void SwitchContext()
    {
      AssignCurrentSymbolsToToggles();

      if (Toggles.FirstOrDefault(t => t.Symbol == "IS_SERVER").Define)
      {
        EnableClient();
        DisableServer();
      }
      else if (Toggles.FirstOrDefault(t => t.Symbol == "IS_CLIENT").Define)
      {
        EnableServer();
        DisableClient();
      }

      AddDefineSymbols();
    }

    [MenuItem(TITLE+"/Settings")]
    public static void Init()
    {
      AssignCurrentSymbolsToToggles();
      GetWindow(typeof(CoreBuildSettings)).Show();
    }

    private static void AssignCurrentSymbolsToToggles()
    {
      string definesString = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);

      foreach (string s in definesString.Split(';'))
      {
        DefineSymbols symbols = Toggles.FirstOrDefault(t => t.Symbol == s);
        if (symbols != null)
        {
          symbols.Define = true;
        }
      }
    }

    private static void AddDefineSymbols()
    {
      // Current symbols.
      string definesString = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);

      // Symbols which are set as false in toggles.
      List<string> excludes = Toggles.Where(t => !t.Define).Select(t => t.Symbol).ToList();

      // Current symbols without excludes.
      List<string> allDefines = definesString.Split(';').ToList().Except(excludes).ToList();

      // Add non existing symbols provided by toggles to current symbols avoiding duplication.
      allDefines.AddRange(Toggles.Where(t => t.Define).Select(t => t.Symbol).ToList().Except(allDefines));

      // Remove empty strings.
      allDefines.RemoveAll(string.IsNullOrEmpty);

      // Apply symbols.
      PlayerSettings.SetScriptingDefineSymbolsForGroup(
          EditorUserBuildSettings.selectedBuildTargetGroup,
          string.Join(";", allDefines.ToArray()));

      // Close window.
      GetWindow(typeof(CoreBuildSettings)).Close();
    }

    private static void DisableClient()
    {
      Toggles.Each(t =>
      {
        if (t.Symbol == "IS_CLIENT")
        {
          t.Define = false;
        }
      });
    }

    private static void EnableClient()
    {
      Toggles.Each(t =>
      {
        if (t.Symbol == "IS_CLIENT")
        {
          t.Define = true;
        }
      });
    }


    private static void EnableServer()
    {
      Toggles.Each(t =>
      {
        if (t.Symbol == "IS_SERVER")
        {
          t.Define = true;
        }
      });
    }

    private static void DisableServer()
    {
      Toggles.Each(t =>
      {
        if (t.Symbol == "IS_SERVER")
        {
          t.Define = false;
        }
      });
    }

    void OnGUI()
    {
      try
      {
        foreach (DefineSymbols defineSymbols in Toggles)
        {
          defineSymbols.Define = GUILayout.Toggle(defineSymbols.Define, defineSymbols.Symbol);
          if (defineSymbols.Define)
          {
            switch (defineSymbols.Symbol)
            {
              case "IS_SERVER":
                DisableClient();
                break;
              case "IS_CLIENT":
                DisableServer();
                break;
            }
          }
        }
      }
      catch (Exception e)
      {
        this.Log("Exception occur during CoreBuildSettings:OnGUI. " + e, LogLevel.Error);
      }
      finally
      {
        if (GUILayout.Button("Define symbols"))
        {
          AddDefineSymbols();
        }
      }
    }
  }
}
