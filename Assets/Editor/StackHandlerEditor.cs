using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(StackController))]
public class StackHandlerEditor : Editor
{
    private Vector2Int _GridSize;
    private Vector2Int _CellSize;
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GUILayout.Space(25);

        StackController gridSystemController = (StackController)target;
        if (Application.isPlaying)
        {
            if (GUILayout.Button("Add Stack"))
            {
                gridSystemController.AddEditorStack();
                EditorUtility.SetDirty(gridSystemController);
            }
            if (GUILayout.Button("Remove Stack"))
            {
                gridSystemController.RemoveEditorStack();
                EditorUtility.SetDirty(gridSystemController);
            }
            return;
        }
        else
        {
            if (GUILayout.Button("Place Stacks"))
            {
                gridSystemController.UpdateStacks();
                EditorUtility.SetDirty(gridSystemController);
            }
        }
            
    }
}
