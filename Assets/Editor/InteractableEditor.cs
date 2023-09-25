using System.Linq;
using UnityEditor;

[CustomEditor(typeof(Interactable), true), CanEditMultipleObjects]
public class InteractableEditor : Editor
{
    private static readonly string[] EventProperties = new[] { "OnPointerDown", "OnPointerDrag", "OnPointerUp" };

    private static string[] ExcludedProperties;

    private static bool ShowEvents;

    private void OnEnable()
    {
        ExcludedProperties = EventProperties.Concat(new[] { "m_Script" }).ToArray();
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        ShowEvents = EditorGUILayout.Foldout(ShowEvents, "Events");

        if (ShowEvents)
        {
            foreach (var property in EventProperties)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty(property));
            }
        }

        DrawPropertiesExcluding(serializedObject, ExcludedProperties.ToArray());
        serializedObject.ApplyModifiedProperties();
    }
}
