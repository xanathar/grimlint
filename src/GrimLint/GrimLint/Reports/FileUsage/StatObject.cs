using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace GrimLint.Reports.FileUsage
{
	abstract class StatObject
	{
		public string Name;
		public int TimesUsed = 0;
		public int FileSize = 0;
		public HashSet<string> UsedInWallsets = new HashSet<string>();
		public HashSet<string> UsedInstancesSamples = new HashSet<string>();

		public IEnumerable<string> GetPropertyNames()
		{
			yield return "Name";
			yield return "Times Used";
			yield return "Size in Bytes";
			yield return "Wallsets using this";
			yield return "Sample of objects using this";

			foreach (string s in GetPropertyNamesSpecific())
				yield return s;
		}

		public IEnumerable<object> GetPropertyValues()
		{
			yield return Name;
			yield return TimesUsed;
			yield return FileSize;
			yield return string.Join(", ", UsedInWallsets);
			yield return string.Join(", ", UsedInstancesSamples.Take(3));

			foreach (string s in GetPropertyValuesSpecific())
				yield return s;
		}

		protected abstract IEnumerable<string> GetPropertyNamesSpecific();
		protected abstract IEnumerable<object> GetPropertyValuesSpecific();

		 


		protected int GetFileSize(string file)
		{
			FileInfo f = new FileInfo(file);
			return (int)f.Length;
		}

		protected string NormalizeFilename(string file)
		{
			int i = file.IndexOf("mod_assets");
			return file.Substring(i).ToLower().Replace('\\', '/');
		}

		protected string NormalizeFilenamePartial(string file)
		{
			int i = file.IndexOf("mod_assets");
			return file.Substring(i).Replace('\\', '/');
		}

		public void MergeStatsOfParent(StatObject parent)
		{
			TimesUsed += parent.TimesUsed;
			UsedInstancesSamples.UnionWith(parent.UsedInstancesSamples);
			UsedInWallsets.UnionWith(parent.UsedInWallsets);
		}

		public void MarkUsedByWallset(string wsname)
		{
			UsedInWallsets.Add(wsname);
			++TimesUsed;
		}

		public void MarkUsedByObject(string id, string name)
		{
			UsedInstancesSamples.Add(name);
			++TimesUsed;
		}

	}
}
