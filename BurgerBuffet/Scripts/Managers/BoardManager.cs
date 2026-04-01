using Godot;
using System;
using System.Collections.Generic;

public partial class BoardManager : Node2D
{
    public static BoardManager Instance { get; private set; }
    public List<(int, int)> _occupiedSquares = new List<(int, int)>();
    public List<(int, int)> _occupiedMeanieSquares = new List<(int, int)>();
    private int FLOORSIZE_X = 16, FLOORSIZE_Y = 14;
    private (int, int) _boardSquare;
    private bool _validSquare;
    public override void _EnterTree()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            QueueFree();
        }
    }

    public override void _Ready()
    {
        GlobalSignals.Instance.RestartGame += ResetOccupiedSquares;
    }

    private void ResetOccupiedSquares()
    {
        _occupiedSquares.Clear();
        _occupiedMeanieSquares.Clear();
    }


    public (int, int) OccupyRandomAvailableBoardSquare(bool isMeanie)
    {
        int randX = GD.RandRange(0, FLOORSIZE_X - 1);
        int randY = GD.RandRange(0, FLOORSIZE_Y - 1);

        _validSquare = true;
        for (int i = 0; i < _occupiedSquares.Count; i++)
        {
            if (_occupiedSquares[i].Item1 == randX && _occupiedSquares[i].Item2 == randY)
            {
                _validSquare = false;
            }
        }

        if (!_validSquare)
        {
            return OccupyRandomAvailableBoardSquare(isMeanie);
        }
        else
        {
            _boardSquare = (randX, randY);
            if (isMeanie) { _occupiedMeanieSquares.Add(_boardSquare);}
            _occupiedSquares.Add(_boardSquare);
            return _boardSquare;
        }
    }


    public (int, int) FindAvailableSquareForMeanie((int, int) square)
    {
        List<(int, int)> candidates = new()
    {
        (square.Item1, square.Item2 - 1), // up
        (square.Item1, square.Item2 + 1), // down
        (square.Item1 - 1, square.Item2), // left
        (square.Item1 + 1, square.Item2)  // right
    };

        List<(int, int)> validSquares = new();

        foreach (var candidate in candidates)
        {
            bool inBounds =
                candidate.Item1 >= 0 &&
                candidate.Item1 < FLOORSIZE_X &&
                candidate.Item2 >= 0 &&
                candidate.Item2 < FLOORSIZE_Y;

            bool occupied = _occupiedMeanieSquares.Contains(candidate);

            if (inBounds && !occupied)
            {
                validSquares.Add(candidate);
            }
        }

        if (validSquares.Count == 0)
        {
            // No valid adjacent square exists
            return square; // or return (-1, -1), depending on your design
        }

        int randIndex = GD.RandRange(0, validSquares.Count - 1);
        return validSquares[randIndex];
    }

}