using System;
using System.Collections.Generic;
using UnityEngine;

public class AStarHandler
{
    public readonly HashSet<IGridCell> VisitedCells = new();
    private readonly PriorityQueue<IGridCell> Frontier = new();
    
    public List<IGridCell> Path { get; private set; } = new();
    public bool FoundPath { get; private set; }
    public bool HaveValidPath { get; private set; }
    public bool OutOfRange { get; private set; }

    private bool HaveData;
    
    public void FindPath_AStar(IGridCell startCell, IGridCell endCell)
    {
        ClearData();
        
        HaveData = true;
        Frontier.Clear();
        startCell.PathfindingData.MovePathData.Distance = 0;
        startCell.PathfindingData.MovePathData.Visited = true;
        startCell.PathfindingData.MovePathData.Estimation = EuclideanDistance(startCell, endCell);
        Frontier.Enqueue(startCell, startCell.PathfindingData.MovePathData.Estimation);
        VisitedCells.Add(startCell);

        while (Frontier.Count > 0)
        {
            IGridCell current = Frontier.Dequeue();
            if (current == endCell)
            {
                FoundPath = true;
                return;
            }

            for (CellDirection direction = CellDirection.N; direction <= CellDirection.W; direction++)
            {
                CheckNeighbours(current, endCell, direction);
            }
        }
    }
    void CheckNeighbours(IGridCell current, IGridCell endCell, CellDirection direction)
    {
        if (!current.TryGetNeighbor(direction, out var neighbor) ||
            neighbor.CellType != CellType.Traversable ||
            neighbor.Occupied)
            return;

        int tentativeG = current.PathfindingData.MovePathData.Distance + 1;
        if (tentativeG >= neighbor.PathfindingData.MovePathData.Distance) return;

        neighbor.PathfindingData.MovePathData.Previous = current;
        neighbor.PathfindingData.MovePathData.Distance = tentativeG;
        neighbor.PathfindingData.MovePathData.Visited = true;
        neighbor.PathfindingData.MovePathData.Estimation = tentativeG + EuclideanDistance(neighbor, endCell);

        if (Frontier.Contains(neighbor))
        {
            Frontier.UpdatePriority(neighbor, neighbor.PathfindingData.MovePathData.Estimation);
            return;
        }

        Frontier.Enqueue(neighbor, neighbor.PathfindingData.MovePathData.Estimation);
        VisitedCells.Add(neighbor);
    }
    
    public void ClearData()
    {
        if (!HaveData) return;

        foreach (var cells in VisitedCells)
        {
            cells.PathfindingData.ClearMovementData();
        }

        VisitedCells.Clear();
        Path.Clear();
        HaveData = false;
        FoundPath = false;
        HaveValidPath = false;
        OutOfRange = false;
    }
    
    public void ReconstructPath(IGridCell endCell, int range = int.MaxValue)
    {
        var current = endCell;
        while (current != null)
        {
            if (current.PathfindingData.MovePathData.Distance <= range)
            {
                Path.Add(current);
            }
            else
            {
                current.PathfindingData.MovePathData.IsOutOfRange = true;
                OutOfRange = true;
            }
            current.PathfindingData.IsMovementPath = true;
            current = current.PathfindingData.MovePathData.Previous;
        }

        HaveValidPath = true;
        Path.Reverse();
    }
    
    private int EuclideanDistance(IGridCell startCell, IGridCell endCell)
    {
        int dx = Mathf.Abs(startCell.CellCoordinates.x - endCell.CellCoordinates.x);
        int dy = Mathf.Abs(startCell.CellCoordinates.y - endCell.CellCoordinates.y);
        return dx + dy;
    }
}