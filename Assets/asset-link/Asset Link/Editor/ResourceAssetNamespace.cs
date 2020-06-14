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
    public class ResourceAssetNamespace : ScriptableObject
    {
        [SerializeField]
        public List<ResourceAssetNamespace> subNamespaces = new List<ResourceAssetNamespace>();

        [SerializeField]
        public List<ResourceAsset> assets = new List<ResourceAsset>();

        [SerializeField]
        public string namespaceName = "";

        [SerializeField]
        public string displayName = "";

        [SerializeField]
        public string path = "";

        public bool expanded;



        public static ResourceAssetNamespace Make(string name)
        {
            ResourceAssetNamespace ns = ScriptableObject.CreateInstance<ResourceAssetNamespace>();
            ns.namespaceName = name;
            ns.displayName = name;
            return ns;
        }




        public ResourceAssetNamespace AppendNamespace(ResourceAssetNamespace from)
        {
            // appending subnamespaces, merging if necessary

            ResourceAssetNamespace nsCopy = from.Copy();
            nsCopy.path = Path.Combine(this.path, this.namespaceName);
            if (this.subNamespaces.Contains(from))
            {
                ResourceAssetNamespace myNamespace = subNamespaces.Find(x => x.namespaceName == from.namespaceName);
                myNamespace.AppendNamespace(nsCopy);

                //appending new resources.
                foreach (ResourceAsset newAsset in from.assets)
                {
                    if (this.assets.FindIndex(x => x.assetName == newAsset.assetName) < 0) // checking if contained
                        this.assets.Add(newAsset.Copy());
                }
            }
            else // then just add it
            {
                this.subNamespaces.Add(nsCopy);
            }
            return nsCopy;



        }

        public void AddAsset(ResourceAsset asset)
        {
            if (asset == null)
                return;

            string[] splitPath = asset.path.Split('\\', '/');
            if (splitPath.Length > 0 && splitPath[0] == this.namespaceName) // if first path segment is this namespace, just remove it;
            {
                List<string> pathList = new List<string>(splitPath);
                pathList.RemoveAt(0);
                splitPath = pathList.ToArray();
            }


            ResourceAssetNamespace currentNamespace = this;
            foreach (string pathSegment in splitPath)
            {
                int nextNamespaceIndex = currentNamespace.subNamespaces.FindIndex(x => x.namespaceName == pathSegment);
                if (nextNamespaceIndex >= 0) // has namespace
                    currentNamespace = currentNamespace.subNamespaces[nextNamespaceIndex];//advancing
                else
                {
                    ResourceAssetNamespace newNamespace = ResourceAssetNamespace.Make(pathSegment);
                    currentNamespace = currentNamespace.AppendNamespace(newNamespace);

                }
            }
            currentNamespace.assets.Add(asset);
        }

        public void RemoveAsset(string assetName)
        {
            int index = assets.FindIndex(x => x.assetName.Equals(assetName));
            if (index >= 0)
                assets.RemoveAt(index);
        }

        public void RemoveNamespace(string namespaceName)
        {
            int index = subNamespaces.FindIndex(x => x.namespaceName.Equals(namespaceName));
            if (index >= 0)
                subNamespaces.RemoveAt(index);
        }

        public ResourceAssetNamespace Copy()
        {
            ResourceAssetNamespace newNamespace = ResourceAssetNamespace.Make(this.namespaceName);
            foreach (ResourceAssetNamespace ns in subNamespaces)
                newNamespace.subNamespaces.Add(ns.Copy());
            foreach (ResourceAsset asset in assets)
                newNamespace.assets.Add(asset.Copy());
            newNamespace.expanded = expanded;
            newNamespace.path = path;
            return newNamespace;
        }

        public void ExpandTo(string guid)
        {
            ExpandToInternal(guid);
        }

        private bool ExpandToInternal(string guid)
        {
            bool foundGuid = false;
            if (assets.Find(x=>x.guid == guid) != null)
            {
                foundGuid = true;
                assets.Find(x => x.guid == guid).expanded = true;
            }
            else
            {
                foreach (ResourceAssetNamespace ns in subNamespaces)
                    if (ns.ExpandToInternal(guid))
                        foundGuid = true;
            }
            expanded = foundGuid;
            return foundGuid;
        }

        public override bool Equals(object obj)
        {
            return obj != null && obj.GetType().Equals(typeof(ResourceAssetNamespace)) && ((ResourceAssetNamespace)obj).namespaceName == namespaceName;
        }

        public override int GetHashCode()
        {
            return namespaceName.GetHashCode();
        }
    }
}