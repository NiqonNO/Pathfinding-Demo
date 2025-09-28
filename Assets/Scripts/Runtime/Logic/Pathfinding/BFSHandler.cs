using System;
using System.Collections.Generic;

public abstract class BFSHandler
{
    public readonly HashSet<IGridCell> VisitedCells = new();
    private readonly Queue<IGridCell> Frontier = new();

    protected bool HaveData;

    public void GetRange_BFS(IGridCell startCell, int range)
    {
        ClearData();
        
        HaveData = true;
        Frontier.Enqueue(startCell);
        VisitedCells.Add(startCell);

        while (Frontier.Count > 0)
        {
            IGridCell current = Frontier.Dequeue();
            if (GetCellDistance(current) >= range) continue;
            for (CellDirection direction = CellDirection.N; direction <= CellDirection.W; direction++)
            {
                CheckNeighbors(current, direction);
            }
        }
    }
    void CheckNeighbors(IGridCell current, CellDirection direction)
    {
        if (!current.TryGetNeighbor(direction, out var neighbor) ||
            VisitedCells.Contains(neighbor) ||
            !CellVisitCondition(neighbor)) return;
                
        SetCellDistance(neighbor, GetCellDistance(current) + 1);
        OnCellVisited(neighbor);

        Frontier.Enqueue(neighbor);
        VisitedCells.Add(neighbor);
    }
    
    public virtual void ClearData()
    {
        if (!HaveData) return;
        
        foreach (var cells in VisitedCells)
        {
            OnCellCleared(cells);
        }

        VisitedCells.Clear();
        HaveData = false;
    }

    protected abstract bool CellVisitCondition(IGridCell cell);
    protected abstract void OnCellVisited(IGridCell cell);
    protected abstract void OnCellCleared(IGridCell cell);
    protected abstract int GetCellDistance(IGridCell cell);
    protected abstract void SetCellDistance(IGridCell cell, int distance);
}