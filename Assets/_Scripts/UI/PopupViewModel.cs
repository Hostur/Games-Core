using System;
using Core.Attributes;
using Core.InternalCommunication;
using Development.Logging;
using UnityEngine;

namespace UI
{
  [CoreRegister]
  public class PopupViewModel : LocaleViewModel
  {
#region Buttons
    private void Positive()
    {
      Finalize(PopupResultType.Positive);
    }

    private void Negative()
    {
      Finalize(PopupResultType.Negative);
    }

    private void Cancel()
    {
      Finalize(PopupResultType.Cancel);
    }

    internal void HardwareCancel()
    {
      if(CanBackgroundClick)
        Cancel();
    }

    private void OnHardwareBackButtonEvent(object sender, EventArgs arg)
    {
      HardwareBackButtonEvent e = arg as HardwareBackButtonEvent;
      e.Consumed = true;
      HardwareCancel();
    }

    private bool CanBackgroundClick => _cancelOnBackgroundAvailable;

    private void Finalize(PopupResultType resultType)
    {
      this.Log($"Finalize popup with resultType '{resultType.ToString()}'.", LogLevel.DevelopmentInfo);
      _resultCallback?.Invoke(new PopupResult(resultType, _customPopupParam, _neverShowAgain));
      CoreGameEventsManager.Unsubscribe<HardwareBackButtonEvent>(OnHardwareBackButtonEvent);
      OnEnableDisable?.Invoke(false);
    }
#endregion

#region Properties
    private string Title { get; set; }

    private string Content { get; set; }

    private string PositiveButton { get; set; }

    private string NegativeButton { get; set; }

    private string CancelButton { get; set; }
 
    private PopupOptions ButtonOptions { get; set; }

    private PopupSize SizeOptions { get; set; }

    private bool ShowTitle { get; set; }

    private bool _cancelOnBackgroundAvailable { get; set; }

    private string _customPopupParam { get; set; }

    private Color32[] _buttonsColors { get; set; }

    private bool _neverShowAgain { get; set; }

    private bool _showNeverShowAgainOption { get; set; }
    #endregion

    /// <summary>
    /// Action reference holder.
    /// </summary>
    private Action<PopupResult> _resultCallback;
    private readonly Color32[] _defaultColors = new Color32[3];

    public override void Refresh()
    {
      base.Refresh();
      FireOnPropertyChanged(() => Title);
      FireOnPropertyChanged(() => Content);
      FireOnPropertyChanged(() => ButtonOptions);
      FireOnPropertyChanged(() => PositiveButton);
      FireOnPropertyChanged(() => NegativeButton);
      FireOnPropertyChanged(() => CancelButton);
      FireOnPropertyChanged(() => SizeOptions);
      FireOnPropertyChanged(() => CanBackgroundClick);
      FireOnPropertyChanged(() => _buttonsColors);
      FireOnPropertyChanged(() => _showNeverShowAgainOption);
    }

    [CoreRegisterEventHandler(typeof(ShowPopupEvent))]
    private void OnShowPopupEvent(object sender, EventArgs arg)
    {
      this.Log("Show popup requested.", LogLevel.DevelopmentInfo);
      ShowPopup(arg as ShowPopupEvent);
    }

    private void ShowPopup(ShowPopupEvent showPopupEvent)
    {
      if (string.IsNullOrEmpty(showPopupEvent.PopupTitleKey))
      {
        Title = "Title";
        ShowTitle = false;
      }
      else
      {
        ShowTitle = true;
        Title = Translate(showPopupEvent.PopupTitleKey);
      }

      Content = Translate(showPopupEvent.PopupContentKey);

      SizeOptions = showPopupEvent.Size;
      PositiveButton = (!string.IsNullOrEmpty(showPopupEvent.PositiveButtonKey)) ? Translate(showPopupEvent.PositiveButtonKey) : Translate("OK");
      NegativeButton = (!string.IsNullOrEmpty(showPopupEvent.NegativeButtonKey)) ? Translate(showPopupEvent.NegativeButtonKey) : Translate("NO");
      CancelButton = (!string.IsNullOrEmpty(showPopupEvent.CancelButtonKey)) ? Translate(showPopupEvent.CancelButtonKey) : Translate("CANCEL");

      ButtonOptions = showPopupEvent.ButtonOptions;
      _resultCallback = showPopupEvent.ResultCallback;

      _cancelOnBackgroundAvailable = showPopupEvent.CancelOnBackgroundAvailable;
      _customPopupParam = showPopupEvent.CustomParam;

      if (showPopupEvent.AllButtonsColor == new Color())
        _buttonsColors = _defaultColors;
      else
        _buttonsColors = new[] { showPopupEvent.AllButtonsColor, showPopupEvent.AllButtonsColor, showPopupEvent.AllButtonsColor };

      _showNeverShowAgainOption = showPopupEvent.ShowNeverShowAgainCheckbox;

      CoreGameEventsManager.Subscribe<HardwareBackButtonEvent>(OnHardwareBackButtonEvent);
      // Enabling popup cause Refreshing on view model so FireOnPropertyChanged will be invoked from Refresh function.
      OnEnableDisable?.Invoke(true);
    }

