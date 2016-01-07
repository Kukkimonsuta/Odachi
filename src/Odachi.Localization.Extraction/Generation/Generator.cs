using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Odachi.Localization.Extraction.Generation
{
	public abstract class Generator : IDisposable
	{
		public virtual void WriteHeader() { }

		public abstract void WriteEntry(KeywordMatchGroup match);

		public virtual void WriteFooter() { }

		public abstract void Flush();

		#region IDisposable

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
			}
		}

		public void Dispose()
		{
			Dispose(true);
		}

		#endregion
	}

	public interface IGeneratorFactory
	{
		string DefaultExtension { get; }

		Generator Create(Stream output, Encoding encoding);
	}
}
