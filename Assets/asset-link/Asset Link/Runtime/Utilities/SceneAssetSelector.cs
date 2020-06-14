//-------------------------------
//          Asset Link
// Copyright © 2020 ABXY Games
//-------------------------------

using ABXY.AssetLink.Internal;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ABXY.AssetLink
{
    /// <summary>
    /// Use this component to expose an Asset Link Scene Resource selector in the editor GUI.
    /// It enables users to select a scene resource type, and then find matching scene objects at runtime
    /// </summary>
    [System.Serializable]
    public struct SceneAssetSelector
    {

# pragma warning disable CS0649
        [SerializeField]
        private string selectionGUID;
#pragma warning restore CS0649

        /// <summary>
        /// Used by the Scene resources system, you don't need this
        /// </summary>
        /// <returns></returns>
        public string GetGuid()
        {
            return selectionGUID;
        }

        /// <summary>
        /// Retrieves all scene components matching selector
        /// </summary>
        /// <returns>An array of matching components</returns>
        public Component[] GetAll()
        {
            return RealtimeResourceContainer.GetSceneComponents(selectionGUID);

        }

        /// <summary>
        /// Retrieves all scene components of type T matching the selector
        /// </summary>
        /// <typeparam name="T"> The component type</typeparam>
        /// <returns>An array of matching components</returns>
        public T[] GetAll<T>() where T : UnityEngine.Component
        {
            Component[] components = GetAll();
            List<T> castComponents = new List<T>();
            foreach (Component component in components)
            {
                T castComponent = (T)component;
                if (castComponent != null)
                    castComponents.Add(castComponent);
            }
            return castComponents.ToArray();
        }

        /// <summary>
        /// Retrieves a scene component of type T matching the selector, if one exists
        /// </summary>
        /// <typeparam name="T">The component type</typeparam>
        /// <returns>The matching component</returns>
        public T Get<T>() where T : Component
        {
            T[] result = GetAll<T>();
            return result.Length != 0 ? result[0] : null;
        }

    }
}