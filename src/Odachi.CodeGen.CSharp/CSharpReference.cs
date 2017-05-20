using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Odachi.CodeModelGen.CSharp
{
    public class CSharpReference
    {
        public CSharpReference(CSharpMapping mapping, IEnumerable<CSharpReference> genericArguments)
        {
            if (mapping == null)
                throw new ArgumentNullException(nameof(mapping));
            if (genericArguments == null)
                throw new ArgumentNullException(nameof(genericArguments));

            Mapping = mapping;
            GenericArguments = genericArguments;
            Name = CreateFullName();
        }

        private string CreateFullName()
        {
            if (!GenericArguments.Any())
            {
                return Mapping.Name;
            }

            var genericArguments = string.Join(", ", GenericArguments.Select(a => a.Name));

            return $"{Mapping.Name}<{genericArguments}>";
        }

        public string Name { get; }
        public CSharpMapping Mapping { get; }

        public IEnumerable<CSharpReference> GenericArguments { get; }
    }
}
