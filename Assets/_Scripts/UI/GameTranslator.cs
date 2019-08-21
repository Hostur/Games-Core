using System;
using System.Collections.Generic;
using Core.Attributes;
using Core.Collections;
using Core.InternalCommunication;
using Localization;

namespace UI
{
  [CoreRegister(true)]
  public class GameTranslator
  {
    private Dictionary<string, string> _dictionary;
    private ScriptableLanguageDictionary _languageDictionary;
    public GameTranslator()
    {
      _languageDictionary = UnityEngine.Resources.Load<ScriptableLanguageDictionary>("Balance/LanguageDictionary");
      ReloadDictionary();
      CoreGameEventsManager.Subscribe<LanguagesManager.AfterLanguageChangedEvent>(OnReloadLanguageEvent);
    }

    private void ReloadDictionary()
    {
      _dictionary = _languageDictionary.GetDictionary(LanguagesManager.CurrentLanguage);
      CoreGameEventsManager.Publish(this, new LanguageReloadedEvent());
    }

    public string Translate(string key)
    {
      return _dictionary.GetIfExists(key) ?? key;
    }

    public string Translate(string key, string postfix)
    {
      return Translate(key) + postfix;
    }

    private void OnReloadLanguageEvent(object sender, EventArgs arg)
    {
      ReloadDictionary();
    }

    public class LanguageReloadedEvent : CoreGameEvent {}
  }
}