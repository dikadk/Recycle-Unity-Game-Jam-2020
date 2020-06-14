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
    public class NamespaceEditor : EditorWindow
    {
        ResourceAssetNamespace editingNamespace;
        string currentName = "";

        System.Action<ResourceAssetNamespace> onEditApplied;

        public static void Show(ResourceAssetNamespace ns, System.Action<ResourceAssetNamespace> onEditApplied)
        {
            NamespaceEditor editor = CreateInstance<NamespaceEditor>();
            editor.maxSize = new Vector2(300, (EditorGUIUtility.singleLineHeight * 2f) + (EditorGUIUtility.standardVerticalSpacing * 4f));
            editor.minSize = new Vector2(300, (EditorGUIUtility.singleLineHeight * 2f) + (EditorGUIUtility.standardVerticalSpacing * 4f));
            editor.editingNamespace = ns;
            editor.currentName = ns.namespaceName;
            editor.onEditApplied = onEditApplied;
            editor.ShowAuxWindow();
        }

        private void OnGUI()
        {
            currentName = EditorGUILayout.TextField("Namespace name", currentName);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Cancel"))
            {
                this.Close();
            }

            if (GUILayout.Button("Apply"))
            {
                editingNamespace.namespaceName = currentName;
                editingNamespace.displayName = currentName;
                onEditApplied?.Invoke(editingNamespace);
                this.Close();
            }

            EditorGUILayout.EndHorizontal();


        }


    }
}