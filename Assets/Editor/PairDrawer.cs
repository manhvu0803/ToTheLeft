using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(FreeSlotLevelController.Pair))]
public class PairDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var slotRect = new Rect(position.x, position.y, position.width / 2 - 10, position.height);
        EditorGUIUtility.labelWidth = 30;
        EditorGUI.PropertyField(slotRect, property.FindPropertyRelative("Slot"));
        EditorGUIUtility.labelWidth = 40;
        var targetRect = new Rect(position.x + position.width / 2, position.y, position.width / 2, position.height);
        EditorGUI.PropertyField(targetRect, property.FindPropertyRelative("Target"));
        EditorGUIUtility.labelWidth = 0;
    }
}
