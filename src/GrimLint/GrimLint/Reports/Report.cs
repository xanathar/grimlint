using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using GrimLint.Model;

namespace GrimLint.Reports
{
	public abstract class Report
	{
		public void Process(Dungeon D)
		{
			string dir = Path.Combine(D.BaseDirectory, "GrimLint", "Reports");
			Directory.CreateDirectory(dir);

			string file = Path.Combine(dir, GetReportName() + ".xlsx");
			byte[] bytes = Run(D);
			File.WriteAllBytes(file, bytes);
			
			Lint.MsgInfo("Report saved in {0}", file);
		}

		public abstract byte[] Run(Dungeon D);
		public abstract string GetReportName();
	}
}
