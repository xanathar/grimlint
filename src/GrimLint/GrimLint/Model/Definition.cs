using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GrimLint.Model
{
	public class Definition
	{
		Dictionary<string, object> m_Properties;
		public string Name { get; private set; }
		public string DeclaringFile { get; private set; }

		public Definition(string file, Dictionary<string, object> props)
		{
			DeclaringFile = file;
			m_Properties = props;
			Name = m_Properties["name"] as string;	
		}

		public void Merge(Definition def)
		{
			foreach (KeyValuePair<string, object> kvp in def.m_Properties)
			{
				m_Properties[kvp.Key] = kvp.Value;
			}
		}

		public Dictionary<string, object> Properties
		{
			get { return m_Properties; }
		}


		public IEnumerable<object> FlattenedValues
		{
			get
			{
				return GetFlattened(m_Properties); 
			}
		}

		private IEnumerable<object> GetFlattened(Dictionary<string, object> dic)
		{
			foreach (object o in dic.Values)
			{
				if (o is Dictionary<string, object>)
				{
					foreach (object oo in GetFlattened((Dictionary<string, object>)o))
						yield return oo;
				}
				else
				{
					yield return o;
				}
			}
		}
	}
}
