using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Malee.Editor;
[CustomEditor(typeof(ConveyorBikeList))]
public class ConveyorBikeListEditor : Editor
{
    ReorderableList bikeList;
    private void OnEnable()
    {
        bikeList = new ReorderableList(serializedObject.FindProperty("possBikes"));
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        bikeList.DoLayoutList();
        serializedObject.ApplyModifiedProperties();
    }
}
