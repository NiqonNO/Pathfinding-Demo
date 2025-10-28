using System;
using System.Collections.Generic;
using UnityEngine;

public class AStarHandler : PathfindingSearchHandler<AStarCellData>
{
    private readonly PriorityQueue<AStarCellData> Frontier = new();
    
    public List<IGridCell> Path { get; private set; } = new();
    public bool FoundPath { get; private set; }
    public bool HaveValidPath { get; private set; }
    public bool OutOfRange { get; private set; }
    
    public void FindPath(CellData startCell, CellData endCell)
    {
        ClearData();
        
        HaveData = true;
        var originCell = startCell.MovementPathData;
        originCell.Distance = 0;
        originCell.Estimation = EuclideanDistance(startCell, endCell);
        Frontier.Enqueue(originCell, originCell.Estimation);
        VisitedCells.Add(originCell);

        while (Frontier.Count > 0)
        {
            AStarCellData current = Frontier.Dequeue();
            if (current.Cell == endCell)
            {
                FoundPath = true;
                return;
            }

            for (CellDirection direction = CellDirection.N; direction <= CellDirection.W; direction++)
            {
                ScanCell(current, endCell, direction);
            }
        }
    }
    private void ScanCell(AStarCellData current, CellData endCell, CellDirection direction)
    {
        if (!current.TryGetNext(direction, out var neighbor)) return;
        
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

        VisitedCells.Add(neighbor);
        Frontier.Enqueue(neighbor, neighbor.Estimation);
    }
    
    public void GetPath(CellData endCell, int range = int.MaxValue)
    {
        var current = endCell.MovementPathData;
        if (!VisitedCells.Contains(current)) return;
        
        while (current != null)
        {
            if (current.Distance <= range)
            {
                Path.Add(current.Cell.Cell);
            }
            else
            {
                current.IsOutOfRange = true;
                OutOfRange = true;
            }
            
            current.Display();
            current = current.Previous;
        }

        HaveValidPath = true;
        Path.Reverse();
    }
    
    public override void ClearData()
    {
        base.ClearData();
        Frontier.Clear();
        
        Path.Clear();
        FoundPath = false;
        HaveValidPath = false;
        OutOfRange = false;
    }
    
    private int EuclideanDistance(CellData startCell, CellData endCell)
    {
        int dx = Mathf.Abs(startCell.CellCoordinates.x - endCell.CellCoordinates.x);
        int dy = Mathf.Abs(startCell.CellCoordinates.y - endCell.CellCoordinates.y);
        return dx + dy;
    }
}