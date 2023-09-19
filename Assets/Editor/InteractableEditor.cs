using UnityEditor;

[CustomEditor(typeof(Interactable), true)]
public class InteractableEditor : Editor
{
    private static readonly string[] EventProperties = new[] { "OnPointerDown", "OnPointerDrag", "OnPointerUp" };

    private static bool ShowEvents;

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

        DrawPropertiesExcluding(serializedObject, EventProperties);
        serializedObject.ApplyModifiedProperties();
    }
}
