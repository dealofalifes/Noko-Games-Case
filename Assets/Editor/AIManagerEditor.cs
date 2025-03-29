using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AIManagerEditor))]
public class AIManagerEditor : Editor
{
    private List<MachineController> _JobList;
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        //GUILayout.Space(25);

        //GUILayout.Label("Set the job link for NPC", EditorStyles.boldLabel);

        //if (_JobList == null)
        //    _JobList = new List<MachineController>();

        //for (int i = 0; i < _JobList.Count; i++)
        //{
        //    _JobList[i] = (MachineController)EditorGUILayout.ObjectField($"Job {i}", _JobList[i], typeof(MachineController), true);
        //}

        //if (GUILayout.Button("Add Job"))
        //{
        //    _JobList.Add(null);
        //}

        //if (_JobList.Count > 0 && GUILayout.Button("Remove Last Job"))
        //{
        //    _JobList.RemoveAt(_JobList.Count - 1);
        //}

        //AIManager aiManager = (AIManager)target;
        //if (GUILayout.Button("Create NPC With These Jobs"))
        //{
            
        //}

        //EditorUtility.SetDirty(aiManager); // Mark as changed
    }
}
