using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(StackController))]
public class StackHandlerEditor : Editor
{
    private int _StackID;
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GUILayout.Space(25);

        GUILayout.Label("Stack ID", EditorStyles.boldLabel);
        _StackID = EditorGUILayout.IntField("Stack ID:", Mathf.Clamp(_StackID, 1, 4));

        StackController gridSystemController = (StackController)target;
        if (Application.isPlaying)
        {
            if (GUILayout.Button("Add Stack"))
            {
                gridSystemController.AddEditorStack(_StackID);
                EditorUtility.SetDirty(gridSystemController);
            }
            if (GUILayout.Button("Remove Stack"))
            {
                gridSystemController.RemoveEditorStack(_StackID);
                EditorUtility.SetDirty(gridSystemController);
            }
            return;
        }            
    }
}
