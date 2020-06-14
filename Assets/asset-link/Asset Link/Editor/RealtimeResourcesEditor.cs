//-------------------------------
//          Asset Link
// Copyright © 2020 ABXY Games
//-------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ABXY.AssetLink.Internal
{
    [CustomEditor(typeof(RealtimeResourceContainer))]
    public class RealtimeResourcesEditor :ResourceContainerEditor
    {
        protected override string GetRootNamespaceName()
        {
            return "RScene";
        }

        protected override bool RealtimeOnly()
        {
            return true;
        }

        protected override string GetExpectedName()
        {
            return "Scene Resources.asset";
        }
    }
}