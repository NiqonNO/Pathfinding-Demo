using System;
using System.Collections.Generic;

public class DDAHandler : CellSearchHandler<IDDAData>
{
    private readonly Queue<IDDAData> Frontier = new();
    
    public List<ICellSearchData> Path { get; } = new();
    public bool HaveValidPath { get; private set; }
    
    public void ShowPath(IDDAData startCell, IDDAData targetCell)
    {
        ClearData();
        HaveData = true;
        
        OnValid(startCell);
        VisitedCells.Add(startCell);
        Frontier.Enqueue(startCell);
        Path.Add(startCell);
        
        while (Frontier.Count > 0)
        {
            IDDAData current = Frontier.Dequeue();
            ScanCell(current, targetCell);
        }
    }

    private void ScanCell(IDDAData current, IDDAData target)
    {
        float xEdge = current.X + 0.5f;
        float yEdge = current.Y + 0.5f;

        float tMaxX = target.X == 0 ? float.MaxValue : 
            Math.Abs(xEdge / target.X);
        
        float tMaxY = target.Y == 0 ? float.MaxValue : 
            Math.Abs(yEdge / target.Y);
        
        CellDirection nextDirection = tMaxX < tMaxY ? 
            target.ScanDirection :
            target.DepthDirection;
        
        EnqueueNext(nextDirection, current, target);
    }

    private void EnqueueNext(CellDirection direction, IDDAData current, IDDAData target)
    {
        if (!current.TryGetNext(direction, out var next)) return;
        
        VisitedCells.Add(next);
        if (next == target)
        {
            HaveValidPath = true;
            Path.Reverse();
            return;
        }
        
        OnValid(next);
        Frontier.Enqueue(next);
        Path.Add(next);
    }

    public override void ClearData()
    {
        base.ClearData();
        
        Frontier.Clear();
        Path.Clear();
        HaveValidPath = false;
    }
    protected override void ValidateCell(IDDAData cell) => cell.OnValid();
    protected override void ClearCell(IDDAData cell) => cell.Clear();
}