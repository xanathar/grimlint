using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace GrimLint.Model
{
	public class Assets
	{
		Dictionary<string, Asset> m_Assets = new Dictionary<string, Asset>();
		Dictionary<DefinitionType, Dictionary<string, Definition>> m_Defs = new Dictionary<DefinitionType, Dictionary<string, Definition>>();

		public void LoadXml(string filename)
		{
			XmlDocument xdoc = new XmlDocument();
			xdoc.Load(filename);

			foreach (XmlElement xe in xdoc.SelectNodes("/*/o").OfType<XmlElement>())
			{
				Asset A = new Asset(xe, this);
				m_Assets[A.Name] = A;
			}
		}

		public void AddDef(DefinitionType type, Definition def)
		{
			if (!m_Defs.ContainsKey(type))
				m_Defs[type] = new Dictionary<string, Definition>();

			m_Defs[type][def.Name] = def;
		}

		public void CloneDef(DefinitionType type, Definition def)
		{
			if (!m_Defs.ContainsKey(type) || !m_Defs[type].ContainsKey(def.Name))
			{
				AddDef(type, def);
				return;
			}

			m_Defs[type][def.Name].Merge(def);
		}

		public Asset Get(string Name)
		{
			if (m_Assets.ContainsKey(Name))
				return m_Assets[Name];
			return null;
		}

		public IEnumerable<Definition> GetAllDefs(DefinitionType type)
		{
			if (m_Defs.ContainsKey(type))
				return m_Defs[type].Values;

			return new Definition[0];
		}


		public Definition GetDef(DefinitionType type, string name)
		{
			if (m_Defs.ContainsKey(type) && m_Defs[type].ContainsKey(name))
				return m_Defs[type][name];

			return null;
		}

		public void CreateAssetsFromDefs()
		{
			var defs = GetAllDefs(DefinitionType.Object);

			foreach (var def in defs)
			{
				string clss = def.Properties.Find("class") as string;
				string baseObject = def.Properties.Find("baseObject") as string;
				if (clss != null)
				{
					EntityClass ec;
					if (Enum.TryParse<EntityClass>(clss, out ec))
					{
						Asset A = new Asset()
						{
							Name = def.Name,
							Class = ec,
						};
						m_Assets[A.Name] = A;
					}
					else
					{
						Lint.MsgErr("[{0}]: Object {1} is defined with unknown class {2}", def.DeclaringFile, def.Name, clss);
					}
				}
				else if (baseObject != null)
				{
					Asset A = this.Get(baseObject);
					if (A == null)
					{
						Lint.MsgWarn("[{0}]: Object {1} is a clone of not found object {2}", def.DeclaringFile, def.Name, baseObject);
					}
					else
					{
						A = new Asset(A);
						A.Name = def.Name;
						m_Assets[A.Name] = A;
					}
				}
			}
		}

	}
}
