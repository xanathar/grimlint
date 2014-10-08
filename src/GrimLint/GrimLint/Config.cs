using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace GrimLint
{
	public static class Config
	{
		static XmlDocument m_XmlDocument = new XmlDocument();

		public static void LoadXml(string filename)
		{
			m_XmlDocument.Load(filename);
		}

		public static bool IsRuleEnabled(string ruleName)
		{
			XmlElement xe = m_XmlDocument.SelectSingleNode(string.Format("/Config/Rules/{0}", ruleName)) as XmlElement;
			if (xe == null) return true;
			return (xe.GetAttribute("enabled") != "no");
		}

		public static Tuple<HashSet<string>, List<string>> GetEntitiesToIgnore(string ruleName)
		{
			XmlElement xe = m_XmlDocument.SelectSingleNode(string.Format("/Config/Rules/{0}", ruleName)) as XmlElement;
			if (xe == null) return new Tuple<HashSet<string>, List<string>>(new HashSet<string>(), new List<string>());
			string ignore = xe.GetAttribute("ignore");
			string[] toIgnore = ignore.Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
			return new Tuple<HashSet<string>, List<string>>(
				new HashSet<string>(toIgnore.Where(s => !s.Contains('*'))),
				new List<string>(toIgnore.Where(s => s.Contains('*') && (s.Length > 2))));
		}

		public static bool ShouldRuneIgnoreKnownNames(string ruleName)
		{
			XmlElement xe = m_XmlDocument.SelectSingleNode(string.Format("/Config/Rules/{0}", ruleName)) as XmlElement;
			if (xe == null) return false;
			return (xe.GetAttribute("ignoreKnownNames") == "yes");
		}

	}
}
