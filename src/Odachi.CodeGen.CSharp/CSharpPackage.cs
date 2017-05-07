using Odachi.CodeGen.CSharp.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Odachi.CodeGen.CSharp
{
	public class CSharpPackage
	{
		public CSharpPackage(string name, string @namespace = null)
		{
			if (name == null)
				throw new ArgumentNullException(nameof(name));

			Name = name;
			Namespace = @namespace ?? name;
		}

		public string Name { get; }

		public string Namespace { get; }

		public IList<CSharpModule> Modules { get; } = new List<CSharpModule>();

		public void Save(CSharpContext context, string path, Encoding encoding = null)
		{
			encoding = encoding ?? Encoding.GetEncoding(0);

			foreach (var module in Modules)
			{
				var filePath = Path.Combine(path, module.RelativePath);
				if (!Directory.Exists(filePath))
					Directory.CreateDirectory(filePath);

				var fullPath = Path.Combine(filePath, module.Name);

				using (var writer = new CSharpWriter())
				{
					module.WriteTo(context, this, writer);

					using (var stream = new FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.None))
					using (var output = new StreamWriter(stream, encoding))
					{
						writer.WriteTo(output);
					}
				}
			}
		}
	}
}
