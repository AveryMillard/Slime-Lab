using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
public class ReadOnlyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        GUI.enabled = false; // Disable editing

        switch (property.propertyType)
        {
            case SerializedPropertyType.Vector2:
                EditorGUI.Vector2Field(position, label, property.vector2Value);
                break;
            default:
                EditorGUI.PropertyField(position, property, label);
                break;
        }

        GUI.enabled = true;  // Re-enable editing for other fields
    }
}
#endif