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
    public class Drawers
    {
        private static readonly Color32 highlightColor = new Color32(61, 128, 223, 255);

        private static readonly Color32 resourceColor = new Color32(143, 143, 143, 255);

        private static readonly Color32 headerTextColor = new Color32(244, 244, 244, 255);

        private static readonly Color32 darkTrashColor = new Color32(120, 120, 120, 255);

        public static bool hasDuplicateNames { get; private set; }
        public static bool hasInvalidNames { get; private set; }

        public static bool DrawNamespaceTree(ResourceAssetNamespace root, string filter, bool realtimeOnly)
        {
            return DrawNamespaceTree(root, null, filter, null, false, realtimeOnly);
        }

        public static bool DrawSelectionNamespaceTree(ResourceAssetNamespace root, string filter, string typeFilter, string lastSelection, bool realtimeOnly)
        {
            selectedGuid = lastSelection;
            return DrawNamespaceTree(root, null, filter, typeFilter, true, realtimeOnly);
        }

        public static string selectedGuid { get; private set; }

        private static Texture2D _trashIcon = null;
        private static Texture2D trashIcon
        {
            get
            {
                if (_trashIcon == null)
                    _trashIcon = (Texture2D)Resources.Load("AL-Trash");
                return _trashIcon;
            }
        }

        private static Texture2D _addEnumIcon = null;
        private static Texture2D addEnumIcon
        {
            get
            {
                if (_addEnumIcon == null)
                    _addEnumIcon = (Texture2D)Resources.Load("AL-AddIcon");
                return _addEnumIcon;
            }
        }

        private static bool DrawDeleteButton(Rect position, Color color)
        {
            GUIStyle button = new GUIStyle(GUI.skin.button);
            button.padding = new RectOffset(1, 1, 1, 1);
            button.onNormal.background = null;
            button.normal.background = null;
            button.focused.background = null;
            button.active.background = null;
            button.hover.background = null;
            button.onFocused.background = null;
            button.onActive.background = null;
            button.onHover.background = null;
            Color currentColor = GUI.color;
            GUI.color = color;
            bool result = GUI.Button(position, new GUIContent(trashIcon), button);
            GUI.color = currentColor;
            return result;
        }

        private static bool DrawAddEnumButton(Rect position, Color color)
        {
            GUIStyle button = new GUIStyle(GUI.skin.button);
            button.padding = new RectOffset(1, 1, 1, 1);
            button.onNormal.background = null;
            button.normal.background = null;
            button.focused.background = null;
            button.active.background = null;
            button.hover.background = null;
            button.onFocused.background = null;
            button.onActive.background = null;
            button.onHover.background = null;
            Color currentColor = GUI.color;
            GUI.color = color;
            bool result = GUI.Button(position, new GUIContent(addEnumIcon), button);
            GUI.color = currentColor;
            return result;
        }


        private static bool DrawNamespaceTree(ResourceAssetNamespace root, ResourceAssetNamespace parent, string filter, string filterType, bool isSelectionMode, bool realtimeOnly)
        {

            bool modified = false;
            Rect titleRect = EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight);

            EditorGUI.DrawRect(EditorGUI.IndentedRect(titleRect), GUI.color);



            EditorGUI.BeginChangeCheck();
            //Right click and drag and drop handling
            if (!isSelectionMode)
            {
                switch (Event.current.type)
                {
                    case EventType.ContextClick:
                        // Doing right click menu
                        if (titleRect.Contains(Event.current.mousePosition))
                        {
                            GenericMenu menu = new GenericMenu();
                            menu.AddItem(new GUIContent("Add resource"), false, (object data) =>
                            {
                                OnAddAsset(data, realtimeOnly);
                                modified = true;
                            }, root);
                            menu.AddItem(new GUIContent("Add namespace"), false, (object data) =>
                            {
                                OnAddNamespace(data);
                                modified = true;
                            }, root);
                            if (parent != null)// meaning, this isn't the root node
                            {
                                menu.AddSeparator("");
                                menu.AddItem(new GUIContent("Rename"), false, (object data) =>
                                {
                                    modified = true;
                                    NamespaceEditor.Show(root, null);
                                }, root);
                                menu.AddSeparator("");
                                menu.AddItem(new GUIContent("Delete"), false, () =>
                                {
                                    modified = true;
                                    parent.RemoveNamespace(root.namespaceName);
                                });
                            }
                            menu.ShowAsContext();
                            modified = true;
                            Event.current.Use();
                        }
                        break;
                    case EventType.Repaint:
                        DragAndDropData dropData = DragAndDrop.GetGenericData("DraggedResourceAsset") as DragAndDropData;
                        if (titleRect.Contains(Event.current.mousePosition) && dropData != null && dropData.draggedNamespace != root)
                        {
                            DragAndDrop.visualMode = DragAndDropVisualMode.Move;
                            EditorGUI.DrawRect(EditorGUI.IndentedRect(titleRect), highlightColor);
                        }
                        break;
                    case EventType.DragExited:
                        DragAndDropData dragData = DragAndDrop.GetGenericData("DraggedResourceAsset") as DragAndDropData;
                        if (dragData != null && titleRect.Contains(Event.current.mousePosition) && dragData.draggedNamespace != root)
                        {
                            DragAndDrop.SetGenericData("DraggedResourceAsset", null);
                            DragAndDrop.AcceptDrag();
                            modified = true;
                            if (dragData.draggedAsset != null)
                            {
                                dragData.sourceNamespace.RemoveAsset(dragData.draggedAsset.assetName);
                                dragData.draggedAsset.path = "";
                                root.assets.Add(dragData.draggedAsset);
                            }
                            else if (dragData.draggedNamespace != null)
                            {
                                dragData.sourceNamespace.RemoveNamespace(dragData.draggedNamespace.namespaceName);
                                dragData.draggedNamespace.path = "";
                                root.subNamespaces.Add(dragData.draggedNamespace);
                            }
                        }
                        break;
                    case EventType.MouseDown:
                        if (!titleRect.Contains(Event.current.mousePosition) || parent == null)
                            break;
                        DragAndDrop.PrepareStartDrag();
                        DragAndDrop.SetGenericData("DraggedResourceAsset", new DragAndDropData(null, root, parent));
                        break;
                    case EventType.MouseUp:
                        if (!titleRect.Contains(Event.current.mousePosition))
                            break;
                        DragAndDrop.PrepareStartDrag();
                        break;
                    case EventType.MouseDrag:
                        if (!titleRect.Contains(Event.current.mousePosition))
                            break;
                        Drawers.DragAndDropData draggedData = DragAndDrop.GetGenericData("DraggedResourceAsset") as Drawers.DragAndDropData;
                        if (draggedData != null)
                        {
                            DragAndDrop.StartDrag(root.namespaceName);
                            DragAndDrop.visualMode = DragAndDropVisualMode.Move;
                        }
                        Event.current.Use();
                        break;
                }
            }

            if (EditorGUI.EndChangeCheck())
                modified = true;

            // doing header

            if (filter.Length == 0 && root.assets.Count + root.subNamespaces.Count > 0)
                root.expanded = EditorGUI.Foldout(titleRect, root.expanded, root.displayName);
            else
                EditorGUI.LabelField(titleRect, root.displayName);

            Rect deleteButtonRect = new Rect(titleRect.x + titleRect.width - EditorGUIUtility.singleLineHeight, titleRect.y, EditorGUIUtility.singleLineHeight, EditorGUIUtility.singleLineHeight);

            
            if (parent!= null && DrawDeleteButton(deleteButtonRect, darkTrashColor))
            {
                parent.RemoveNamespace(root.namespaceName);
                modified = true;
            }

            // list for checking for duplicates
            List<string> duplicateNameList = new List<string>();
            hasDuplicateNames = false;

            // drawing sub elements
            if (root.expanded || filter.Length > 0)
            {
                EditorGUI.indentLevel++;
                for (int index = 0; index < root.subNamespaces.Count; index++)
                {

                    //checking for duplicate namespace
                    if (duplicateNameList.Contains(root.subNamespaces[index].namespaceName))
                        hasDuplicateNames = true;
                    duplicateNameList.Add(root.subNamespaces[index].namespaceName);


                    bool subNamespaceModified = DrawNamespaceTree(root.subNamespaces[index], root, filter, filterType, isSelectionMode, realtimeOnly);
                    if (subNamespaceModified)
                        modified = true;
                }

                for (int index = 0; index < root.assets.Count; index++)
                {
                    ResourceAsset asset = root.assets[index];
                    //checking for duplicate namespace
                    if (duplicateNameList.Contains(asset.assetName))
                        hasDuplicateNames = true;
                    duplicateNameList.Add(asset.assetName);


                    if (Utils.CheckAgainstFilter(asset, filter, filterType))
                    {
                        bool resourceModified = DrawResourceAsset(root, asset, isSelectionMode);
                        if (resourceModified)
                            modified = true;
                    }
                }
                if (hasDuplicateNames)
                    EditorGUILayout.HelpBox("Assets and namespaces must have Unique names!", MessageType.Error, true);

                EditorGUI.indentLevel--;
            }

            if (!Utils.IsValidVariableName(root.namespaceName))
            {
                hasInvalidNames = true;
                EditorGUILayout.HelpBox("Namespace names must start with a letter, and only contain letters, numbers, underscores, and spaces", MessageType.Error, true);
            }

            if (parent != null && parent.namespaceName == root.namespaceName)
            {
                hasInvalidNames = true;
                EditorGUILayout.HelpBox("Namespace names cannot have the same name as their parent's namespace", MessageType.Error, true);

            }
            return modified;
        }

        public static void FinishDrawingTree()
        {
            hasDuplicateNames = false;
            hasInvalidNames = false;
        }

        private static bool DrawResourceAsset(ResourceAssetNamespace owningNs, ResourceAsset asset, bool isSelectionMode)
        {

            bool modified = false;
            Rect resourceTitleRect = EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight);

            GUIStyle headerFoldoutStyle = new GUIStyle(EditorStyles.foldout);
            headerFoldoutStyle.onNormal.textColor = headerTextColor;
            headerFoldoutStyle.normal.textColor = headerTextColor;
            headerFoldoutStyle.focused.textColor = headerTextColor;
            headerFoldoutStyle.active.textColor = headerTextColor;
            headerFoldoutStyle.hover.textColor = headerTextColor;
            headerFoldoutStyle.onFocused.textColor = headerTextColor;
            headerFoldoutStyle.onActive.textColor = headerTextColor;
            headerFoldoutStyle.onHover.textColor = headerTextColor;

            

            EditorGUI.DrawRect(EditorGUI.IndentedRect(resourceTitleRect), resourceColor);

            EditorGUI.BeginChangeCheck();
            // Doing right click menu

            if (resourceTitleRect.Contains(Event.current.mousePosition))
            {

                switch (Event.current.type)
                {
                    case EventType.MouseDown:
                        if (isSelectionMode)
                        {
                            selectedGuid = asset.guid;
                            break;
                        }
                        DragAndDrop.PrepareStartDrag();
                        DragAndDrop.SetGenericData("DraggedResourceAsset", new DragAndDropData(asset, null, owningNs));

                        break;
                    case EventType.MouseUp:
                        DragAndDrop.PrepareStartDrag();
                        break;
                    case EventType.MouseDrag:
                        if (isSelectionMode)
                            break;
                        Drawers.DragAndDropData draggedData = DragAndDrop.GetGenericData("DraggedResourceAsset") as Drawers.DragAndDropData;
                        if (draggedData != null)
                        {
                            DragAndDrop.StartDrag(asset.assetName);
                            DragAndDrop.visualMode = DragAndDropVisualMode.Move;
                        }
                        Event.current.Use();
                        break;
                    case EventType.Repaint:
                        break;
                    case EventType.DragUpdated:
                        break;
                    case EventType.DragPerform:
                        break;
                    case EventType.DragExited:
                        break;
                    case EventType.ContextClick:
                        if (isSelectionMode)
                            break;
                        GenericMenu menu = new GenericMenu();
                        menu.AddItem(new GUIContent("Delete"), false, () =>
                        {
                            owningNs.RemoveAsset(asset.assetName);
                            modified = true;
                        });
                        menu.ShowAsContext();
                        Event.current.Use();
                        modified = true;
                        break;
                }
            }

            if (EditorGUI.EndChangeCheck())
                modified = true;

            if (asset.guid == selectedGuid && isSelectionMode)
                EditorGUI.DrawRect(resourceTitleRect, highlightColor);

            // drawing delete button
            Rect deleteRect = new Rect(resourceTitleRect.x + resourceTitleRect.width - EditorGUIUtility.singleLineHeight,
                resourceTitleRect.y, EditorGUIUtility.singleLineHeight, EditorGUIUtility.singleLineHeight);

            if (isSelectionMode)
            {
                EditorGUI.LabelField(resourceTitleRect, asset.assetName);
                DrawAssetPreview(new Vector2(resourceTitleRect.x + resourceTitleRect.width, resourceTitleRect.y),
                    asset, resourceTitleRect.width - GUI.skin.label.CalcSize(new GUIContent(asset.assetName)).x + EditorGUIUtility.standardVerticalSpacing);
            }
            else
            {
                if (DrawDeleteButton(deleteRect, headerTextColor))
                {
                    owningNs.RemoveAsset(asset.assetName);
                    modified = true;
                }
                if (!asset.expanded)
                {
                    asset.expanded = EditorGUI.Foldout(resourceTitleRect, asset.expanded, asset.assetName, headerFoldoutStyle);
                    DrawAssetPreview(new Vector2(resourceTitleRect.x + resourceTitleRect.width - EditorGUIUtility.singleLineHeight - EditorGUIUtility.standardVerticalSpacing, resourceTitleRect.y),
                        asset, resourceTitleRect.width - GUI.skin.label.CalcSize(new GUIContent(asset.assetName)).x + EditorGUIUtility.standardVerticalSpacing);
                }
                else
                {
                    // drawing title
                    asset.expanded = EditorGUI.Foldout(new Rect(resourceTitleRect.x, resourceTitleRect.y, resourceTitleRect.width, resourceTitleRect.height)
                        , asset.expanded, asset.assetName, headerFoldoutStyle);

                    EditorGUI.BeginChangeCheck();
                    //Rect textFieldRect = new Rect(resourceTitleRect.x, resourceTitleRect.y, resourceTitleRect.width - EditorGUIUtility.singleLineHeight, resourceTitleRect.height);


                    EditorGUI.indentLevel++;
                    asset.assetName = EditorGUILayout.TextField(asset.assetName);

                    

                    if (!asset.isRealtime)
                    {
                        //drawing value controls
                        asset.assetType = (ResourceAsset.AssetType)EditorGUILayout.EnumPopup(asset.assetType);
                        switch (asset.assetType)
                        {
                            case ResourceAsset.AssetType.String:
                                asset.stringValue = EditorGUILayout.TextArea(asset.stringValue);
                                break;
                            case ResourceAsset.AssetType.Bool:
                                asset.boolValue = EditorGUILayout.Toggle(asset.boolValue);
                                break;
                            case ResourceAsset.AssetType.Float:
                                asset.floatValue = EditorGUILayout.FloatField("Value", asset.floatValue);
                                break;
                            case ResourceAsset.AssetType.Int:
                                asset.intValue = EditorGUILayout.IntField("Value", asset.intValue);
                                break;
                            case ResourceAsset.AssetType.Color:
                                asset.colorValue = EditorGUILayout.ColorField("Value", asset.colorValue);
                                break;
                            case ResourceAsset.AssetType.Enum:
                                EditorGUI.indentLevel++;

                                // for duplicate checks
                                List<string> duplicateEnumNames = new List<string>();
                                bool foundDuplicateEnum = false;

                                for (int index = 0; index < asset.enumItems.Count; index++)
                                {
                                    Rect enumValueRect = EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight);
                                    enumValueRect.width -= EditorGUIUtility.singleLineHeight;
                                    asset.enumItems[index] = EditorGUI.TextField(enumValueRect, asset.enumItems[index]);
                                    //delete button
                                    Rect deleteButtonRect = new Rect(enumValueRect.x + enumValueRect.width, enumValueRect.y, EditorGUIUtility.singleLineHeight, EditorGUIUtility.singleLineHeight);
                                    if (DrawDeleteButton(deleteButtonRect, darkTrashColor))
                                        asset.enumItems.RemoveAt(index);

                                    if (duplicateEnumNames.Contains(asset.enumItems[index]))
                                        foundDuplicateEnum = true;
                                    duplicateEnumNames.Add(asset.enumItems[index]);

                                    if (!Utils.IsValidVariableName(asset.enumItems[index]))
                                    {
                                        hasInvalidNames = true;
                                        EditorGUILayout.HelpBox("Enum values must start with a letter, and only contain letters, numbers, underscores, and spaces", MessageType.Error, true);
                                    }
                                }

                                if (foundDuplicateEnum)
                                {
                                    EditorGUILayout.HelpBox("Enum values cannot be named the same", MessageType.Error,true);
                                    hasInvalidNames = true;
                                }


                                // add button
                                Rect addButtonRect = EditorGUILayout.GetControlRect(false, 1.5f*EditorGUIUtility.singleLineHeight);
                                addButtonRect.x += addButtonRect.width - 2 * EditorGUIUtility.singleLineHeight;
                                addButtonRect.width = 2 * EditorGUIUtility.singleLineHeight;
                                if (DrawAddEnumButton(addButtonRect, darkTrashColor))
                                    asset.enumItems.Add("");

                                EditorGUI.indentLevel--;
                                break;
                            case ResourceAsset.AssetType.Vector2:
                                asset.vector2Value = EditorGUILayout.Vector2Field("Value", asset.vector2Value);
                                break;
                            case ResourceAsset.AssetType.Vector2Int:
                                asset.vector2IntValue = EditorGUILayout.Vector2IntField("Value", asset.vector2IntValue);
                                break;
                            case ResourceAsset.AssetType.Vector3:
                                asset.vector3Value = EditorGUILayout.Vector3Field("Value", asset.vector3Value);
                                break;
                            case ResourceAsset.AssetType.Vector3Int:
                                asset.vector3IntValue = EditorGUILayout.Vector3IntField("Value", asset.vector3IntValue);
                                break;
                            case ResourceAsset.AssetType.ObjectReference:
                                asset.objectReferenceValue = EditorGUILayout.ObjectField("Value", asset.objectReferenceValue, typeof(Object), false);
                                System.Type assetType = asset.objectReferenceValue != null?asset.objectReferenceValue.GetType():null;
                                if (asset.objectReferenceValue != null && (typeof(GameObject).IsAssignableFrom(assetType) || typeof(GameObject) == assetType))
                                {
                                    asset.isPool = EditorGUILayout.Toggle("Pool",asset.isPool);
                                    if (asset.isPool)
                                    {
                                        asset.poolSize = Mathf.Clamp(EditorGUILayout.IntField("Pool size", asset.poolSize), 1, int.MaxValue);
                                        asset.poolDebugEnabled = EditorGUILayout.Toggle("Write to console", asset.poolDebugEnabled);
                                    }
                                }
                                else
                                    asset.isPool = false;
                                break;
                        }
                        
                    }
                    if (!Utils.IsValidVariableName(asset.assetName))
                    {
                        hasInvalidNames = true;
                        EditorGUILayout.HelpBox("Resource names must start with a letter, and only contain letters, numbers, underscores, and spaces", MessageType.Error, true);
                    }

                    EditorGUI.indentLevel++;
                    string commentLabel = string.IsNullOrEmpty(asset.comment) ? "Comment" : GetCommentStub(asset.comment);
                    asset.commentExpanded = EditorGUILayout.Foldout(asset.commentExpanded, commentLabel);
                    if (asset.commentExpanded)
                        asset.comment = EditorGUILayout.TextArea(asset.comment);
                    EditorGUI.indentLevel--;

                    if (EditorGUI.EndChangeCheck())
                        modified = true;
                    EditorGUI.indentLevel--;
                }
            }

            return modified;
        }

        private static string GetCommentStub(string comment)
        {
            string stub = comment.Replace("\n", "");
            stub = stub.Substring(0, Mathf.Min(30, stub.Length));
            return stub;
        }

        private static void DrawAssetPreview(Vector2 topRight, ResourceAsset asset, float maxWidth)
        {
            object value = asset.GetValue();
            if (value != null)
            {
                if (asset.assetType == ResourceAsset.AssetType.ObjectReference)
                {
                    Rect imageRect = new Rect(
                        topRight.x - EditorGUIUtility.singleLineHeight,
                        topRight.y,
                        EditorGUIUtility.singleLineHeight,
                        EditorGUIUtility.singleLineHeight);
                    EditorGUI.DrawPreviewTexture(imageRect, AssetPreview.GetMiniThumbnail(value as Object));
                }
                else
                {
                    GUIStyle headerLabelStyle = new GUIStyle(EditorStyles.label);
                    headerLabelStyle.normal.textColor = headerTextColor;
                    float width = GUI.skin.label.CalcSize(new GUIContent(value.ToString())).x + (EditorGUI.indentLevel * 16);
                    width = Mathf.Clamp(width, 0, maxWidth);
                    Rect labelRect = new Rect(
                        topRight.x - width,
                        topRight.y,
                        width,
                        EditorGUIUtility.singleLineHeight);
                    EditorGUI.LabelField(labelRect, value.ToString(), headerLabelStyle);
                }
            }

        }


        // the GenericMenu.MenuFunction2 event handler for when a menu item is selected
        private static void OnAddNamespace(object root)
        {
            ResourceAssetNamespace rootNS = root as ResourceAssetNamespace;
            rootNS.expanded = true;
            NamespaceEditor.Show(ResourceAssetNamespace.Make("NewNamespace"), (ResourceAssetNamespace newNamespace) =>
            {
                rootNS.AppendNamespace(newNamespace);
            });
        }

        private static void OnAddAsset(object root, bool realtimeOnly)
        {
            ResourceAssetNamespace rootNS = root as ResourceAssetNamespace;
            rootNS.expanded = true;
            ResourceAsset newAsset = ResourceAsset.Make("", "NewResource");
            newAsset.isRealtime = realtimeOnly;
            rootNS.assets.Add(newAsset);
        }

        private class DragAndDropData
        {
            public ResourceAsset draggedAsset { get; private set; }
            public ResourceAssetNamespace draggedNamespace { get; private set; }
            public ResourceAssetNamespace sourceNamespace { get; private set; }

            public DragAndDropData(ResourceAsset draggedAsset, ResourceAssetNamespace draggedNamespace, ResourceAssetNamespace sourceNamespace)
            {
                this.draggedAsset = draggedAsset;
                this.sourceNamespace = sourceNamespace;
                this.draggedNamespace = draggedNamespace;
            }
        }
    }
}