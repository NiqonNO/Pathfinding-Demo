using System;
using System.Collections.Generic;
using UnityEngine;

public class ShadowCastHandler : PathfindingSearchHandler<ShadowCastCellData>
{
    private readonly Stack<ShadowCastCellData> RowFrontier = new();
    private readonly Stack<ShadowCastCellData> CellFrontier = new();

    public void GetVisibility(CellData startCell, int range)
    {
        ClearData();
        HaveData = true;
        var originCell = startCell.AttackRangeData;
        VisitedCells.Add(originCell);
        originCell.Display();
        
        for (Octant octant = Octant.NE; octant <= Octant.NW; octant++)
        {
            originCell.GetDirectionsForOctant(octant);
            if (originCell.TryGetNextRow(out var rowOrigin))
            {
                RowFrontier.Push(rowOrigin);
            }

            while (RowFrontier.Count > 0)
            {
                ShadowCastCellData row = RowFrontier.Pop();
                ScanRow(row, range);
            }
        }
    }

    private void ScanRow(ShadowCastCellData rowOrigin, int range)
    {
        ShadowCastCellData cellData = rowOrigin;
        CellFrontier.Push(rowOrigin);
        VisitedCells.Add(rowOrigin);
        
        while (CellFrontier.Count > 0)
        {
            cellData = CellFrontier.Pop();
            ScanCell(cellData, range);
        }
        
        if (cellData.Visible != Visibility.Opaque)
        {
            TryEnqueueNextRow(cellData.RowOrigin, cellData.MinSlope, cellData.MaxSlope, range);
        }
    }

    private void ScanCell(ShadowCastCellData currentCellData, int range)
    {
        float enterSlope = (currentCellData.Column - 0.5f) / (currentCellData.Depth + 0.5f);
        float exitSlope = (currentCellData.Column + 0.5f) / (currentCellData.Depth - 0.5f);

        bool underSlope = exitSlope < currentCellData.MinSlope;
        bool overSlope = enterSlope > currentCellData.MaxSlope;

        if (underSlope)
        {
            TryEnqueueNextCell(currentCellData, currentCellData.MinSlope, currentCellData.MaxSlope, range);
            return;
        }
        if (overSlope) return;

        bool visible = currentCellData.IsVisible();
        if (visible)
        {
            currentCellData.Visible = Visibility.Transparent;
            currentCellData.Display();
            
            if(currentCellData.PreviousVisible == Visibility.Opaque)
                currentCellData.RowOrigin = currentCellData;
        }
        else
        {
            currentCellData.Visible = Visibility.Opaque;
            
            if (currentCellData.PreviousVisible == Visibility.Transparent)
                TryEnqueueNextRow(currentCellData.RowOrigin, currentCellData.MinSlope, Math.Min(currentCellData.MaxSlope, enterSlope), range);
            
            currentCellData.MinSlope = Math.Max(currentCellData.MinSlope, exitSlope);
        }
        
        TryEnqueueNextCell(currentCellData, currentCellData.MinSlope, currentCellData.MaxSlope, range);
    }

    void TryEnqueueNextRow(ShadowCastCellData current, float minSlope, float maxSlope, int range)
    {
        if (current.Distance >= range ||
            minSlope >= maxSlope ||
            !current.TryGetNextRow(out var newRow)) return;
        newRow.MinSlope = minSlope;
        newRow.MaxSlope = maxSlope;
        RowFrontier.Push(newRow);
    }
    void TryEnqueueNextCell(ShadowCastCellData current, float minSlope, float maxSlope, int range)
    {
        if (current.Column >= current.Depth ||
            current.Distance >= range ||
            minSlope >= maxSlope ||
            !current.TryGetNextCell(out var newCell)) return;
        CellFrontier.Push(newCell);
        VisitedCells.Add(newCell);
    }

    public override void ClearData()
    {
        base.ClearData();
        RowFrontier.Clear();
        CellFrontier.Clear();
    }
}