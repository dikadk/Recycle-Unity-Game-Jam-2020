//-------------------------------
//          Asset Link
// Copyright © 2020 ABXY Games
//-------------------------------

using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace ABXY.AssetLink.Internal {
    public class CreateAssetMenuItems
    {
        [UnityEditor.MenuItem("Assets/Create/Asset Link/Resources",priority =30)]
        public static void MakeResources()
        {
            string assetPath = AssetDatabase.GetAssetPath(Selection.activeObject);

            if (assetPath.Length > 0)
            {
                string path = Application.dataPath.Replace("/Assets/", "/").Replace("/Assets", "/") + assetPath;
                if (!Directory.Exists(path)) // then this is a file
                    assetPath = Path.GetDirectoryName(assetPath);


                ResourceContainer container = ScriptableObject.CreateInstance<ResourceContainer>();
                AssetDatabase.CreateAsset(container, Path.Combine(assetPath,"Resources.asset"));
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                Selection.activeObject = container;
            }

        }

        [UnityEditor.MenuItem("Assets/Create/Asset Link/Resources", validate = true)]
        public static bool MakeResourcesValidate()
        {
            return ResourceContainer.instance == null;
        }


        [UnityEditor.MenuItem("Assets/Create/Asset Link/Scene Resources", priority = 31)]
        public static void MakeSceneResources()
        {
            string assetPath = AssetDatabase.GetAssetPath(Selection.activeObject);

            if (assetPath.Length > 0)
            {
                string path = Application.dataPath.Replace("/Assets/", "/").Replace("/Assets", "/") + assetPath;
                if (!Directory.Exists(path)) // then this is a file
                    assetPath = Path.GetDirectoryName(assetPath);

                RealtimeResourceContainer container = ScriptableObject.CreateInstance<RealtimeResourceContainer>();
                AssetDatabase.CreateAsset(container, Path.Combine(assetPath, "Scene Resources.asset"));
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                Selection.activeObject = container;
            }

        }

        [UnityEditor.MenuItem("Assets/Create/Asset Link/Scene Resources", validate = true)]
        public static bool MakeSceneResourcesValidate()
        {
            return RealtimeResourceContainer.instance == null;
        }
    }
}
