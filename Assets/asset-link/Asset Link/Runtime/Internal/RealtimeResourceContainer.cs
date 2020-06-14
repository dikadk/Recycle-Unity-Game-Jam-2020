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
    public class RealtimeResourceContainer : ResourceContainerBase
    {
        private static RealtimeResourceContainer _instance = null;


        public static RealtimeResourceContainer instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = UnityEngine.Resources.Load<RealtimeResourceContainer>("Scene Resources");
                    _instance?.Reset();
                }
                return _instance;
            }
        }


        public static ResourceAsset GetByGUID(string guid)
        {
            if (instance != null)
                return instance.GetByGuidInternal(guid);
            else
                return null;
        }


        public static List<ResourceAsset> GetAssets()
        {
            if (instance != null)
                return instance.GetAssetsInternal();
            else
                return new List<ResourceAsset>();
        }

        public static void RegisterSceneComponent(string guid, Component component)
        {
            ResourceAsset asset = GetByGUID(guid);
            if (asset != null && !asset.registeredComponents.Contains(component))
            {
                asset.registeredComponents.Add(component);
            }
        }

        public static void UnRegisterSceneComponent(string guid, Component component)
        {
            ResourceAsset asset = GetByGUID(guid);
            if (asset != null)
            {
                asset.registeredComponents.Remove(component);
            }
        }

        public static Component[] GetSceneComponents(string guid)
        {
            ResourceAsset asset = GetByGUID(guid);
            List<Component> components = new List<Component>();
            if (asset != null)
            {
                components.AddRange(asset.registeredComponents);
            }
            return components.ToArray();
        }

    }
    
}