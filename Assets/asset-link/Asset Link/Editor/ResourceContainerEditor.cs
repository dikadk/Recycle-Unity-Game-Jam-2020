//-------------------------------
//          Asset Link
// Copyright © 2020 ABXY Games
//-------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEditor.IMGUI.Controls;
namespace ABXY.AssetLink.Internal
{
    [CustomEditor(typeof(ResourceContainer))]
    public class ResourceContainerEditor : Editor
    {
        ResourceAssetNamespace root;
        SearchField searchField;
        string searchText = "";
        bool modified = false;


        // used for selector popup
        public bool isSelector = false;
        public System.Action<string> onSelect;
        public string currentSelection = "";
        public string typeFilter = "";

        private static string expandToAssetGuid = "";

        private void OnEnable()
        {
            if (this == null||serializedObject == null || serializedObject.targetObject == null)
                return;
            LoadTree(serializedObject.FindProperty("resources"));
            searchField = new SearchField();

            if (!string.IsNullOrEmpty(expandToAssetGuid))
            {
                root.ExpandTo(expandToAssetGuid);
                expandToAssetGuid = "";
            }
        }

        protected virtual string GetExpectedName()
        {
            return "Resources.asset";
        }

        public override void OnInspectorGUI()
        {
            if (this == null)
                return;
                        

            if (EditorApplication.isCompiling)
            {
                EditorGUILayout.HelpBox("Code is compiling, please wait", MessageType.Info, true);
                return;

            }

            if (string.IsNullOrEmpty(AssetDatabase.GetAssetPath(target)))
                return;

            string assetPath = AssetDatabase.GetAssetPath(target);
            if (Path.GetFileName(Path.GetDirectoryName(assetPath)) != "Resources")
            {
                EditorGUILayout.HelpBox("The parent directory of this asset must be named \"Resources\"", MessageType.Warning,true);
                return;
            }

            if (Path.GetFileName(Path.GetFileName(assetPath)) != GetExpectedName())
            {
                EditorGUILayout.HelpBox("This asset must be named " + GetExpectedName().Replace(".asset",""), MessageType.Warning, true);
                return;
            }

            if (!serializedObject.FindProperty("pathHasBeenInitialized").boolValue && !isSelector)
            {
                EditorGUILayout.HelpBox("Code generation path has not been set", MessageType.Warning, true);
                DrawSavePath("Set path");
                return;
            }

            EditorGUILayout.Space();

            Rect searchBarRect = EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight);
            searchText = searchField.OnGUI(searchBarRect, searchText);

            EditorGUILayout.Space();

            bool drawerReportsModified = false;

            if (isSelector)
            {
                drawerReportsModified = Drawers.DrawSelectionNamespaceTree(root, searchText, typeFilter, currentSelection, RealtimeOnly());
                currentSelection = Drawers.selectedGuid;
            }
            else
                drawerReportsModified = Drawers.DrawNamespaceTree(root, searchText, RealtimeOnly());

            bool duplicateNamesFound = Drawers.hasDuplicateNames;
            bool incorrectNamesFound = Drawers.hasInvalidNames;

            Drawers.FinishDrawingTree();

            if (drawerReportsModified)
                modified = true;

            if (root.assets.Count + root.subNamespaces.Count == 0 && !isSelector)
                EditorGUILayout.HelpBox("No assets have been added. Right click to add some", MessageType.Info, true);

            EditorGUILayout.Space();

            DrawSavePath("Change");

            EditorGUILayout.Space();
            
            // doing apply buttons
            EditorGUILayout.BeginHorizontal();

            GUILayout.FlexibleSpace();
            if (!isSelector )
            {
                EditorGUI.BeginDisabledGroup(!modified || duplicateNamesFound || incorrectNamesFound);
                if (GUILayout.Button("Reset"))
                {
                    LoadTree(serializedObject.FindProperty("resources"));
                    modified = false;
                }
                if (GUILayout.Button("Apply"))
                {
                    Apply();
                }

                EditorGUI.EndDisabledGroup();
            }
            else
            {
                EditorGUI.BeginDisabledGroup(string.IsNullOrEmpty(currentSelection));
                if (isSelector && GUILayout.Button("Select"))
                {
                    onSelect?.Invoke(currentSelection);
                }
                EditorGUI.EndDisabledGroup();
            }

            EditorGUILayout.EndHorizontal();
            
        }

        public static void GoToAsset(string guid)
        {
            expandToAssetGuid = guid;
            Selection.activeObject = ResourceContainer.instance;
        }

