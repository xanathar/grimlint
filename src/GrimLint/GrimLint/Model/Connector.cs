using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GrimLint.Model
{
	public class Connector
	{
		public string Source { get; set; }
		public string SourceAction { get; set; }
		public string Target { get; set; }
		public string TargetAction { get; set; }

		public Connector()
		{

		}

		public Connector(string source, string addConnMethodCallParamsCode)
		{
			Source = source;
			string origcode = addConnMethodCallParamsCode;

			addConnMethodCallParamsCode = addConnMethodCallParamsCode.Replace("addConnector(", "");
			addConnMethodCallParamsCode = addConnMethodCallParamsCode.Replace(")", "");
			addConnMethodCallParamsCode = addConnMethodCallParamsCode.Replace("\"", "");

			string[] pieces = addConnMethodCallParamsCode.Split(',');

			if (pieces.Length != 3)
				Lint.MsgErr("error parsing connector in source {0} - {1}", Source, origcode);
			else
			{
				SourceAction = pieces[0].Trim();
				Target = pieces[1].Trim();
				TargetAction = pieces[2].Trim();
			}
		}
	}
}
