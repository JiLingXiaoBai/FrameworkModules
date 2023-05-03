using System;
using System.Reflection;
using UnityEditor.UIElements;
using UnityEngine;

namespace JLXB.Framework.BehaviorTree.Editor
{
    public class BoundsResolver : FieldResolver<BoundsField, Bounds>
    {
        public BoundsResolver(FieldInfo fieldInfo) : base(fieldInfo)
        {
        }
        protected override BoundsField CreateEditorField(FieldInfo fieldInfo)
        {
            return new BoundsField(fieldInfo.Name);
        }

        public static bool IsAcceptable(Type infoType, FieldInfo info) => infoType == typeof(Bounds);
    }
}