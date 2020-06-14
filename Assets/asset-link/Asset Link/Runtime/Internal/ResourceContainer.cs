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
    
    public class ResourceContainer : ResourceContainerBase
    {
        
        private static ResourceContainer _instance = null;

        public static ResourceContainer instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = UnityEngine.Resources.Load<ResourceContainer>("Resources");
                    _instance?.Reset();
                }
                return _instance;
            }
        }



        public static object GetValue(string path)
        {
            if (instance != null)
                return instance.GetValueInternal(path);
            else
                return null;
        }

        public static GameObject GetPoolObject(string path)
        {
            if (!Application.isPlaying)
            {
                Debug.LogError("Pool objects can only be instantiated at runtime");
                return null;
            }

            if (instance != null)
                return instance.GetPoolObjectInternal(path);
            else
                return null;
        }

        public static void ReturnPoolObject(string path, GameObject poolObject)
        {
            if (!Application.isPlaying)
            {
                Debug.LogError("Pool objects can only be returned at runtime");
                return;
            }

            if (instance != null)
                instance.ReturnPoolObjectInternal(path, poolObject);
        }

        public static ResourceAsset GetByPath(string path)
        {
            if (instance != null)
                return instance.GetByPathInternal(path);
            else
                return null;
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
    }
    
}