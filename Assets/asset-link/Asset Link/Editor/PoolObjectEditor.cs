//-------------------------------
//          Asset Link
// Copyright © 2020 ABXY Games
//-------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ABXY.AssetLink.Internal
{
    [CustomEditor(typeof(PoolObject))]
    public class PoolObjectEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (Application.isPlaying && GUILayout.Button("Return to pool"))
            {
                (target as PoolObject).ReturnToPool();
            }
        }
    }
}