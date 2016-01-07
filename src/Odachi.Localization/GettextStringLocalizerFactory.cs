using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Globalization;
using System.IO;

namespace Odachi.Localization
{
	public class GettextStringLocalizerFactory : IStringLocalizerFactory
	{
		public GettextStringLocalizerFactory(IGettextProcessor processor)
		{
			if (processor == null)
				throw new ArgumentNullException(nameof(processor));

			_processor = processor;
		}

		private IGettextProcessor _processor;

		public IStringLocalizer Create(Type resourceSource)
		{
			return new GettextStringLocalizer(_processor);
		}

		public IStringLocalizer Create(string baseName, string location)
		{
			return new GettextStringLocalizer(_processor);
		}
	}
}
