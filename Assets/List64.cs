using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DefaultNamespace
{
	public class List64<T> : IEnumerable<T>
	{
		public bool IsEmpty => _listOfLists.Count == 0;

		private readonly List<List<T>> _listOfLists = new();
		public T LastElement => IsEmpty ? default : _listOfLists[^1][_listOfLists[^1].Count - 1];
		
		public void Add(T value)
		{
			for (int i = 0; i < _listOfLists.Count; i++)
			{
				if (_listOfLists[i].Count < int.MaxValue)
				{
					_listOfLists[i].Add(value);
					return;
				}
			}

			var newList = new List<T>()
						  {
							  value
						  };
			
			_listOfLists.Add(newList);
		}

		public void Remove(T value)
		{
			for (int i = 0; i < _listOfLists.Count; i++)
			{
				if (_listOfLists[i].Contains(value))
				{
					_listOfLists[i].Remove(value);
					if (_listOfLists[i].Count == 0) _listOfLists.RemoveAt(i);
					return;
				}
			}
		}

		public void Remove(List64<T> listToRemove)
		{
			foreach (var elementToRemove in listToRemove)
			{
				Remove(elementToRemove);
			}
		}

		public void RemoveLastElement()
		{
			if (IsEmpty) return;
			Remove(_listOfLists[^1][_listOfLists[^1].Count - 1]);
		}
		
		public bool Contains(T value)
		{
			for (int i = 0; i < _listOfLists.Count; i++)
			{
				if (_listOfLists[i].Contains(value))
				{
					return true;
				}
			}

			return false;
		}
		
		public void Clear() => _listOfLists.Clear();

		public IEnumerator<T> GetEnumerator()
		{
			foreach (var iteratingList in _listOfLists)
			{
				foreach (var iteratingElement in iteratingList)
				{
					yield return iteratingElement;
				}
			}
		}
		
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}
