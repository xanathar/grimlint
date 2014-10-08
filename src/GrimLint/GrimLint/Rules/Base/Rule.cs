using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using GrimLint.Model;
using GrimLint.Rules;

namespace GrimLint.Rules.Base
{
	public abstract class Rule
	{
		HashSet<string> m_EntitiesToIgnore;
		List<string> m_EntitiesToIgnorePatterns;
		bool m_IgnoreKnownNames;

		protected Rule()
		{
			var ignoreData = Config.GetEntitiesToIgnore(this.GetRuleName());
			m_EntitiesToIgnore = ignoreData.Item1;
			m_EntitiesToIgnorePatterns = ignoreData.Item2;
			m_IgnoreKnownNames = Config.ShouldRuneIgnoreKnownNames(this.GetRuleName());
		}

		public string GetRuleName()
		{
			return this.GetType().Name;
		}

		public abstract void Run(Dungeon D);


		public void Info(Entity E, string format, params object[] args)
		{
			if (ShouldReport(E))
				Lint.MsgInfo("{0}-{1}: {2}", GetRuleName(), E, string.Format(format, args));
		}

		private bool ShouldReport(Entity E)
		{
			if (E.IsEmptyEntity())
				return true;

			if (m_IgnoreKnownNames && E.KnownName)
				return false;

			if (E.Id.EndsWith("_nolint"))
				return false;

			if (m_EntitiesToIgnore.Contains(E.Name))
				return false;

			foreach (string pattern in m_EntitiesToIgnorePatterns)
			{
				if (pattern.StartsWith("*") && pattern.EndsWith("*"))
				{
					string p = pattern.Substring(1, pattern.Length - 2);
					if (E.Name.Contains(p))
						return false;
				}
				else if (pattern.StartsWith("*") && E.Name.EndsWith(pattern.Substring(1)))
					return false;
				else if (pattern.EndsWith("*") && E.Name.EndsWith(pattern.Substring(0, pattern.Length - 1)))
					return false;
			}

			return true;
		}

		public void Error(Entity E, string format, params object[] args)
		{
			if (ShouldReport(E))
				Lint.MsgErr("{0}-{1}: {2}", GetRuleName(), E, string.Format(format, args));
		}

		public void Warning(Entity E, string format, params object[] args)
		{
			if (ShouldReport(E))
				Lint.MsgWarn("{0}-{1}: {2}", GetRuleName(), E, string.Format(format, args));
		}





	}
}
