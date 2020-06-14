//-------------------------------
//          Asset Link
// Copyright © 2020 ABXY Games
//-------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ABXY.AssetLink
{
    /// <summary>
    /// Add this component to the root GameObject for any object you'd like to pool
    /// </summary>
    [AddComponentMenu("Asset Link/Pool Object")]
    public class PoolObject : MonoBehaviour
    {
        public bool isCurrentlyCheckedOut { get; private set; }

        /// <summary>
        /// The internal asset path for the Resources system
        /// </summary>
        [HideInInspector]
        public string assetPath = "";

        /// <summary>
        /// Called when the pooled object is loaned out from the pool. Use this for calling initial setup methods
        /// </summary>
        public UnityEngine.Events.UnityEvent OnLoanedFromPool = null;


        /// <summary>
        /// Called when the pooled object is returned to the pool. Use this to reset scripts on the pooled object
        /// </summary>
        public UnityEngine.Events.UnityEvent OnReturnedToPool = null;

       
        /// <summary>
        /// Call this to return this object to the pool
        /// </summary>
        public void ReturnToPool()
        {
            ABXY.AssetLink.Internal.ResourceContainer.ReturnPoolObject(assetPath, gameObject);
        }
        
        /// <summary>
        /// Called by the pooling system. You don't need to call it.
        /// </summary>
        public void Hide()
        {
            isCurrentlyCheckedOut = false;

            gameObject.SetActive(false);
            OnReturnedToPool?.Invoke();
        }

        /// <summary>
        /// Called by the pooling system. You don't need to call it.
        /// </summary>
        public void Show()
        {

            isCurrentlyCheckedOut = true;
            gameObject.SetActive(true);
            OnLoanedFromPool?.Invoke();
        }
    }
}