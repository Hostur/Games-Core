using System;
using Core;
using Core.Attributes;
using UnityEngine;
using UnityEngine.UI;
#pragma warning disable 649

namespace UI
{
  public class LabelTranslator : CoreBehaviour
  {
    [SerializeField] private Text _label;
    [SerializeField] private string _key;
    [SerializeField] private string _postfix;
    [SerializeField] private bool _upperCase;

    [CoreInject]
    private GameTranslator _gameTranslator;

    protected override void OnAwake()
    {
      base.OnAwake();
      Refresh();
    }

    private void Refresh()
    {
      if(!string.IsNullOrEmpty(_postfix))
        _label.text = _upperCase ? _gameTranslator.Translate(_key, _postfix).ToUpper() : _gameTranslator.Translate(_key);
      else
        _label.text = _upperCase ? _gameTranslator.Translate(_key).ToUpper() : _gameTranslator.Translate(_key);
    }

    [CoreRegisterEventHandler(typeof(GameTranslator.LanguageReloadedEvent))]
    private void OnLanguageReloadedEvent(object sender, EventArgs arg)
    {
      Refresh();
    }

    private void Reset() => _label = GetComponent<Text>();
  }
}
