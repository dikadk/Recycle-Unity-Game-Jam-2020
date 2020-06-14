//-------------------------------
//          Asset Link
// Copyright © 2020 ABXY Games
//-------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ABXY.AssetLink.Internal.CodeGen
{
    public class ArbitraryBuilder : FieldBase
    {
        List<string> content = new List<string>();
        public ArbitraryBuilder(List<string> content)
        {
            this.content = content;
        }

        public override FieldBase ReadLines(List<string> lines)
        {
            throw new System.NotImplementedException();
        }

        public override List<string> WriteLines(List<string> lines, int indent)
        {
            foreach(string line in content)
            {
                lines.Add(Indent(indent) + line);
            }
            return lines;
        }
    }
}
