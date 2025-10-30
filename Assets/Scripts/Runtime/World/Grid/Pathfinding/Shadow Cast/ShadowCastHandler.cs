using System;
using System.Collections.Generic;

public class ShadowCastHandler : CellSearchHandler<IShadowCastData>
{
    private readonly Stack<IShadowCastData> RowFrontier = new();
    private readonly Stack<IShadowCastData> CellFrontier = new();

    public void GetVisibility(IShadowCastData startCell, int range)
    {
        ClearData();
        HaveData = true;
        
        OnValid(startCell);
        VisitedCells.Add(startCell);
        
        for (OctantDirection octant = OctantDirection.NE; octant <= OctantDirection.NW; octant++)
        {
            (startCell.DepthDirection, startCell.ScanDirection) = octant.GetDirectionsForOctant();
            EnqueueNextRow(startCell, 0.0f, 1.0f, range);

            while (RowFrontier.Count > 0)
            {
                IShadowCastData row = RowFrontier.Pop();
                ScanRow(row, range);
            }
        }
    }

    private void ScanRow(IShadowCastData rowOrigin, int range)
    {
        IShadowCastData cellData = rowOrigin;
        CellFrontier.Push(rowOrigin);
        VisitedCells.Add(rowOrigin);
        
        while (CellFrontier.Count > 0)
        {
            cellData = CellFrontier.Pop();
            ScanCell(cellData, range);
        }
        
        if (cellData.Transparency != TransparencyState.Opaque)
        {
            EnqueueNextRow(cellData.RowOrigin, cellData.MinSlope, cellData.MaxSlope, range);
        }
    }

    private void ScanCell(IShadowCastData currentCellData, int range)
    {
        float enterSlope = (currentCellData.X - 0.5f) / (currentCellData.Y + 0.5f);
        float exitSlope = (currentCellData.X + 0.5f) / (currentCellData.Y - 0.5f);

        bool underSlope = exitSlope < currentCellData.MinSlope;
        bool overSlope = enterSlope > currentCellData.MaxSlope;

        if (underSlope)
        {
            EnqueueNextCell(currentCellData, currentCellData.MinSlope, currentCellData.MaxSlope, range);
            return;
        }

        if (overSlope) return;
        
        if (currentCellData.IsTransparent())
        {
            currentCellData.Transparency = TransparencyState.Transparent;
            
            float slope = ((float)currentCellData.X) / ((float)currentCellData.Y);
            currentCellData.Visible = (slope >= currentCellData.MinSlope && slope <= currentCellData.MaxSlope) ? 
                VisibilityState.Visible : VisibilityState.Partial;
            
            OnValid(currentCellData);
            
            if(currentCellData.PreviousTransparency == TransparencyState.Opaque)
                currentCellData.RowOrigin = currentCellData;
        }
        else
        {
            currentCellData.Transparency = TransparencyState.Opaque;
            currentCellData.Visible = VisibilityState.Obscured;
            
            if (currentCellData.PreviousTransparency == TransparencyState.Transparent)
                EnqueueNextRow(currentCellData.RowOrigin, currentCellData.MinSlope, Math.Min(currentCellData.MaxSlope, enterSlope), range);
            
            currentCellData.MinSlope = Math.Max(currentCellData.MinSlope, exitSlope);
        }
        
        EnqueueNextCell(currentCellData, currentCellData.MinSlope, currentCellData.MaxSlope, range);
    }

    void EnqueueNextRow(IShadowCastData current, float minSlope, float maxSlope, int range)
    {
        if (current.Distance >= range ||
            minSlope >= maxSlope ||
            !current.TryGetNext(current.DepthDirection, out var newRow)) return;
        
        newRow.DepthDirection = current.DepthDirection;
        newRow.ScanDirection = current.ScanDirection;
        newRow.Transparency = TransparencyState.Inapplicable;
        newRow.PreviousTransparency = TransparencyState.Inapplicable;
        newRow.X = current.X;
        newRow.Y = current.Y + 1;
        newRow.MinSlope = minSlope;
        newRow.MaxSlope = maxSlope;
        newRow.RowOrigin = newRow;
        
        RowFrontier.Push(newRow);
    }
    void EnqueueNextCell(IShadowCastData current, float minSlope, float maxSlope, int range)
    {
        if (current.X >= current.Y ||
            current.Distance >= range ||
            minSlope >= maxSlope ||
            !current.TryGetNext(current.ScanDirection, out var newCell)) return;
        
        newCell.DepthDirection = current.DepthDirection;
        newCell.ScanDirection = current.ScanDirection;
        newCell.Transparency = TransparencyState.Inapplicable;
        newCell.PreviousTransparency = current.Transparency;
        newCell.X = current.X + 1;
        newCell.Y = current.Y;
        newCell.MinSlope = minSlope;
        newCell.MaxSlope = maxSlope;
        newCell.RowOrigin = current.RowOrigin;
        
        VisitedCells.Add(newCell);
        CellFrontier.Push(newCell);
    }
    
    public override void ClearData()
    {
        base.ClearData();
        RowFrontier.Clear();
        CellFrontier.Clear();
    }

    protected override void ValidateCell(IShadowCastData cell) => cell.OnValid();
    protected override void ClearCell(IShadowCastData cell) => cell.Clear();
}