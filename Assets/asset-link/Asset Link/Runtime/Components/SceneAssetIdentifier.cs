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
    /// Use this component to make the target component visibile to the Asset Link Scene Resources System
    /// </summary>
    [AddComponentMenu("Asset Link/Scene Asset Identifier")]
    [ExecuteInEditMode]
    public class SceneAssetIdentifier : MonoBehaviour
    {
        [SerializeField]
        SceneAssetSelector selector = new SceneAssetSelector();

        /// <summary>
        /// The target component
        /// </summary>
        [SerializeField]
        private Component target = null;

        private void OnEnable()
        {
            if (target == null) return;
            RealtimeResourceContainer.RegisterSceneComponent(selector.GetGuid(), target);
        }

        private void OnDisable()
        {
            if (target == null) return;
            RealtimeResourceContainer.UnRegisterSceneComponent(selector.GetGuid(), target);
        }

        private void OnDestroy()
        {
            if (target == null) return;
            RealtimeResourceContainer.UnRegisterSceneComponent(selector.GetGuid(), target);
        }

    }
}