//-------------------------------
//          Asset Link
// Copyright © 2020 ABXY Games
//-------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ABXY.AssetLink.Internal.CodeGen;
using System.IO;
using System.Text.RegularExpressions;

namespace ABXY.AssetLink.Internal
{
    public class Tree2Code
    {
        public static void Write(ResourceAssetNamespace treeRoot, string path)
        {
            NamespaceBuilder builder = new NamespaceBuilder("ABXY.AssetLink");
            BuildCode(builder, treeRoot);
            CodeBuilder.WriteToFile(path, builder);
        }

        
        private static void BuildCode(FieldContainer root, ResourceAssetNamespace ns)
        {
            ClassBuilder newRoot = new ClassBuilder(ToValidMemberName(ns.namespaceName), false);
            root.AddField(newRoot);

            foreach (ResourceAsset asset in ns.assets)
            {
                string validAssetName = ToValidMemberName(asset.assetName);

                if (asset.isRealtime)
                {
                    List<string> getLines = new List<string>();

                    if (!string.IsNullOrEmpty( asset.comment))
                        getLines.Add("/// <summary>" + asset.comment.Replace("\n","") +"</summary>");
                    getLines.Add("public class " + validAssetName);
                    getLines.Add("{");
                    getLines.Add("    /// <summary>Get all scene components matching "+asset.fullPath+"</summary>");
                    getLines.Add("    public static UnityEngine.Component[] GetAll()");
                    getLines.Add("    {");
                    getLines.Add("        return ABXY.AssetLink.Internal.RealtimeResourceContainer.GetSceneComponents(\"" + asset.guid + "\");");
                    getLines.Add("    }");

                    getLines.Add("    /// <summary>Get all scene components matching " + asset.fullPath + " of type T</summary>");
                    getLines.Add("    public static T[] GetAll<T>() where T : UnityEngine.Component");
                    getLines.Add("    {");
                    getLines.Add("        UnityEngine.Component[] components = GetAll();");
                    getLines.Add("        System.Collections.Generic.List<T> castComponents = new System.Collections.Generic.List<T>();");
                    getLines.Add("        foreach (UnityEngine.Component component in components)");
                    getLines.Add("        {");
                    getLines.Add("            T castComponent = (T)component;");
                    getLines.Add("            if (castComponent != null)");
                    getLines.Add("                castComponents.Add(castComponent);");
                    getLines.Add("        }");
                    getLines.Add("        return castComponents.ToArray();");
                    getLines.Add("    }");

                    getLines.Add("    /// <summary>Get a scene component matching " + asset.fullPath + " of type T, if one exists</summary>");
                    getLines.Add("    public static T Get<T>() where T : UnityEngine.Component");
                    getLines.Add("    {");
                    getLines.Add("        T[] result = GetAll<T>();");
                    getLines.Add("        return result.Length != 0 ? result[0] : null;");
                    getLines.Add("    }");

                    getLines.Add("    /// <summary>Get all scene components matching " + asset.fullPath + " and the given selector predicate</summary>");
                    getLines.Add("    public static UnityEngine.Component GetWhere(System.Predicate<UnityEngine.Component> selector)");
                    getLines.Add("    {");
                    getLines.Add("        UnityEngine.Component[] result = GetAllWhere(selector);");
                    getLines.Add("        return result.Length != 0 ? result[0] : null;");
                    getLines.Add("    }");

                    getLines.Add("    /// <summary>Get a scene component matching " + asset.fullPath + " and the given selector predicate</summary>");
                    getLines.Add("    public static T GetWhere<T>(System.Predicate<T> selector) where T : UnityEngine.Component");
                    getLines.Add("    {");
                    getLines.Add("        T[] result = GetAllWhere<T>(selector);");
                    getLines.Add("        return result.Length != 0 ? result[0] : null;");
                    getLines.Add("    }");

                    getLines.Add("    /// <summary>Get a scene components matching " + asset.fullPath + "</summary>");
                    getLines.Add("    public static UnityEngine.Component Get()");
                    getLines.Add("    {");
                    getLines.Add("        UnityEngine.Component[] result = GetAll();");
                    getLines.Add("        return result.Length != 0 ? result[0] : null;");
                    getLines.Add("    }");

                    getLines.Add("    /// <summary>Get all scene components matching " + asset.fullPath + " and the given selector predicate</summary>");
                    getLines.Add("    public static UnityEngine.Component[] GetAllWhere(System.Predicate<UnityEngine.Component> selector)");
                    getLines.Add("    {");
                    getLines.Add("        System.Collections.Generic.List<UnityEngine.Component> components = new System.Collections.Generic.List<UnityEngine.Component>();");
                    getLines.Add("        components.AddRange(GetAll());");
                    getLines.Add("        return components.FindAll(selector).ToArray();");
                    getLines.Add("    }");

                    getLines.Add("    /// <summary>Get all scene components matching " + asset.fullPath + " and the given selector predicate</summary>");
                    getLines.Add("    public static T[] GetAllWhere<T>(System.Predicate<T> selector) where T : UnityEngine.Component");
                    getLines.Add("    {");
                    getLines.Add("        System.Collections.Generic.List<T> components = new System.Collections.Generic.List<T>();");
                    getLines.Add("        components.AddRange(GetAll<T>());");
                    getLines.Add("        return components.FindAll(selector).ToArray();");
                    getLines.Add("    }");
                    
                    getLines.Add("}");


                    newRoot.AddField(new ArbitraryBuilder(getLines));
                }
                else
                {
                    List<string> commentLines = new List<string>();
                    if (!string.IsNullOrEmpty(asset.comment)){
                        commentLines.Add("/// <summary>" + asset.comment.Replace("\n", "") + "</summary>");
                    }
                    newRoot.AddField(new ArbitraryBuilder(commentLines));
                    switch (asset.assetType)
                    {
                        case ResourceAsset.AssetType.String:
                            newRoot.AddField(new FieldBuilder(FieldBuilder.AccessibilityValues.Public, true, true, "string", validAssetName, "@\"" + asset.stringValue + "\""));
                            break;
                        case ResourceAsset.AssetType.Bool:
                            newRoot.AddField(new FieldBuilder(FieldBuilder.AccessibilityValues.Public, true, true, "bool", validAssetName, asset.boolValue.ToString().ToLower()));
                            break;
                        case ResourceAsset.AssetType.Float:
                            newRoot.AddField(new FieldBuilder(FieldBuilder.AccessibilityValues.Public, true, true, "float", validAssetName, asset.floatValue.ToString() + "f"));
                            break;
                        case ResourceAsset.AssetType.Int:
                            newRoot.AddField(new FieldBuilder(FieldBuilder.AccessibilityValues.Public, true, true, "int", validAssetName, asset.intValue.ToString()));
                            break;
                        case ResourceAsset.AssetType.Color:
                            newRoot.AddField(new FieldBuilder(FieldBuilder.AccessibilityValues.Public, true, true, "UnityEngine.Color",
                                validAssetName, string.Format("new UnityEngine.Color({0},{1},{2}, {3})",
                                new object[] {
                            asset.colorValue.r.ToString() + "f",
                            asset.colorValue.g.ToString() + "f",
                            asset.colorValue.g.ToString() + "f",
                            asset.colorValue.b.ToString() + "f",
                                })));
                            break;
                        case ResourceAsset.AssetType.Enum:
                            EnumBuilder enumBuilder = new EnumBuilder(validAssetName);
                            foreach (string enumValue in asset.enumItems)
                                enumBuilder.AddValue(enumValue);
                            newRoot.AddField(enumBuilder);

                            break;
                        case ResourceAsset.AssetType.Vector2:
                            newRoot.AddField(new FieldBuilder(FieldBuilder.AccessibilityValues.Public, true, true, "UnityEngine.Vector2",
                                validAssetName, string.Format("new UnityEngine.Vector2({0},{1})",
                                new object[] {
                            asset.vector2Value.x.ToString() + "f",
                            asset.vector2Value.y.ToString() + "f"
                                })));
                            break;
                        case ResourceAsset.AssetType.Vector2Int:
                            newRoot.AddField(new FieldBuilder(FieldBuilder.AccessibilityValues.Public, true, true, "UnityEngine.Vector2Int",
                                validAssetName, string.Format("new UnityEngine.Vector2Int({0},{1})",
                                new object[] {
                            asset.vector2IntValue.x.ToString(),
                            asset.vector2IntValue.y.ToString()
                                })));
                            break;
                        case ResourceAsset.AssetType.Vector3:
                            newRoot.AddField(new FieldBuilder(FieldBuilder.AccessibilityValues.Public, true, true, "UnityEngine.Vector3",
                                validAssetName, string.Format("new UnityEngine.Vector3({0},{1}, {2})",
                                new object[] {
                            asset.vector3Value.x.ToString() + "f",
                            asset.vector3Value.y.ToString() + "f",
                            asset.vector3Value.z.ToString() + "f"
                                })));
                            break;
                        case ResourceAsset.AssetType.Vector3Int:
                            newRoot.AddField(new FieldBuilder(FieldBuilder.AccessibilityValues.Public, true, true, "UnityEngine.Vector3Int",
                                validAssetName, string.Format("new UnityEngine.Vector3Int({0},{1}, {2})",
                                new object[] {
                            asset.vector3IntValue.x.ToString(),
                            asset.vector3IntValue.y.ToString(),
                            asset.vector3IntValue.z.ToString()
                                })));
                            break;
                        case ResourceAsset.AssetType.ObjectReference:
                            if (!asset.isPool)
                            {
                                if (asset.GetValue() != null)
                                {
                                    List<string> getLines = new List<string>();
                                    string typeName = asset.GetValue().GetType().FullName;
                                    string propName = validAssetName;
                                    string cachedName = "_" + validAssetName;

                                    getLines.Add("private static " + typeName + " " +cachedName + " = null;");
                                    
                                    getLines.Add("public static " + typeName + " " + propName + " {");
                                    getLines.Add("  get{");
                                    getLines.Add("      if(" + cachedName + " != null)");
                                    getLines.Add("          return " + cachedName + ";");
                                    getLines.Add("      " + cachedName + " = (" + asset.GetValue().GetType().FullName
                                        + ")ABXY.AssetLink.Internal.ResourceContainer.GetValue( \"" +
                                        Path.Combine(asset.path, asset.assetName).Replace("\\", "\\\\") + "\" );");
                                    getLines.Add("      return " + cachedName+";");
                                    getLines.Add("  }");
                                    getLines.Add("}");

                                    newRoot.AddField(new ArbitraryBuilder(getLines));
                                }
                            }
                            else
                            {
                                List<string> getLines = new List<string>();

                                getLines.Add("public class " + validAssetName);
                                getLines.Add("{");
                                getLines.Add("    private static ABXY.AssetLink.Internal.ResourceAsset cachedAsset = null;");
                                getLines.Add("    /// <summary>Get a new instance of " + asset.fullPath + " from the object pool</summary>");
                                getLines.Add("    public static UnityEngine.GameObject GetPooledInstance()");
                                getLines.Add("    {");
                                getLines.Add("          if (!UnityEngine.Application.isPlaying){");
                                getLines.Add("              UnityEngine.Debug.LogError(\"Pool objects can only be instantiated at runtime\");");
                                getLines.Add("              return null;");
                                getLines.Add("          }");
                                getLines.Add("          LoadAsset();");
                                getLines.Add("          return cachedAsset!=null?cachedAsset.GetInstance():null;");
                                getLines.Add("    }");

                                getLines.Add("    /// <summary>Return instance of " + asset.fullPath + " to the object pool</summary>");
                                getLines.Add("    public static void ReturnPooledInstance(UnityEngine.GameObject pooledAsset)");
                                getLines.Add("    {");
                                getLines.Add("          if (!UnityEngine.Application.isPlaying){");
                                getLines.Add("              UnityEngine.Debug.LogError(\"Pool objects can only be returned at runtime\");");
                                getLines.Add("              return;");
                                getLines.Add("          }");
                                getLines.Add("          LoadAsset();");
                                getLines.Add("          cachedAsset?.ReturnPooledAsset(pooledAsset);");
                                getLines.Add("    }");
                                getLines.Add("    private static void LoadAsset()");
                                getLines.Add("    {");
                                getLines.Add("          if (cachedAsset == null){");
                                getLines.Add("               ABXY.AssetLink.Internal.ResourceAsset newAsset = ABXY.AssetLink.Internal.ResourceContainer.GetByPath( \"" +
                                        Path.Combine(asset.path, asset.assetName).Replace("\\", "\\\\") + "\" );");
                                getLines.Add("               cachedAsset = newAsset != null && newAsset.isPool? newAsset:null;");
                                getLines.Add("          }");
                                getLines.Add("    }");
                                getLines.Add("}");
                                newRoot.AddField(new ArbitraryBuilder(getLines));

                            }
                            break;
                    }
                }
            }
            foreach (ResourceAssetNamespace subNS in ns.subNamespaces)
            {
                BuildCode(newRoot, subNS);
            }

        }

        public static string ToValidMemberName(string text)
        {
            text.Trim();
            text = text.Replace(" ", "_");
            text = Regex.Replace(text, "^[0-9]+", "");
            return text;

        }
    }
}