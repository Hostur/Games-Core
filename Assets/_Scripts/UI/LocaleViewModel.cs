using MVVM;

namespace UI
{
  public class LocaleViewModel : ViewModelBase
  {
    private readonly GameTranslator _gameTranslator;

    public LocaleViewModel(GameTranslator gameTranslator)
    {
      _gameTranslator = gameTranslator;
    }

    protected virtual string Translate(string key)
    {
      return _gameTranslator.Translate(key);
    }
  }
}
