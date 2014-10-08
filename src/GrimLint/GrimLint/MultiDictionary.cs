using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GrimLint
{
	public class MultiDictionary<T1, T2> : Dictionary<T1, List<T2>>
	{
		public void AddMulti(T1 key, T2 value)
		{
			if (!ContainsKey(key))
				Add(key, new List<T2>());

			this[key].Add(value);
		}
	}

	public static class DictionaryExtMethods
	{
		public static V Find<K, V>(this Dictionary<K, V> dic, K key)
		{
			if (dic.ContainsKey(key))
				return dic[key];

			return default(V);
		}
	}
}
