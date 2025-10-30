using System.Collections.Generic;
using UnityEngine;

public class AStarHandler : CellSearchHandler<IAStarData>
{
    private readonly PriorityQueue<IAStarData> Frontier = new();
    
    public List<ICellSearchData> Path { get; private set; } = new();
    public bool FoundPath { get; private set; }
    public bool HaveValidPath { get; private set; }
    public bool OutOfRange { get; private set; }
    
    public void FindPath(IAStarData startCell, IAStarData endCell)
    {
        ClearData();
        if (!endCell.IsTraversable()) return;
        HaveData = true;
        
        startCell.Distance = 0;
        startCell.Estimation = ManhattanDistance(startCell, endCell);
        
        VisitedCells.Add(startCell);
        Frontier.Enqueue(startCell, startCell.Estimation);

        while (Frontier.Count > 0)
        {
            IAStarData current = Frontier.Dequeue();
            if (current == endCell)
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
    private void ScanCell(IAStarData current, IAStarData endCell, CellDirection direction)
    {
        if (!current.TryGetNext(direction, out var neighbor) ||
            !neighbor.IsTraversable()) return;
        
        int tentativeG = current.Distance + 1;

        if (VisitedCells.Contains(neighbor) && 
            tentativeG >= neighbor.Distance) return;

        neighbor.Previous = current;
        neighbor.Distance = tentativeG;
        neighbor.Estimation = tentativeG + ManhattanDistance(neighbor, endCell);
        
        if (Frontier.Contains(neighbor))
        {
            Frontier.UpdatePriority(neighbor, neighbor.Estimation);
            return;
        }
        
        VisitedCells.Add(neighbor);
        Frontier.Enqueue(neighbor, neighbor.Estimation);
    }
    
    public void GetPath(IAStarData endCell, int range = int.MaxValue)
    {
        IAStarData current = endCell;
        if (!VisitedCells.Contains(current)) return;
        
        while (current != null)
        {
            if (current.Distance <= range)
            {
                Path.Add(current);
            }
            else
            {
                current.IsOutOfRange = true;
                OutOfRange = true;
            }
            
            OnValid(current);
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
    
    protected override void ValidateCell(IAStarData cell) => cell.OnValid();
    protected override void ClearCell(IAStarData cell) => cell.Clear();
    
    private int ManhattanDistance(IAStarData startCell, IAStarData endCell)
    {
        int dx = Mathf.Abs(startCell.X - endCell.X);
        int dy = Mathf.Abs(startCell.Y - endCell.Y);
        return dx + dy;
    }
}