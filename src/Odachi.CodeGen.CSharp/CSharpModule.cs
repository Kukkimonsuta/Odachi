using Odachi.CodeModelGen.CSharp.Internal;
using Odachi.CodeModelGen.CSharp.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Odachi.CodeModelGen.CSharp
{
    public class CSharpModule
    {
        public CSharpModule(string name, string relativeNamespace = null, string relativePath = null)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            Name = name;
            RelativeNamespace = relativeNamespace;
            RelativePath = relativePath ?? ".";

            UseNamespace("System");
        }

        public string Name { get; }
        public string RelativeNamespace { get; }
        public string RelativePath { get; }

        public IList<string> Usings { get; } = new List<string>();
        public IList<CSharpFragment> Fragments { get; } = new List<CSharpFragment>();

        public void UseReference(CSharpReference reference)
        {
            UseNamespace(reference.Mapping.Namespace);

            foreach (var genericArgument in reference.GenericArguments)
            {
                UseReference(genericArgument);
            }
        }
        public void UseNamespace(string @namespace)
        {
            if (@namespace == null)
                return;

            if (Usings.Contains(@namespace, StringComparer.OrdinalIgnoreCase))
                return;

            Usings.Add(@namespace);
        }

        public void WriteTo(CSharpContext context, CSharpPackage package, CSharpWriter writer)
        {
            using (var bodyWriter = new CSharpWriter())
            {
                // render body first, so usings are resolved
                using (bodyWriter.WriteIndentedBlock(prefix: $"namespace {CSharpHelpers.CombineNamespaces(package.Namespace, this.RelativeNamespace)} "))
                {
                    foreach (var fragment in Fragments)
                    {
                        fragment.WriteTo(context, package, this, bodyWriter);
                    }
                }

                bodyWriter.Flush();

                // render usings
                if (Usings.Count > 0)
                {
                    foreach (var @using in Usings)
                    {
                        writer.WriteIndented($"using {@using};");
                    }

                    writer.WriteLine();
                }

                // render body
                bodyWriter.WriteTo(writer);
            }
        }
    }
}
