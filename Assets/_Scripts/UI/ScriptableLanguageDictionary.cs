using System;
using System.Collections.Generic;
using Core.Balance;
using Development.Logging;
using UnityEngine;

namespace Localization
{
  public class ScriptableLanguageDictionary : ScriptableCollection<DictionaryRecord>
  {
    protected override string ConcretePath => "LanguageDictionary";

    public Dictionary<string, string> GetDictionary(GameLanguage language)
    {
      try
      {
        Dictionary<string, string> dict = new Dictionary<string, string>(Entities.Length);
        switch (language)
        {
          case GameLanguage.PL:
          {
            for (int i = 0; i < Entities.Length; i++)
              dict.Add(Entities[i].Tag, Entities[i].Polish);
            return dict;
          }
          default:
          {
            for (int i = 0; i < Entities.Length; i++)
              dict.Add(Entities[i].Tag, Entities[i].English);
            return dict;
          }
        }
      }
      catch (Exception e)
      {
        this.Log("Exception occur during creating dictionary: " + e, LogLevel.Error);
        return new Dictionary<string, string>(0);
      }
    }
  }

  [SerializeField]
  [Serializable]
  [ClassMapping(true, typeof(ScriptableLanguageDictionary), "Assets/Editor/EditorResources/Dictionary.xlsx")]
  public struct DictionaryRecord
  {
    public string Tag;
    public string English;
    public string Polish;
  }
}
