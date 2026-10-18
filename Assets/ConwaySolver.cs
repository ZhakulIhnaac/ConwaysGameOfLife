using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using DefaultNamespace;
using UnityEngine;

public class ConwaySolver : MonoBehaviour
{
	/*
	 * Project start time = 14:35 Lisbon time
	 * Algorithm completed time = 15:30 Lisbon time
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
	private const int _generationCount = 10;

	private readonly Dictionary<Tuple<long, long>, int> _aliveCandidateCells = new();
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
		
		foreach (var iteratingAliveCandidateCell in _aliveCandidateCells)
		{
			if (iteratingAliveCandidateCell.Value is 3)
			{
				_aliveCells.Add(iteratingAliveCandidateCell.Key);
			}
		}
	}

	#region Utils
	private int GetAliveNeighbourCountsOfCell(Tuple<long, long> cellCoordinates)
	{
		int neighbourCount = 0;

		foreach (var iteratingRelativeNeighbourCoordinate in _relativeNeighbourCoordinates)
		{
			try
			{
				var neighbourCellCoordinates = new Tuple<long, long>(cellCoordinates.Item1 + iteratingRelativeNeighbourCoordinate.Item1, cellCoordinates.Item2 + iteratingRelativeNeighbourCoordinate.Item2);

				if (_aliveCells.Contains(neighbourCellCoordinates))
				{
					neighbourCount++;
				}
			}
			catch (Exception e)
			{
				//Out of int64 range, out of the board. We will skip this coordinate since there is no cell at this position.
			}
		}

		return neighbourCount;
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