    public PopupViewModel(GameTranslator gameTranslator) : base(gameTranslator)
    {
      try
      {
        _defaultColors[0] = GraphicHelper.DefaultPositiveButtonColor;
        _defaultColors[1] = GraphicHelper.DefaultNegativeButtonColor;
        _defaultColors[2] = GraphicHelper.DefaultCancelButtonColor;
      }
      catch (Exception e)
      {
        this.Log($"Exception occur during constructing PopupViewModel. \n{e}", LogLevel.Error);
      }
    }
  }

  public enum PopupSize
  {
    W800H600,
    W800H400,
    W600H400,
    W400H400
  }

  public class ShowPopupEvent : CoreGameEvent
  {
    public PopupOptions ButtonOptions { get; }
    public string PopupTitleKey { get; }
    public string PopupContentKey { get; }
    public Action<PopupResult> ResultCallback { get; }

    public string PositiveButtonKey { get; } = null;
    public string NegativeButtonKey { get; } = null;
    public string CancelButtonKey { get; } = null;

    public bool CancelOnBackgroundAvailable { get; }
    public PopupSize Size { get; }
    public string CustomParam { get; set; } = null;

    public Color32 AllButtonsColor;
    public bool ShowNeverShowAgainCheckbox;

    public ShowPopupEvent(PopupOptions buttonOptions, string popupTitleKey, string popupContentKey, Action<PopupResult> onResultCallback, bool cancelOnBackgroundClick = false, PopupSize size = PopupSize.W800H600)
    {
      ButtonOptions = buttonOptions;
      PopupTitleKey = popupTitleKey;
      PopupContentKey = popupContentKey;
      ResultCallback = onResultCallback;
      Size = size;
      CancelOnBackgroundAvailable = cancelOnBackgroundClick;
    }

    public ShowPopupEvent(PopupOptions buttonOptions,
      string popupTitleKey,
      string popupContentKey,
      string positiveButtonKey,
      Action<PopupResult> onResultCallback,
      bool cancelOnBackgroundClick = false,
      PopupSize size = PopupSize.W800H600)
    {
      ButtonOptions = buttonOptions;
      PopupTitleKey = popupTitleKey;
      PopupContentKey = popupContentKey;
      ResultCallback = onResultCallback;
      PositiveButtonKey = positiveButtonKey;
      Size = size;
      CancelOnBackgroundAvailable = cancelOnBackgroundClick;
    }

    public ShowPopupEvent(PopupOptions buttonOptions,
      string popupTitleKey,
      string popupContentKey,
      string positiveButtonKey,
      string negativeButtonKey,
      Action<PopupResult> onResultCallback,
      bool cancelOnBackgroundClick = false,
      PopupSize size = PopupSize.W800H600)
    {
      ButtonOptions = buttonOptions;
      PopupTitleKey = popupTitleKey;
      PopupContentKey = popupContentKey;
      ResultCallback = onResultCallback;
      PositiveButtonKey = positiveButtonKey;
      NegativeButtonKey = negativeButtonKey;
      Size = size;
      CancelOnBackgroundAvailable = cancelOnBackgroundClick;
    }

    public ShowPopupEvent(
      PopupOptions buttonOptions,
      string popupTitleKey,
      string popupContentKey,
      string positiveButtonKey = null,
      string negativeButtonKey = null,
      string cancelButtonKey = null,
      bool cancelOnBackgroundClick = false,
      Action<PopupResult> onResultCallback = null,
      PopupSize size = PopupSize.W800H600)
    {
      ButtonOptions = buttonOptions;
      PopupTitleKey = popupTitleKey;
      PopupContentKey = popupContentKey;
      ResultCallback = onResultCallback;
      PositiveButtonKey = positiveButtonKey;
      NegativeButtonKey = negativeButtonKey;
      CancelButtonKey = cancelButtonKey;
      Size = size;
      CancelOnBackgroundAvailable = cancelOnBackgroundClick;
    }
  }

  public class PopupResult
  {
    public PopupResultType ResultType { get; }
    public string CustomParam { get; }
    public bool NeverShowAgain { get; }

    public PopupResult(PopupResultType resultType, string customParam, bool neverShowAgain = false)
    {
      ResultType = resultType;
      CustomParam = customParam;
      NeverShowAgain = neverShowAgain;
    }

    public static implicit operator PopupResultType(PopupResult result)
    {
      return result.ResultType;
    }

    public static implicit operator PopupResult(PopupResultType type)
    {
      return new PopupResult(type, null);
    }
  }

  public enum PopupResultType
  {
    Positive,
    Negative,
    Cancel
  }

  public enum PopupOptions
  {
    Positive,
    PositiveNegative,
    PositiveNegativeCancel,
    PositiveCancel
  }
}