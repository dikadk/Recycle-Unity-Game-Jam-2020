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
    public class ResourceSelectorDropdownWindow : EditorWindow
    {

        ResourceContainerEditor editor;

        public static string selection = "";

        [SerializeField]
        Object objectThatOpenedMe;
        public static ResourceSelectorDropdownWindow Display(Object opener, string typeFilter, ResourceContainerBase resourceContainer)
        {
            ResourceSelectorDropdownWindow window = CreateInstance<ResourceSelectorDropdownWindow>();
            window.editor = Editor.CreateEditor(resourceContainer) as ResourceContainerEditor;
            window.editor.isSelector = true;
            window.editor.typeFilter = typeFilter;
            window.objectThatOpenedMe = opener;
            window.titleContent = new GUIContent("Asset Selection");
            window.editor.onSelect += (string newGuid)=> {
                selection = newGuid;
                EditorUtility.SetDirty(window.objectThatOpenedMe);
            };
            
            window.Show();
            return window;
        }

        private void OnGUI()
        {
            editor.OnInspectorGUI();
        }

        public void OnBeforeAssemblyReload()
        {
            Close();
        }
    }
}