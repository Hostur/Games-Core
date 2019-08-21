using Core.Attributes;
using Core.InternalCommunication;
using MVVM;
using Resources;
using Resources.ResourcesManagement;
using UI;
using UI.Scenes;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;
using Image = UnityEngine.UI.Image;

// ReSharper disable UnusedMember.Local
// ReSharper disable InconsistentNaming
#pragma warning disable 649

namespace Examples
{
  public class MainMenuView : View<MainMenuViewModel>
  {
    [SerializeField] [TextBind] private Text _translatedTextExample;

    [SerializeField] [InputFieldBind("TextFieldExample")]
    private InputField _inputTextExample;

    [SerializeField] [ButtonBind("OnLoadGameSceneButtonClick", "CanClickLoadGameSceneButton")]
    private Button _loadGameSceneButton;

    [SerializeField]
    [LazyImage("example", "example")]
    private Image _lazyBackground;
  }

  [CoreRegister]
  public class MainMenuViewModel : LocaleViewModel
  {
    private string _translatedTextExample => Translate("ENTER_3_CHARACTERS");
    private string _textFieldExampleBackField;

    public MainMenuViewModel(GameTranslator gameTranslator) : base(gameTranslator) {}

    private string TextFieldExample
    {
      get => _textFieldExampleBackField;
      set { _textFieldExampleBackField = value; Refresh(); }
    }

    public override void Refresh()
    {
      base.Refresh();
      FireOnPropertyChanged(() => CanClickLoadGameSceneButton);
    }

    private bool CanClickLoadGameSceneButton => TextFieldExample?.Length == 3;
    private void OnLoadGameSceneButtonClick() => CoreGameEventsManager.Publish(this, new SceneChangeEvent(SceneName.Game));
  }
}