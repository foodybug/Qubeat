using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MultiSortedDictionary<TKey, TValue>
{
	public MultiSortedDictionary()
	{
		dic = new SortedDictionary<TKey, List<TValue>>();
	}

	public MultiSortedDictionary(IComparer<TKey> comparer)
	{
		dic = new SortedDictionary<TKey, List<TValue>>(comparer);
	}

	public void Add(TKey key, TValue value)
	{
		List<TValue> list;

		if (dic.TryGetValue(key, out list))
		{
			list.Add(value);
		}
		else
		{
			list = new List<TValue>();
			list.Add(value);

			dic.Add(key, list);
		}
	}
	
	public void Remove(TKey key, TValue value)
	{
		List<TValue> list;

		if (dic.TryGetValue(key, out list))
		{
			list.Remove(value);
		}
		else
		{
			Debug.LogError("[MultiDictionary]Remove: Invalid key access.");
		}
	}
	
	public bool ContainsKey(TKey _key)
	{
		if(dic.ContainsKey(_key) == true)
			return true;
		else
			return false;
	}
	
	public void Clear()
	{
		foreach(KeyValuePair<TKey, List<TValue>> pair in dic)
		{
			pair.Value.Clear();
		}
		
		dic.Clear();
	}

	public IEnumerable<TKey> Keys
	{
		get
		{
			return this.dic.Keys;
		}
	}

	public List<TValue> this[TKey key]
	{
		get
		{
			List<TValue> list;

			if (this.dic.TryGetValue(key, out list))
			{
				return list;
			}
			else
			{
				//return new List<TValue>();
//				Debug.LogWarning("[MultiSortedDictionary]no key exist");
				
				list = new List<TValue>();
				dic.Add(key, list);
				return list;
			}
		}
	}
	
	public MyEnumerator GetEnumerator() 
	{
		return new MyEnumerator(this);
	}
	
	public class MyEnumerator 
	{
		SortedDictionary<TKey, List<TValue>>.Enumerator iter;
		
//		MultiSortedDictionary<TKey, TValue> collection;
		public MyEnumerator(MultiSortedDictionary<TKey, TValue> coll) 
		{
			iter = coll.dic.GetEnumerator();
			//iter.Reset();
//			collection = coll;
		}
		
		public bool MoveNext()
		{
			return iter.MoveNext();
		}
		
		public KeyValuePair<TKey, List<TValue>> Current 
		{
			get 
			{
				return iter.Current;
			}
		}
	}

	private SortedDictionary<TKey, List<TValue>> dic = null;
}

