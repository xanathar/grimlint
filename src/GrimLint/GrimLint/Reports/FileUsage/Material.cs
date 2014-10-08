using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GrimLint.Reports.FileUsage
{
	class Material : StatObject
	{
		public HashSet<string> Textures;

		public Material(GrimLint.Model.Definition def)
		{
			Name = def.Name;
			Textures = new HashSet<string>(def.FlattenedValues.OfType<string>().Select(f => f.ToLower()).Where(f => f.EndsWith(".tga")).Select(f => f.Replace(".tga", ".dds")));
		}

		protected override IEnumerable<string> GetPropertyNamesSpecific()
		{
			yield return "Textures";
		}

		protected override IEnumerable<object> GetPropertyValuesSpecific()
		{
			yield return string.Join(", ", Textures);
		}
	}
}
