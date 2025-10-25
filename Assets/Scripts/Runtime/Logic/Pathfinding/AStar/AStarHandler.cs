using System;
using System.Collections.Generic;
using UnityEngine;

public class AStarHandler : PathfindingSearchHandler
{
    private readonly PriorityQueue<AStarCell> Frontier = new();
    private readonly Dictionary<IGridCell, AStarCell> VisitedCells = new();
    
    public List<IGridCell> Path { get; private set; } = new();
    public bool FoundPath { get; private set; }
    public bool HaveValidPath { get; private set; }
    public bool OutOfRange { get; private set; }
    
    public AStarHandler(Func<IGridCell, bool> cellVisitCondition, Action<PathfindingCell> onCellValid, Action<IGridCell> onCellCleared) : 
        base(cellVisitCondition, onCellValid, onCellCleared) { }
    
    public void FindPath_AStar(IGridCell startCell, IGridCell endCell)
    {
        ClearData();
        
        HaveData = true;
        var originCell = new AStarCell(startCell)
        {
            Estimation = EuclideanDistance(startCell, endCell),
            Distance = 0
        };
        Frontier.Enqueue(originCell, originCell.Estimation);
        VisitedCells.Add(startCell, originCell);
        ValidCells.Add(startCell);

        while (Frontier.Count > 0)
        {
            AStarCell current = Frontier.Dequeue();
            if (current.Cell == endCell)
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
    private void CheckNeighbours(AStarCell current, IGridCell endCell, CellDirection direction)
    {
        if (!TryGetNext(current, direction, out var neighbor) ||
            !(CellVisitCondition?.Invoke(neighbor.Cell)?? false))
            return;
        
        int tentativeG = current.Distance + 1;
        if (tentativeG >= neighbor.Distance) return;

        neighbor.Previous = current;
        neighbor.Distance = tentativeG;
        neighbor.Estimation = tentativeG + EuclideanDistance(neighbor.Cell, endCell);

        if (Frontier.Contains(neighbor))
        {
            Frontier.UpdatePriority(neighbor, neighbor.Estimation);
            return;
        }

        Frontier.Enqueue(neighbor, neighbor.Estimation);
    }

    private bool TryGetNext(AStarCell current, CellDirection direction, out AStarCell newCell)
    {
        newCell = null;
        if (!current.Cell.TryGetNeighbor(direction, out var next)) return false;
        if (VisitedCells.TryGetValue(next, out var cell))
        {
            newCell = cell;
        }
        else
        {
            newCell = new AStarCell(next, current);
            VisitedCells.Add(next, newCell);
        }
        return true;
    }
    
    public override void ClearData()
    {
        base.ClearData();
        Frontier.Clear();
        VisitedCells.Clear();
        FoundPath = false;
        HaveValidPath = false;
        OutOfRange = false;
    }
    
    public void ReconstructPath(IGridCell endCell, int range = int.MaxValue)
    {
        if (!VisitedCells.TryGetValue(endCell, out var current)) return;
        while (current != null)
        {
            ValidCells.Add(current.Cell);
            if (current.Distance <= range)
            {
                Path.Add(current.Cell);
            }
            else
            {
                current.IsOutOfRange = true;
                OutOfRange = true;
            }
            
            OnCellValid?.Invoke(current);
            current = current.Previous;
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