using System;
using Core;
using Core.Attributes;
using Core.InternalCommunication;
using UnityEngine;

namespace Localization
{
  public enum GameLanguage
  {
    PL,
    ENG
  }

  public class LanguagesManager : CoreBehaviour
  {
    private const string KEY_GAME_LANGUAGE = "keyGameLanguage";
    public static GameLanguage CurrentLanguage => (GameLanguage)PlayerPrefs.GetInt(KEY_GAME_LANGUAGE,
      (int)(Application.systemLanguage == SystemLanguage.Polish ? GameLanguage.PL : GameLanguage.ENG));
    public static string CurrentLanguageShortString => CurrentLanguage == GameLanguage.PL ? "pl" : "en";


    [CoreRegisterEventHandler(typeof(SetGameLanguageEvent))]
    private void OnSetGameLanguageEvent(object sender, EventArgs arg)
    {
      SetGameLanguageEvent e = arg as SetGameLanguageEvent;
      PlayerPrefs.SetInt(KEY_GAME_LANGUAGE, (int)e.Language);
      PlayerPrefs.Save();
      Publish(new AfterLanguageChangedEvent(e.Language));
    }

    public class SetGameLanguageEvent : CoreGameEvent
    {
      public GameLanguage Language { get; }
      public SetGameLanguageEvent(GameLanguage language)
      {
        Language = language;
      } 
    }

    public class AfterLanguageChangedEvent : CoreGameEvent
    {
      public GameLanguage Language { get; }
      public AfterLanguageChangedEvent(GameLanguage language)
      {
        Language = language;
      }
    }
  }
}
