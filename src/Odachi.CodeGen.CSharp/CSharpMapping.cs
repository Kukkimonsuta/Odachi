using Odachi.CodeModelGen.CSharp.Internal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Odachi.CodeModelGen.CSharp
{
    public class CSharpMapping
    {
        public CSharpMapping(Type type, string name = null, string @namespace = null, bool? isGeneric = null)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            Type = type;
            Name = name ?? CSharpHelpers.GetName(type);
            Namespace = @namespace ?? type.Namespace;
            IsGeneric = isGeneric ?? type.GetTypeInfo().IsGenericType;
        }

        public Type Type { get; }

        public string Name { get; }
        public string Namespace { get; }
        public bool IsGeneric { get; }
    }
}
