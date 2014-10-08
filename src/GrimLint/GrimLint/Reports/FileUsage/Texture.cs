using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GrimLint.Reports.FileUsage
{
	class Texture : StatObject
	{
		public string NameRealCase;

		public Texture(string file)
		{
			this.Name = NormalizeFilename(file);
			this.NameRealCase = NormalizeFilenamePartial(file);
			this.FileSize = GetFileSize(file);
		}

		protected override IEnumerable<string> GetPropertyNamesSpecific()
		{
			yield break;
		}

		protected override IEnumerable<object> GetPropertyValuesSpecific()
		{
			yield break;
		}
	}
}
