using System;
using System.Collections.Generic;

public class BSFHandler : PathfindingSearchHandler
{
    private readonly Queue<BSFCell> Frontier = new();
    
    public BSFHandler(Func<IGridCell, bool> cellVisitCondition, Action<PathfindingCell> onCellValid, Action<IGridCell> onCellCleared) : 
        base(cellVisitCondition, onCellValid, onCellCleared) { }
    
    public void GetRange_BFS(IGridCell startCell, int range)
    {
        ClearData();
        
        HaveData = true;
        var originCell = new BSFCell(startCell)
        {
            Distance = 0
        };
        Frontier.Enqueue(originCell);
        ValidCells.Add(startCell);
        OnCellValid?.Invoke(originCell);

        while (Frontier.Count > 0)
        {
            BSFCell current = Frontier.Dequeue();
            if (current.Distance >= range) continue;
            for (CellDirection direction = CellDirection.N; direction <= CellDirection.W; direction++)
            {
                CheckNeighbors(current, direction);
            }
        }
    }
    void CheckNeighbors(BSFCell current, CellDirection direction)
    {
        if (!current.TryGetNext(direction, out var neighbor) ||
            ValidCells.Contains(neighbor.Cell) ||
            !(CellVisitCondition?.Invoke(neighbor.Cell)?? false)) return;

        Frontier.Enqueue(neighbor);
        ValidCells.Add(neighbor.Cell);
        OnCellValid?.Invoke(neighbor);
    }
    
    public override void ClearData()
    {
        base.ClearData();
        Frontier.Clear();
    }
}