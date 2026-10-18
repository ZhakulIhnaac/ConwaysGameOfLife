using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DefaultNamespace
{
	public class List64<T> : IEnumerable<T>
	{
//		public long Count => _listOfLists.Sum(iteratingList => iteratingList.Count);
		
		private readonly List<List<T>> _listOfLists = new();

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
