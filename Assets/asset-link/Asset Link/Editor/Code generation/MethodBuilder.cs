//-------------------------------
//          Asset Link
// Copyright © 2020 ABXY Games
//-------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ABXY.AssetLink.Internal.CodeGen
{
    public class MethodBuilder : FieldBase
    {

        public string methodName { get; private set; }

        private List<ParameterBuilder> parameters = new List<ParameterBuilder>();

        public override FieldBase ReadLines(List<string> lines)
        {
            throw new System.NotImplementedException();
        }

        public override List<string> WriteLines(List<string> lines, int indent)
        {
            throw new System.NotImplementedException();
        }
    }
}
