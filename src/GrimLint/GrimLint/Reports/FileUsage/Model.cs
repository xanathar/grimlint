using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GrimrockModelToolkit.Grim3d;

namespace GrimLint.Reports.FileUsage
{
	class Model : StatObject
	{
		public string NameRealCase;

		public HashSet<string> Materials = new HashSet<string>();
		public List<string> ObjectNamesUsingThisModel = new List<string>();

		public Model(string file)
		{
			this.Name = NormalizeFilename(file);
			this.NameRealCase = NormalizeFilenamePartial(file);
			this.FileSize = GetFileSize(file);

			GrimModel model = GrimModel.LoadFromPath(file);

			foreach (var node in model.Nodes)
			{
				if ((node.MeshEntity != null) && (node.MeshEntity.MeshData != null) && (node.MeshEntity.MeshData.Segments != null))
				{
					foreach (var seg in node.MeshEntity.MeshData.Segments)
					{
						if (!string.IsNullOrWhiteSpace(seg.Material))
							Materials.Add(seg.Material);
					}
				}
			}
		}

		protected override IEnumerable<string> GetPropertyNamesSpecific()
		{
			yield return "Materials";
			yield return "Definitions using this";
		}

		protected override IEnumerable<object> GetPropertyValuesSpecific()
		{
			yield return string.Join(", ", Materials);
			yield return string.Join(", ", ObjectNamesUsingThisModel);
		}
	}
}
