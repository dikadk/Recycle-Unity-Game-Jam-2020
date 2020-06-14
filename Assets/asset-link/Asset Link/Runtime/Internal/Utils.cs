//-------------------------------
//          Asset Link
// Copyright © 2020 ABXY Games
//-------------------------------

using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace ABXY.AssetLink.Internal
{
    public static class Utils
    {
        public static bool CheckAgainstFilter(ResourceAsset asset, string filterText, string filterType)
        {
            bool ok = true;
            if (!asset.assetName.ToLower().Contains(filterText.ToLower()) && filterText.Length != 0)
                ok = false;

            if (!string.IsNullOrEmpty(filterType))
            {
                ResourceAsset.AssetType assetFilterType = ResourceAsset.AssetType.Color;
                if (filterType == typeof(string).FullName)
                    assetFilterType = ResourceAsset.AssetType.String;
                else if (filterType == typeof(float).FullName)
                    assetFilterType = ResourceAsset.AssetType.Float;
                else if (filterType == typeof(int).FullName)
                    assetFilterType = ResourceAsset.AssetType.Int;
                else if (filterType == typeof(Color).FullName)
                    assetFilterType = ResourceAsset.AssetType.Color;
                else if (filterType == typeof(System.Enum).FullName)
                    assetFilterType = ResourceAsset.AssetType.Enum;
                else if (filterType == typeof(Vector2).FullName)
                    assetFilterType = ResourceAsset.AssetType.Vector2;
                else if (filterType == typeof(Vector2Int).FullName)
                    assetFilterType = ResourceAsset.AssetType.Vector2Int;
                else if (filterType == typeof(Vector3).FullName)
                    assetFilterType = ResourceAsset.AssetType.Vector3;
                else if (filterType == typeof(Vector3Int).FullName)
                    assetFilterType = ResourceAsset.AssetType.Vector3Int;
                else
                    assetFilterType = ResourceAsset.AssetType.ObjectReference;

                if (assetFilterType != asset.assetType)
                    ok = false;

                if (assetFilterType == ResourceAsset.AssetType.ObjectReference
                    && asset.assetType == ResourceAsset.AssetType.ObjectReference
                    && (asset.GetValue() == null || !asset.GetValue().GetType().FullName.Contains(filterType)))
                    ok = false;
            }
            return ok;
        }

        private static List<string> reservedNames = new List<string>(new string[] {
            "abstract", "as", "base", "bool", "break", "byte", "case", "catch", "char", "checked", "class",
            "const", "continue", "decimal", "default", "delegate", "do", "double", "else", "enum", "event",
            "explicit", "extern", "false", "finally", "fixed", "float", "for", "foreach", "goto", "if", "implicit",
            "in", "int", "interface", "internal", "is", "lock", "long", "namespace", "new", "null", "object",
            "operator", "out", "override", "params", "private", "protected", "public", "readonly", "ref",
            "return", "sbyte", "sealed", "short", "sizeof", "stackalloc", "static", "string", "struct", "switch",
            "this", "throw", "true", "try", "typeof", "uint", "ulong", "unchecked", "unsafe", "ushort", "using",
            "using static", "virtual", "void", "volatile", "while" });

        public static bool IsValidVariableName(string varName)
        {
            if (string.IsNullOrEmpty(varName))
                return false;

            if (!char.IsLetter(varName[0]))
                return false;

            Match match = Regex.Match(varName, "[^a-zA-Z0-9_ ]");
            if (match.Success)
                return false;

            if (reservedNames.Contains(varName))
                return false;

            return true;
        }
    }
}