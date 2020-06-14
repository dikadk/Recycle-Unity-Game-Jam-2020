//-------------------------------
//          Asset Link
// Copyright © 2020 ABXY Games
//-------------------------------

using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ABXY.AssetLink.Internal
{
    public class ResourceContainerBase : ScriptableObject
    {
        [SerializeField]
        protected List<ResourceAsset> resources = new List<ResourceAsset>();

        [SerializeField]
        protected string savePath = "";

#pragma warning disable CS0414
        [SerializeField]
        private bool pathHasBeenInitialized = false;
#pragma warning restore CS0414

        protected Dictionary<string, ResourceAsset> assetDictionary = new Dictionary<string, ResourceAsset>();

        protected object GetValueInternal(string path)
        {
            LoadAssets();
            ResourceAsset resource = null;
            assetDictionary.TryGetValue(path, out resource);
            if (resource != null)
                return resource.GetValue();
            else
                return null;
        }

        protected GameObject GetPoolObjectInternal(string path)
        {
            LoadAssets();
            ResourceAsset resource = null;
            assetDictionary.TryGetValue(path, out resource);
            if (resource != null && resource.isPool)
                return resource.GetInstance();
            else
                return null;
        }

        protected void ReturnPoolObjectInternal(string path, GameObject poolObject)
        {
            LoadAssets();
            ResourceAsset resource = null;
            assetDictionary.TryGetValue(path, out resource);
            if (resource != null && resource.isPool)
                resource.ReturnPooledAsset(poolObject);
        }

        private void LoadAssets()
        {
            if (assetDictionary.Count == 0)
            {
                foreach (ResourceAsset asset in resources)
                    assetDictionary.Add(Path.Combine(asset.path, asset.assetName), asset.Copy());
            }
        }

        protected ResourceAsset GetByPathInternal(string path)
        {
            LoadAssets();
            ResourceAsset resource = null;
            assetDictionary.TryGetValue(path, out resource);
            return resource;
        }

        protected ResourceAsset GetByGuidInternal(string guid)
        {
            int index = resources.FindIndex(x => x.guid == guid);
            if (index < 0)
                return null;
            return resources[index];
        }

        protected List<ResourceAsset> GetAssetsInternal()
        {
            return resources;
        }

        protected void Reset()
        {
            assetDictionary.Clear();
        }
    }
}