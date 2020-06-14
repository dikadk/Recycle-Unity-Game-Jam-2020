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
    [System.Serializable]
    public class ResourceAsset : ScriptableObject
    {
        public string path = "";

        public string assetName = "";

        public enum AssetType { String, Float, Int, Color, Enum, Vector2, Vector2Int, Vector3, Vector3Int, ObjectReference, Bool }

        public AssetType assetType;

        public string stringValue;

        public float floatValue;

        public int intValue;

        public Color colorValue;

        public List<string> enumItems = new List<string>();

        public Vector2 vector2Value;

        public Vector2Int vector2IntValue;

        public Vector3 vector3Value;

        public Vector3Int vector3IntValue;

        public Object objectReferenceValue;

        public bool boolValue;

        public bool isRealtime = false;

        public string guid;

        public bool isPool = false;

        public int poolSize = 5;

        [HideInInspector]
        public bool expanded = false;

        public string comment = "";

        // realtime Resources
        public List<Component> registeredComponents = new List<Component>();

        private PoolObject[] objectPool = new PoolObject[0];

        private int objectPoolIndex = -1;

        private bool poolInitialized = false;

        public bool poolDebugEnabled = false;

        public bool commentExpanded = false;

        private static GameObject _poolObjectRoot = null;
        private static GameObject poolObjectRoot
        {
            get
            {
                if (_poolObjectRoot == null && Application.isPlaying)
                {
                    _poolObjectRoot = new GameObject("Object pool");
                    DontDestroyOnLoad(_poolObjectRoot);
                }
                return _poolObjectRoot;
            }
        }

        public string fullPath
        {
            get
            {
                return Path.Combine(path, assetName);
            }
        }

        public GameObject GetInstance()
        {
            InitializePool();
            if (objectReferenceValue == null)
                return null;


            GameObject result = null;
            PoolObject poolObject = null;
            if (!isPool)
            { // This should never happen, returning null
                Debug.LogError("Pooling methods called on a non-pooled object. This should not happen");
                return null;
            }
            else // then normal operation
            {
                if (objectPoolIndex + 1< objectPool.Length)
                {
                    objectPoolIndex++;
                    poolObject = objectPool[objectPoolIndex];
                    objectPool[objectPoolIndex] = null;
                    if(poolDebugEnabled) Debug.Log("Loaning out pooled object - " + fullPath);
                }
            }

            if (poolObject == null) // then the pool object I had on hand was destroyed externally, or I'm out of pool objects
            {
                if (poolDebugEnabled) Debug.Log("Making new pooled object - " + fullPath);
                poolObject = MakePoolInstance();
                poolObject.Show();
            }

            if (poolObject != null)
            {
                result = poolObject.gameObject;
                poolObject.transform.parent = null;
                poolObject.Show();
            }
            return result;
        }

        private PoolObject MakePoolInstance()
        {
            GameObject newPoolObject = (GameObject)GameObject.Instantiate(objectReferenceValue);
            //newPoolObject.name = assetPath + " (Pooled Instance)";
            PoolObject poolObject = newPoolObject.GetComponent<PoolObject>();
            if (poolObject == null)
            {
                Debug.LogError("Pooled asset " + assetName + " does not have a PoolObject component in the asset root. It will not instantiate");
                return null;
            }

            string assetPath = Path.Combine(path, assetName);
            poolObject.assetPath = assetPath;
            return poolObject;
        }

        
        private void InitializePool()
        {
            if (!poolInitialized && isPool)
            {
                objectPool = new PoolObject[poolSize];
                string assetPath = Path.Combine(path, assetName);
                for (int index = 0; index < poolSize; index++)
                {
                    GameObject objectGo = (GameObject)GameObject.Instantiate(objectReferenceValue);
                    PoolObject poolObject = objectGo.GetComponent<PoolObject>();
                    if (poolObject == null)
                    {
                        Debug.LogError("Pooled asset " + assetName + " does not have a PoolObject component in the asset root. It will not instantiate");
                        return;
                    }
                    poolObject.assetPath = assetPath;
                    poolObject.transform.parent = poolObjectRoot.transform;
                    poolObject.name = assetPath + " (Pooled Instance)";
                    objectPool[index] = poolObject;

                    poolObject.Hide();
                }
            }
            poolInitialized = true;
        }

        public void ReturnPooledAsset(GameObject go)
        {
            if (go == null)
            {
                Debug.LogError("Cannot return object to " + assetName + " pool, object is null");
                return;
            }

            
            PoolObject poolObject = go.GetComponent<PoolObject>();
            if (poolObject == null)
            {
                Debug.LogError("Cannot return object " + go.name + " to " + assetName
                    + " pool, object does not have a PoolObject Component attached to its root");
                return;
            }
            string assetPath = Path.Combine(path, assetName);
            if (poolObject.assetPath != assetPath)
            {
                Debug.LogError("Cannot return object " + go.name + " to " + assetName
                    + " pool, object did not originate from this pool");
                return;
            }

            if (!poolObject.isCurrentlyCheckedOut)
            {
                Debug.LogError("Pool object "+assetName+" already exists in a pool. It cannot be returned to another");
                poolObject.Hide();
                return;
            }

            if (objectPoolIndex == -1)//then pool is full
            {
                if (poolDebugEnabled) Debug.Log(fullPath + " - pool is full, so returned object will be destroyed");
                poolObject.Hide();
                Destroy(go);
                return;
            }

            poolObject.Hide();
            objectPool[objectPoolIndex] = (poolObject);
            poolObject.transform.parent = poolObjectRoot.transform;
            poolObject.name = assetPath + " (Pooled Instance)";
            objectPoolIndex--;
            if (poolDebugEnabled) Debug.Log( fullPath + " - Pooled object returned");
        }

        public static ResourceAsset Make(string path, string name)
        {
            ResourceAsset newAsset = CreateInstance<ResourceAsset>();
            newAsset.assetName = name;
            newAsset.path = path;
            newAsset.expanded = false;
            newAsset.guid = System.Guid.NewGuid().ToString();
            newAsset.hideFlags = HideFlags.HideInHierarchy;
            return newAsset;
        }

        public ResourceAsset Copy()
        {
            ResourceAsset newAsset = ResourceAsset.Make("", "");
            newAsset.assetName = assetName;
            newAsset.assetType = assetType;
            newAsset.stringValue = stringValue;
            newAsset.floatValue = floatValue;
            newAsset.intValue = intValue;
            newAsset.colorValue = colorValue;
            newAsset.enumItems = enumItems;
            newAsset.vector2IntValue = vector2IntValue;
            newAsset.vector2Value = vector2Value;
            newAsset.vector3Value = vector3Value;
            newAsset.vector3IntValue = vector3IntValue;
            newAsset.boolValue = boolValue;
            newAsset.objectReferenceValue = objectReferenceValue;
            newAsset.path = path;
            newAsset.guid = guid;
            newAsset.isRealtime = isRealtime;
            newAsset.isPool = isPool;
            newAsset.poolSize = poolSize;
            newAsset.comment = comment;
            newAsset.poolDebugEnabled = poolDebugEnabled;
            return newAsset;
        }


        public void CopyFrom(ResourceAsset other)
        {
            assetName = other.assetName;
            assetType = other.assetType;
            stringValue = other.stringValue;
            floatValue = other.floatValue;
            intValue = other.intValue;
            colorValue = other.colorValue;
            enumItems = other.enumItems;
            vector2IntValue = other.vector2IntValue;
            vector2Value = other.vector2Value;
            vector3Value = other.vector3Value;
            vector3IntValue = other.vector3IntValue;
            boolValue = other.boolValue;
            objectReferenceValue = other.objectReferenceValue;
            path = other.path;
            guid = other.guid;
            isRealtime = other.isRealtime;
            isPool = other.isPool;
            poolSize = other.poolSize;
            comment = other.comment;
            poolDebugEnabled = other.poolDebugEnabled;
        }

        public object GetValue()
        {
            switch (assetType)
            {
                case AssetType.String:
                    return stringValue;
                case AssetType.Float:
                    return floatValue;
                case AssetType.Int:
                    return intValue;
                case AssetType.Color:
                    return colorValue;
                case AssetType.Enum:
                    return null;
                case AssetType.Vector2:
                    return vector2Value;
                case AssetType.Vector2Int:
                    return vector2IntValue;
                case AssetType.Vector3:
                    return vector3Value;
                case AssetType.Vector3Int:
                    return vector3IntValue;
                case AssetType.ObjectReference:
                    return objectReferenceValue;
                case AssetType.Bool:
                    return boolValue;
            }
            return null;
        }
    }
}