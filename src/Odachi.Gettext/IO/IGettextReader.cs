using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Odachi.Gettext.IO
{
	public interface IGettextReader
	{
		Encoding Encoding { get; set; }

		Translation ReadTranslation();
	}
}
