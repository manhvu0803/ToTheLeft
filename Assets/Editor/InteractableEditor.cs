using System.Linq;
using UnityEditor;

[CustomEditor(typeof(Interactable), true), CanEditMultipleObjects]
public class InteractableEditor : Editor
{
    private static readonly string[] EventProperties = new[] { "OnDown", "OnObjectDragged", "OnUp" };

    private static string[] ExcludedProperties;

    private bool _showEvents;

    private void OnEnable()
    {
        ExcludedProperties = EventProperties.Concat(new[] { "m_Script" }).ToArray();
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        _showEvents = EditorGUILayout.Foldout(_showEvents, "Events");

        if (_showEvents)
        {
            foreach (var property in EventProperties)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty(property));
            }
        }

        DrawPropertiesExcluding(serializedObject, ExcludedProperties);
        serializedObject.ApplyModifiedProperties();
    }
}
