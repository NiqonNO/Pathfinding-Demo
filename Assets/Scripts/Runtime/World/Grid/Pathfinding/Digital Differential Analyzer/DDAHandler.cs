using System;
using System.Collections.Generic;

public class DDAHandler : PathfindingSearchHandler<DDACellData>
{
    private readonly Queue<DDACellData> Frontier = new();
    
    public List<IGridCell> Path { get; private set; } = new();
    public bool HaveValidPath { get; private set; }
    
    public void ShowPath(CellData startCell, CellData targetCell)
    {
        ClearData();
        
        HaveData = true;
        var originCell = startCell.AttackPathData;
        originCell.SetInitialData(startCell.CellCoordinates.x, startCell.CellCoordinates.y, targetCell.CellCoordinates.x, targetCell.CellCoordinates.y);
        Frontier.Enqueue(originCell);
        VisitedCells.Add(originCell);
        originCell.Display();
        
        while (Frontier.Count > 0)
        {
            DDACellData current = Frontier.Dequeue();
            ScanCell(current);
        }
    }

    private void ScanCell(DDACellData current)
    {
        int stepX = Math.Sign(current.EndX - current.StartX);
        int stepY = Math.Sign(current.EndY - current.StartY);

        float xEdge = current.X + 0.5f * stepX;
        float yEdge = current.Y + 0.5f * stepY;

        float tMaxX = current.StartX == current.EndX ? float.MaxValue : 
            Math.Abs((xEdge - current.StartX) / (current.EndX - current.StartX));
        
        float tMaxY = current.StartY == current.EndY ? float.MaxValue : 
            Math.Abs((yEdge - current.StartY) / (current.EndY - current.StartY));
        
        if(tMaxX < tMaxY)
        {
            EnqueueNext(stepX > 0 ? CellDirection.E : CellDirection.W, current);
            return;
        }
        EnqueueNext(stepY > 0 ? CellDirection.N : CellDirection.S, current);
    }

    private void EnqueueNext(CellDirection direction, DDACellData current)
    {
        if (!current.TryGetNext(direction, out var next)) return;
            
        VisitedCells.Add(next);
        next.Display();
        if (next.X == next.EndX && next.Y == next.EndY)
        {
            HaveValidPath = true;
            return;
        }
        Frontier.Enqueue(next);
    }

    public override void ClearData()
    {
        base.ClearData();
        
        Frontier.Clear();
        Path.Clear();
        HaveValidPath = false;
    }
}