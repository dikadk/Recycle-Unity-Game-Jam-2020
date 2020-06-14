//-------------------------------
//          Asset Link
// Copyright © 2020 ABXY Games
//-------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ABXY.AssetLink.Internal.CodeGen
{
    public class FileBuilder : FieldContainer
    {
        private List<FieldBase> fields = new List<FieldBase>();

        public override void AddField(FieldBase newField)
        {
            fields.Add(newField);
        }

        public override FieldBase ReadLines(List<string> lines)
        {
            return null;
        }

        public override List<string> WriteLines(List<string> lines, int indent)
        {
            foreach (FieldBase field in fields)
            {
                lines = field.WriteLines(lines, 0);
            }
            return lines;
        }

        public override void Merge(FieldBase field)
        {
            if (field.GetType() == typeof(FileBuilder))
            {
                FileBuilder castField = field as FileBuilder;

                foreach (FieldBase child in castField.fields)
                {
                    FieldBase preexistingField = fields.FindLast(x => x.name == child.name);
                    if (preexistingField != null)
                    {
                        preexistingField.Merge(child);
                    }
                    else
                    {
                        fields.Add(child);
                    }
                }

            }
        }
    }

}