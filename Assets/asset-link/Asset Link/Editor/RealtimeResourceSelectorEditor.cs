//-------------------------------
//          Asset Link
// Copyright © 2020 ABXY Games
//-------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.IO;

namespace ABXY.AssetLink.Internal
{
    [CustomPropertyDrawer(typeof(SceneAssetSelector))]
    public class RealtimeResourceSelectorEditor : PropertyDrawer
    {
        private ResourceSelectorDropdownWindow selectionWindow;
        private string propertyOwningWindow;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Rect labelRect = new Rect(position.x, position.y, EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(labelRect, label);

            if (RealtimeResourceContainer.instance == null)
            {
                Rect makeResourceButton = new Rect(position.x + EditorGUIUtility.labelWidth, position.y, position.width - EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight);
                if (GUI.Button(makeResourceButton, "Make Scene Resources asset"))
                {
                    string resourcesPath = Path.Combine(Application.dataPath, "Resources");
                    if (!Directory.Exists(resourcesPath))
                        Directory.CreateDirectory(resourcesPath);
                    AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<RealtimeResourceContainer>(), Path.Combine("Assets/Resources", "Scene Resources.asset"));
                }
                return;
            }

            string currentEnumVal = property.FindPropertyRelative("selectionGUID").stringValue;

            //getting limiting type


            // Getting asset
            ResourceAsset currentResourceAsset = RealtimeResourceContainer.GetByGUID(currentEnumVal);



            string displayString = currentResourceAsset == null ? "" : currentResourceAsset.assetName;


            Rect dropdownButtonRect = new Rect(position.x + EditorGUIUtility.labelWidth, position.y, position.width - EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight);
            if (EditorGUI.DropdownButton(dropdownButtonRect, new GUIContent(displayString), FocusType.Keyboard))
            {
                propertyOwningWindow = property.propertyPath;
                selectionWindow?.Close();
                ResourceSelectorDropdownWindow.selection = "";
                selectionWindow = ResourceSelectorDropdownWindow.Display(property.serializedObject.targetObject, "", RealtimeResourceContainer.instance);
            }

            if (selectionWindow != null && !string.IsNullOrEmpty(ResourceSelectorDropdownWindow.selection) && propertyOwningWindow == property.propertyPath)
            {
                property.FindPropertyRelative("selectionGUID").stringValue = ResourceSelectorDropdownWindow.selection;
                propertyOwningWindow = null;
                selectionWindow.Close();
                selectionWindow = null;
            }
        }
    }
}