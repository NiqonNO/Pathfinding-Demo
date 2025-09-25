using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GridController))]
public class GridControllerEditor : Editor
{
    private SerializedProperty GridSettings;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        GridSettings = serializedObject.FindProperty("GridSettings");
        Object DataAsset = GridSettings.objectReferenceValue;

        if (DataAsset == null) return;
        if (!GUILayout.Button("Edit Grid")) return;
        
        Selection.activeObject = DataAsset;
        EditorGUIUtility.PingObject(DataAsset);
    }
}