        private void DrawSavePath(string buttonLabel)
        {
            if (isSelector)
                return;
            serializedObject.Update();
            EditorGUILayout.BeginHorizontal();
            SerializedProperty savePath = serializedObject.FindProperty("savePath");
            EditorGUILayout.LabelField(savePath.stringValue);
            if (GUILayout.Button(buttonLabel))
            {
                string selectedPath = EditorUtility.OpenFolderPanel("Select save directory", Path.Combine(Application.dataPath, savePath.stringValue), "");
                System.Uri path1 = new System.Uri(selectedPath);
                System.Uri path2 = new System.Uri(Application.dataPath);
                System.Uri diff = path2.MakeRelativeUri(path1);
                savePath.stringValue = diff.OriginalString.Replace("%20"," ");
                serializedObject.FindProperty("pathHasBeenInitialized").boolValue = true;
                modified = true;
            }
            EditorGUILayout.EndHorizontal();
            serializedObject.ApplyModifiedProperties();


            
        }

        private void LoadTree(SerializedProperty list)
        {
            root = ResourceAssetNamespace.Make(GetRootNamespaceName());
            root.expanded = true;
            serializedObject.Update();
            for (int index = 0; index < list.arraySize; index++)
            {
                root.AddAsset(SP2Asset(list.GetArrayElementAtIndex(index)));
            }
        }

        protected virtual string GetRootNamespaceName()
        {
            return "R";
        }

        protected virtual bool RealtimeOnly()
        {
            return false;
        }

        private static ResourceAsset SP2Asset(SerializedProperty sp)
        {
            if (sp.objectReferenceValue == null)
                return null;
            ResourceAsset newAsset = (sp.objectReferenceValue as ResourceAsset).Copy();

            return newAsset;
        }

        private void Apply()
        {
            serializedObject.Update();
            SerializedProperty listProp = serializedObject.FindProperty("resources");

            List<ResourceAsset> newAssets = new List<ResourceAsset>();
            WalkTree(root, "", (string path, ResourceAsset asset) =>
            {
                if (asset != null)
                {
                    asset.path = path;
                    asset.name = asset.assetName;
                    newAssets.Add(asset);
                }
            });

            //updating pre-existing elements
            for (int index = 0; index < listProp.arraySize; index++)
            {
                ResourceAsset preExistingAsset = listProp.GetArrayElementAtIndex(index).objectReferenceValue as ResourceAsset;
                ResourceAsset newVersion = newAssets.Find(x => x.guid == preExistingAsset.guid);
                if (newVersion!= null){ // then I need to update
                    preExistingAsset.CopyFrom(newVersion);
                    preExistingAsset.name = newVersion.assetName;
                    newAssets.Remove(newVersion);
                }
                else // asset has been deleted, and I need to remove it
                {
                    Object objectToDelete = listProp.GetArrayElementAtIndex(index).objectReferenceValue;
                    listProp.DeleteArrayElementAtIndex(index);
                    if (objectToDelete != null)//hackety hack
                        listProp.DeleteArrayElementAtIndex(index);
                    DestroyImmediate(objectToDelete, true);
                }
            }

            foreach(ResourceAsset asset in newAssets)
            {
                AssetDatabase.AddObjectToAsset(asset, target);
                int newElementIndex = listProp.arraySize;
                listProp.InsertArrayElementAtIndex(newElementIndex);
                listProp.GetArrayElementAtIndex(newElementIndex).objectReferenceValue = asset;
            }


            serializedObject.ApplyModifiedProperties();
            serializedObject.Update();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            
            string rootPath = Path.Combine(Directory.GetParent(Application.dataPath).FullName, serializedObject.FindProperty("savePath").stringValue);
            Tree2Code.Write(root, Path.Combine(rootPath, GetRootNamespaceName()+ ".cs"));
            modified = false;
        }

        private void WalkTree(ResourceAssetNamespace ns, string path, System.Action<string, ResourceAsset> onProcessElement)
        {
            foreach (ResourceAsset asset in ns.assets)
            {
                onProcessElement?.Invoke(Path.Combine(path, ns.namespaceName), asset);
            }
            foreach (ResourceAssetNamespace subNamespace in ns.subNamespaces)
            {
                WalkTree(subNamespace, Path.Combine(path, ns.namespaceName), onProcessElement);
            }
        }


        private void OnDisable()
        {
            if (modified && !EditorApplication.isCompiling)// cannnot save while compiling, or things get forgotten
            {
                if (EditorUtility.DisplayDialog("Unapplied changes", "There are unapplied changes. Would you like to save them?", "Apply", "Revert"))
                    Apply();
            }
            else
                modified = false;

        }
    }

}