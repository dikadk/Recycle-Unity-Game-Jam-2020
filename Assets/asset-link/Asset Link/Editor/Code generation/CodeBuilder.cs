//-------------------------------
//          Asset Link
// Copyright © 2020 ABXY Games
//-------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;
using System.Text.RegularExpressions;

namespace ABXY.AssetLink.Internal.CodeGen
{
    public class CodeBuilder
    {
        public static void WriteToFile(string path, FieldBase field)
        {
            List<string> lines = new List<string>();
            lines = field.WriteLines(lines, 0);

            if (File.Exists(path))
                File.Delete(path);

            FileStream fs = File.Create(path);
            StreamWriter sw = new StreamWriter(fs);

            foreach (string line in lines)
            {
                sw.WriteLine(line);
            }

            sw.Flush();
            sw.Close();
            fs.Close();
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
        }


        public static FieldBase ReadFromFile(string path)
        {
            if (File.Exists(path))
            {
                List<string> lines = new List<string>(File.ReadAllLines(path));
                for (int index = lines.Count - 1; index >= 0; index--)
                {
                    string currentLine = lines[index];
                    bool isSpace = Regex.Match(currentLine, "^[ \t]*$").Success;
                    if (isSpace || currentLine == "")
                        lines.RemoveAt(index);
                }

                return FieldBase.Interpret(lines, new FileBuilder());
            }
            return null;
        }


    }
}