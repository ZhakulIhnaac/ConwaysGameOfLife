using System;
using System.IO;
using System.Text;
using DefaultNamespace;
using UnityEngine;

public class ConwaySolver : MonoBehaviour
{
	/*
	 * Project start time = 14:35 Lisbon time
	 * Algorithm completed time = 15:30 Lisbon time
	 * Fixes completed time = 16:40 Lisbon time
	 * End time = 17:27 Lisbon time
	 *
	 * Rule:
	 * Imagine a 2D grid, each cell (coordinate) can be “alive” or “dead”.
	 * Every generation of the simulation, the system ticks forward.
	 * Each cell's value changes according to the following:
	 * If an “alive” cell has less than 2 or more than 3 alive neighbours ( in any of the 8 surrounding cells), it becomes dead.
	 * If a “dead” cell had "exactly" 3 alive neighbours, it becomes alive.
	 *
	 * Assumptions:
	 * - The input file is in the Life 1.06 format.
	 * - The input coordinates are in range of signed int64
	 * - The input coordinates has no invalid strings, all of them are valid int64s.
	 */

	private const string _inputTextFileAddress = "input.txt";
	private const string _outputTextFileAddress = "output.txt";
	private const int _generationCount = 1;

	private readonly List64<Tuple<long, long>> _aliveCandidateCells = new();
	private readonly List64<int> _aliveCandidateCellAliveNeighbourCount = new();
	private readonly List64<Tuple<long, long>> _cellsToSetDead = new();
	private readonly StringBuilder _outputText = new();
	private readonly Tuple<long, long>[] _relativeNeighbourCoordinates =
	{
		new(-1, -1),
		new(-1, 0),
		new(-1, 1),
		new(0, 1),
		new(1, 1),
		new(1, 0),
		new(1, -1),
		new(0, -1)
	};

	private List64<Tuple<long, long>> _aliveCells;
	
	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space)) Solve();
	}

	private void Solve()
	{
		var startTime = DateTime.Now;
		Debug.Log($"Solving started at {startTime}");

		LoadInputCells();

		for (int i = 0; i < _generationCount; i++)
		{
			IterateTheGameToTheNextGeneration();
		}

		WriteOutputCells();

		var endTime = DateTime.Now;
		Debug.Log($"Solving completed at {endTime}, with total time of {(endTime - startTime).TotalSeconds} seconds");
	}

	private void IterateTheGameToTheNextGeneration()
	{
		_aliveCandidateCells.Clear();
		_aliveCandidateCellAliveNeighbourCount.Clear();
		_cellsToSetDead.Clear();
		
		foreach (var iteratingAliveCell in _aliveCells)
		{
			var aliveNeighbourCounts = GetAliveNeighbourCountsOfCell(iteratingAliveCell);

			if (aliveNeighbourCounts is < 2 or > 3)
			{
				_cellsToSetDead.Add(iteratingAliveCell);
			}
		}

		_aliveCells.Remove(_cellsToSetDead);

		while (!_aliveCandidateCells.IsEmpty)
		{
			if (_aliveCandidateCellAliveNeighbourCount.LastElement is 3)
			{
				_aliveCells.Add(_aliveCandidateCells.LastElement);
			}

			_aliveCandidateCells.RemoveLastElement();
			_aliveCandidateCellAliveNeighbourCount.RemoveLastElement();
		}
	}

	#region Utils
	private int GetAliveNeighbourCountsOfCell(Tuple<long, long> cellCoordinates)
	{
		int aliveNeighbourCount = 0;

		foreach (var iteratingRelativeNeighbourCoordinate in _relativeNeighbourCoordinates)
		{
			try
			{
				var neighbourCellCoordinates = new Tuple<long, long>(cellCoordinates.Item1 + iteratingRelativeNeighbourCoordinate.Item1, cellCoordinates.Item2 + iteratingRelativeNeighbourCoordinate.Item2);

				if (_aliveCells.Contains(neighbourCellCoordinates))
				{
					aliveNeighbourCount++;
				}
				else
				{
					if (!_aliveCandidateCells.Contains(neighbourCellCoordinates))
					{
						_aliveCandidateCells.Add(neighbourCellCoordinates);
						_aliveCandidateCellAliveNeighbourCount.Add(1);
					}
					else
					{
						_aliveCandidateCellAliveNeighbourCount.SetElementAt(_aliveCandidateCells.GetIndexOf(neighbourCellCoordinates, TupleCompareFunction), _aliveCandidateCellAliveNeighbourCount.GetElementAt(_aliveCandidateCells.GetIndexOf(neighbourCellCoordinates, TupleCompareFunction)) + 1);
					}
				}
			}
			catch (Exception e)
			{
				//Out of int64 range, out of the board. We will skip this coordinate since there is no cell at this position.
			}
		}

		return aliveNeighbourCount;
	}

	private bool TupleCompareFunction(Tuple<long, long> lhs, Tuple<long, long> rhs)
	{
		return lhs.Item1 == rhs.Item1 && lhs.Item2 == rhs.Item2;
	}

	private void LoadInputCells()
	{
		string word = File.ReadAllText($"{Application.dataPath}/{_inputTextFileAddress}");
		string[] words = word.Split('\n');

		_aliveCells = new List64<Tuple<long, long>>();

		// Starting from the second line since the first line is the term "#Life 1.06"
		for (long i = 1; i < words.Length; i++)
		{
			if (string.IsNullOrEmpty(words[i])) continue; // Skip the empty lines. Mostly the last line can be empty.
			_aliveCells.Add(GameOfLifeFormatCoordinateStringToCoordinates(words[i]));
		}

		Tuple<long, long> GameOfLifeFormatCoordinateStringToCoordinates(string stringCoordinate)
		{
			var splitCoordinates = stringCoordinate.Split(' ');
			return new Tuple<long, long>(long.Parse(splitCoordinates[0]), long.Parse(splitCoordinates[1]));
		}
	}

	private void WriteOutputCells()
	{
		_outputText.Clear();

		_outputText.Append("#Life 1.06");

		foreach (var iteratingAliveCell in _aliveCells)
		{
			_outputText.Append($"\n{iteratingAliveCell.Item1} {iteratingAliveCell.Item2}");
		}

		File.WriteAllText($"{Application.dataPath}/{_outputTextFileAddress}", _outputText.ToString());
	}
	#endregion
}
