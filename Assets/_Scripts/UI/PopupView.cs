using Development.Logging;
using MVVM;
using UnityEngine;
using UnityEngine.UI;

// ReSharper disable InconsistentNaming
#pragma warning disable 649

namespace UI
{
  /// <summary>
  /// Popup can be displayed by publishing <see cref="ShowPopupEvent"/>.
  /// </summary>
  public class PopupView : View<PopupViewModel>
  {
    protected override void OnAwake()
    {
      base.OnAwake();
      DontDestroyOnLoad(gameObject);
      gameObject.SetActive(false);
    }

    [SerializeField]
    [ButtonBind("Positive")]
    private Button _positiveButton;

    [SerializeField]
    [ButtonBind("Negative")]
    private Button _negativeButton;

    [SerializeField]
    [ButtonBind("Cancel")]
    private Button _cancelButton;

    [SerializeField]
    [ButtonBind("Cancel", "CanBackgroundClick")]
    private Button _backgroundCancelButton;

    [SerializeField]
    [ToggleBind("_neverShowAgain")]
    private Toggle _neverShowAgain;

    [PropertyBind]
    private string PositiveButton
    {
      get => string.Empty;
      set => _positiveButton.SetText(value);
    }

    [PropertyBind]
    private string NegativeButton
    {
      get => string.Empty;
      set => _negativeButton.SetText(value);
    }

    [PropertyBind]
    private string CancelButton
    {
      get => string.Empty;
      set => _cancelButton.SetText(value);
    }

    [PropertyBind]
    private bool ShowTitle
    {
      get => false;
      set => _tittlePanel.gameObject.SetActive(value);
    }

    [PropertyBind]
    private bool _showNeverShowAgainOption
    {
      get => false;
      set => _neverShowAgain.gameObject.SetActive(value);
    }

    [SerializeField]
    [TextBind]
    private Text Title;

    [SerializeField]
    [TextBind]
    private Text Content;

    [SerializeField]
    private Transform _tittlePanel;

    [SerializeField]
    private RectTransform _popupPanelRectTransform;

    [PropertyBind]
    private PopupOptions ButtonOptions
    {
      get => PopupOptions.Positive; // Not used.
      set => RefreshButtons(value);
    }

    [PropertyBind]
    private PopupSize SizeOptions
    {
      get => PopupSize.W800H600; // Not used.
      set => RefreshSize(value);
    }

    [PropertyBind]
    private Color32[] _buttonsColors
    {
      get => null;
      set => SetButtonColors(value);
    }

    private void RefreshSize(PopupSize popupSize)
    {
      switch (popupSize)
      {
        case PopupSize.W800H600:
          _popupPanelRectTransform.sizeDelta = new Vector2(800, 600);
          break;
        case PopupSize.W400H400:
          _popupPanelRectTransform.sizeDelta = new Vector2(400, 400);
          break;
        case PopupSize.W600H400:
          _popupPanelRectTransform.sizeDelta = new Vector2(600, 400);
          break;
        case PopupSize.W800H400:
          _popupPanelRectTransform.sizeDelta = new Vector2(800, 400);
          break;
      }
    }

    private void SetButtonColors(Color32[] colors)
    {
      _positiveButton.SetColor(colors[0]);
      _negativeButton.SetColor(colors[1]);
      _cancelButton.SetColor(colors[2]);
    }

    /// <summary>
    /// Shows and hides buttons depends on the popup options that comes from published <see cref="ShowPopupEvent"/>.
    /// </summary>
    /// <param name="options">Options binded from view model.</param>
    private void RefreshButtons(PopupOptions options)
    {
      this.Log($"Refreshing popup buttons with options '{options.ToString()}'.", LogLevel.DevelopmentInfo);
      switch (options)
      {
        case PopupOptions.Positive:
        {
          _positiveButton.gameObject.SetActive(true);
          _negativeButton.gameObject.SetActive(false);
          _cancelButton.gameObject.SetActive(false);
          break;
        }
        case PopupOptions.PositiveCancel:
        {
          _positiveButton.gameObject.SetActive(true);
          _cancelButton.gameObject.SetActive(true);
          _negativeButton.gameObject.SetActive(false);   
          break;
        }
        case PopupOptions.PositiveNegative:
        {
          _positiveButton.gameObject.SetActive(true);
          _negativeButton.gameObject.SetActive(true);
          _cancelButton.gameObject.SetActive(false);  
          break;
        }
        case PopupOptions.PositiveNegativeCancel:
        {
          _positiveButton.gameObject.SetActive(true);
          _negativeButton.gameObject.SetActive(true);
          _cancelButton.gameObject.SetActive(true);
          break;
        }
      }
    }
  }
}
