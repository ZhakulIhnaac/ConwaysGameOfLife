using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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

		public T GetElementAt((int, int) index)
		{
			if (_listOfLists.Count < index.Item1 && _listOfLists[index.Item1].Count < index.Item2)
			{
				return _listOfLists[index.Item1][index.Item2];
			}
			
			throw new IndexOutOfRangeException();
		}

		public void SetElementAt((int,int) index, T newValue)
		{
			if (_listOfLists.Count < index.Item1 && _listOfLists[index.Item1].Count < index.Item2)
			{
				_listOfLists[index.Item1][index.Item2] = newValue;
				return;
			}

			throw new IndexOutOfRangeException();
		}

		public (int, int) GetIndexOf(T elementToCheck, Func<T, T, bool> compareFunc)
		{
			for (int i = 0; i < _listOfLists.Count; i++)
			{
				var iteratingList = _listOfLists[i];
				
				for (int j = 0; j < iteratingList.Count; j++)
				{
					if (compareFunc(elementToCheck, iteratingList[j]))
					{
						return (i, j);
					}	
				}
			}

			return (-1, -1);
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
