using UnityEngine;
using UnityEngine.UI;

namespace MVVM.Controls
{
  [RequireComponent(typeof(Image))]
  public class ToggleButton : Button
  {
    #region CustomEditor
    public Sprite PositiveSprite;
    public Sprite NegativeSprite;
    /// <summary>
    /// Value indicating whether initial value of this toggle should be positive.
    /// </summary>
    public bool DefaultPositive = true;  
    /// <summary>
    /// Value indicating whether transitions between positive and negative sprite should be animated.
    /// </summary>
    public bool AnimatedTransitions;
    public bool PlaySoundOnClick = true;
    #endregion

    private bool _positive;
    protected Image ToggleImage;

    protected override void Awake()
    {
      if (!Application.isPlaying) return;
      base.Awake();
      ToggleImage = GetComponent<Image>();
      _positive = DefaultPositive;
      onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
      // ReSharper disable once AssignmentInConditionalExpression
      if (_positive = !_positive)
        SetPositive();
      else
        SetNegative();
    }

    protected override void OnEnable()
    {
      if (!Application.isPlaying) return;
      if (PositiveSprite != null && NegativeSprite != null)
        AfterEnabled(PositiveSprite, NegativeSprite);

      // If sprites doesn't fit _positive flag.
      if(_positive && ToggleImage.sprite != PositiveSprite)
        SetPositive();
      else if(!_positive && ToggleImage.sprite != NegativeSprite)
        SetNegative();
    }

    protected virtual void AfterEnabled(Sprite positive, Sprite negative)
    {

    }

    protected void SetValue(bool value)
    {
      if(value)
        SetPositive();
      else
        SetNegative();
    }

    private void SetPositive()
    {
      _positive = true;
      SetPositive(PositiveSprite, AnimatedTransitions);
    }

    private void SetNegative()
    {
      _positive = false;
      SetNegative(NegativeSprite, AnimatedTransitions);
    }

    protected virtual void SetPositive(Sprite positiveSprite, bool animatedTransition)
    {
      
    }

    protected virtual void SetNegative(Sprite negativeSprite, bool animatedTransition)
    {
      
    }
  }
}
