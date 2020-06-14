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
    [CustomPropertyDrawer(typeof(ResourceSelector))]
    public class ResourceSelectorEditor : PropertyDrawer
    {
        private ResourceSelectorDropdownWindow selectionWindow;
        private string propertyOwningWindow;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Rect labelRect = new Rect(position.x, position.y, EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(labelRect, label);

            if (ResourceContainer.instance == null)
            {
                Rect makeResourceButton = new Rect(position.x + EditorGUIUtility.labelWidth, position.y, position.width - EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight);
                if (GUI.Button(makeResourceButton, "Make Resources Asset"))
                {
                    string resourcesPath = Path.Combine(Application.dataPath, "Resources");
                    if (!Directory.Exists(resourcesPath))
                        Directory.CreateDirectory(resourcesPath);
                    AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<ResourceContainer>(), Path.Combine("Assets/Resources", "Resources.asset"));
                }
                return;
            }

            string currentEnumVal = property.FindPropertyRelative("selectionGUID").stringValue;

            //getting limiting type
            FieldInfo resourceSelectorInfo = property.serializedObject.targetObject.GetType().
                GetField(property.name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);

            ResourceSelector selector = (ResourceSelector)resourceSelectorInfo.GetValue(property.serializedObject.targetObject);

            FieldInfo limitingTypeInfo = typeof(ResourceSelector).GetField("limitingType", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
            System.Type limitingType = limitingTypeInfo.GetValue(selector) as System.Type;
            string filterType = limitingType == null?"": limitingType.FullName;

            // Getting asset
            ResourceAsset currentResourceAsset = ResourceContainer.GetByGUID(currentEnumVal);

            // Doing type Check
            if (currentResourceAsset != null && !Utils.CheckAgainstFilter(currentResourceAsset, "", filterType))
            {// then type limitation no longer makes sense
                property.FindPropertyRelative("selectionGUID").stringValue = "";// resetting selection
                currentResourceAsset = null;
            }

            string displayString = currentResourceAsset == null ? "" : currentResourceAsset.assetName;


            Rect dropdownButtonRect = new Rect(position.x + EditorGUIUtility.labelWidth, position.y, position.width - EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight);

            if(DrawAssetPreview(currentResourceAsset,position, property.FindPropertyRelative("selectionGUID").stringValue))
            {
                Rect editButton = new Rect(position.x + position.width - (2 * EditorGUIUtility.singleLineHeight) - 60,
                    position.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing,
                    60 - EditorGUIUtility.standardVerticalSpacing, EditorGUIUtility.singleLineHeight);
                if (GUI.Button(editButton, "Edit"))
                {
                    ResourceContainerEditor.GoToAsset(property.FindPropertyRelative("selectionGUID").stringValue);
                }

                dropdownButtonRect = new Rect(position.x + EditorGUIUtility.labelWidth + EditorGUIUtility.standardVerticalSpacing, position.y,
                    position.width - EditorGUIUtility.labelWidth - (2 * EditorGUIUtility.singleLineHeight) - (EditorGUIUtility.standardVerticalSpacing * 2f),
                    EditorGUIUtility.singleLineHeight);

            }
            else
            {
                Rect editButtonRect = new Rect(position.x + position.width - 60,
                    position.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing,
                    60, EditorGUIUtility.singleLineHeight);
                if (GUI.Button(editButtonRect, "Edit"))
                {
                    ResourceContainerEditor.GoToAsset(property.FindPropertyRelative("selectionGUID").stringValue);
                }
            }

            

            if (EditorGUI.DropdownButton(dropdownButtonRect, new GUIContent(displayString), FocusType.Keyboard))
            {
                propertyOwningWindow = property.propertyPath;
                selectionWindow?.Close();
                ResourceSelectorDropdownWindow.selection = "";
                selectionWindow = ResourceSelectorDropdownWindow.Display(property.serializedObject.targetObject, filterType, ResourceContainer.instance);
            }


            //closing selection Window
            if (selectionWindow != null && !string.IsNullOrEmpty(ResourceSelectorDropdownWindow.selection) && propertyOwningWindow == property.propertyPath)
            {
                property.FindPropertyRelative("selectionGUID").stringValue = ResourceSelectorDropdownWindow.selection;
                propertyOwningWindow = null;
                selectionWindow.Close();
                selectionWindow = null;
            }
        }

        /// <summary>
        /// returns true if drawing an image preview, false for text
        /// </summary>
        /// <param name="currentResourceAsset"></param>
        /// <param name="position"></param>
        /// <param name="selectionGuid"></param>
        /// <returns></returns>
        private bool DrawAssetPreview(ResourceAsset currentResourceAsset, Rect position, string selectionGuid)
        {
            // doing object reference preview
            if (currentResourceAsset != null && currentResourceAsset.objectReferenceValue != null &&
                (currentResourceAsset.objectReferenceValue as Object) != null &&
                currentResourceAsset.assetType == ResourceAsset.AssetType.ObjectReference)
            {
                
                Rect objectPreviewRect = new Rect(position.x + position.width - (2 * EditorGUIUtility.singleLineHeight), position.y,
                    2f * EditorGUIUtility.singleLineHeight, 2f * EditorGUIUtility.singleLineHeight);

                Object castAsset = currentResourceAsset.objectReferenceValue as Object;
                Texture2D previewTexture = AssetPreview.GetAssetPreview(castAsset);

                if (previewTexture == null)
                    return false;

                EditorGUI.DrawPreviewTexture(objectPreviewRect, previewTexture);
                return true;
                
            }
            else if (currentResourceAsset != null && currentResourceAsset.assetType != ResourceAsset.AssetType.ObjectReference)
            {
                Rect regularPreviewRect = new Rect(position.x + EditorGUIUtility.labelWidth + EditorGUIUtility.standardVerticalSpacing,
                    position.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing,
                    position.width - EditorGUIUtility.labelWidth - 60 - EditorGUIUtility.standardVerticalSpacing, EditorGUIUtility.singleLineHeight);
                EditorGUI.LabelField(regularPreviewRect, System.Convert.ToString(currentResourceAsset.GetValue()));
                
            }
            return false;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            string currentEnumVal = property.FindPropertyRelative("selectionGUID").stringValue;
            ResourceAsset currentResourceAsset = ResourceContainer.GetByGUID(currentEnumVal);
            if (currentResourceAsset != null)
                return (EditorGUIUtility.singleLineHeight * 2f) ;
            else return base.GetPropertyHeight(property, label);
        }

    }
}