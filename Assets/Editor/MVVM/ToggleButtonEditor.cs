using MVVM.Controls;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;

namespace Editor.MVVM
{
  [CustomEditor(typeof(ToggleButton), true)]
  public class ToggleButtonEditor : ButtonEditor
  {
    private bool _showCustom = true;
    private ToggleButton _toggleButton;
    protected override void OnEnable()
    {
      base.OnEnable();
      _toggleButton = (ToggleButton) target;
    }

    public override void OnInspectorGUI()
    {
      _showCustom = EditorGUILayout.Foldout(_showCustom, "Toggle button");

      if (_showCustom)
      {
        EditorGUI.indentLevel++;
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Default positive: ");
        _toggleButton.DefaultPositive = EditorGUILayout.Toggle(_toggleButton.DefaultPositive);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Animated transition: ");
        _toggleButton.AnimatedTransitions = EditorGUILayout.Toggle(_toggleButton.AnimatedTransitions);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Sound on click: ");
        _toggleButton.PlaySoundOnClick = EditorGUILayout.Toggle(_toggleButton.PlaySoundOnClick);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Positive sprite: ");
        _toggleButton.PositiveSprite =
          (Sprite) EditorGUILayout.ObjectField(_toggleButton.PositiveSprite, typeof(Sprite), true);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Negative sprite: ");
        _toggleButton.NegativeSprite =
          (Sprite) EditorGUILayout.ObjectField(_toggleButton.NegativeSprite, typeof(Sprite), true);
        EditorGUILayout.EndHorizontal();
        EditorGUI.indentLevel--;
      }

      base.OnInspectorGUI();
    }
  }
}
