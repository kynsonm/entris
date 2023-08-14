using UnityEngine;
using System;

namespace SubclassList
{
    public class SubclassListAttribute : PropertyAttribute
    {
        Type type;
        public Type Type => type;

        public SubclassListAttribute(Type type)
        {
            this.type = type;
        }
    }
}