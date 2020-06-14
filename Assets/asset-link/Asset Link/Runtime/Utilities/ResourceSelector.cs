//-------------------------------
//          Asset Link
// Copyright © 2020 ABXY Games
//-------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ABXY.AssetLink.Internal;

namespace ABXY.AssetLink
{
    /// <summary>
    /// Use this component to expose an Asset Link Resource selector in the editor GUI. 
    /// </summary>
    [System.Serializable]
    public struct ResourceSelector
    {

        private System.Type limitingType;

        [SerializeField]
        private string selectionGUID;

        private ResourceAsset cachedResourceAsset;

        /// <summary>
        /// Constructor for a resource selector. Use the limitTo parameter
        /// to only allow the selector to expose particular types. For example,
        /// passing in typeof(Material) tells the selector to only expose Material
        /// assets
        /// </summary>
        /// <param name="limitTo"> The limiting type</param>
        public ResourceSelector(System.Type limitTo)
        {
            limitingType = limitTo;
            selectionGUID = "";
            cachedResourceAsset = null;
        }

        /// <summary>
        /// Gets the string value of the selected resource;
        /// </summary>
        /// <returns>The value</returns>
        public string GetStringValue()
        {
            LoadBaseObject();
            if (cachedResourceAsset != null)
                return cachedResourceAsset.stringValue;
            return "";
        }

        /// <summary>
        /// Gets the float value of the selected resource;
        /// </summary>
        /// <returns>The value</returns>
        public float GetFloatValue()
        {
            LoadBaseObject();
            if (cachedResourceAsset != null)
                return cachedResourceAsset.floatValue;
            return 0f;
        }

        /// <summary>
        /// Gets the int value of the selected resource;
        /// </summary>
        /// <returns>The value</returns>
        public int GetIntValue()
        {
            LoadBaseObject();
            if (cachedResourceAsset != null)
                return cachedResourceAsset.intValue;
            return 0;
        }

        /// <summary>
        /// Gets the color value of the selected resource;
        /// </summary>
        /// <returns>The value</returns>
        public Color GetColorValue()
        {
            LoadBaseObject();
            if (cachedResourceAsset != null)
                return cachedResourceAsset.colorValue;
            return Color.magenta;
        }

        /// <summary>
        /// Gets the Vector2 value of the selected resource;
        /// </summary>
        /// <returns>The value</returns>
        public Vector2 GetVector2Value()
        {
            LoadBaseObject();
            if (cachedResourceAsset != null)
                return cachedResourceAsset.vector2Value;
            return Vector2.zero;
        }

        /// <summary>
        /// Gets the Vector2INt value of the selected resource;
        /// </summary>
        /// <returns>The value</returns>
        public Vector2Int GetVector2IntValue()
        {
            LoadBaseObject();
            if (cachedResourceAsset != null)
                return cachedResourceAsset.vector2IntValue;
            return Vector2Int.zero;
        }

        /// <summary>
        /// Gets the Vector3 value of the selected resource;
        /// </summary>
        /// <returns>The value</returns>
        public Vector3 GetVector3Value()
        {
            LoadBaseObject();
            if (cachedResourceAsset != null)
                return cachedResourceAsset.vector3Value;
            return Vector3.zero;
        }

        /// <summary>
        /// Gets the Vector3Int value of the selected resource;
        /// </summary>
        /// <returns>The value</returns>
        public Vector3Int GetVector3IntValue()
        {
            LoadBaseObject();
            if (cachedResourceAsset != null)
                return cachedResourceAsset.vector3IntValue;
            return Vector3Int.zero;
        }

        /// <summary>
        /// Gets the bppl value of the selected resource;
        /// </summary>
        /// <returns>The value</returns>
        public bool GetBoolValue()
        {
            LoadBaseObject();
            if (cachedResourceAsset != null)
                return cachedResourceAsset.boolValue;
            return false ;
        }

        /// <summary>
        /// Gets the object reference value of the selected resource;
        /// </summary>
        /// <returns>The value</returns>
        public T GetObjectReference<T>() where T : Object
        {
            LoadBaseObject();
            if (cachedResourceAsset != null)
                return cachedResourceAsset.objectReferenceValue as T;
            return null;
        }

        private void LoadBaseObject()
        {
            if (cachedResourceAsset != null)
                return;
            ResourceAsset asset = ResourceContainer.GetByGUID(selectionGUID);
            if (!Utils.CheckAgainstFilter(asset, "", limitingType.FullName))
                asset = null;

            if (asset != null)
                cachedResourceAsset = asset;
            else
                Debug.LogWarning("No resource selected");
        }
    }
}