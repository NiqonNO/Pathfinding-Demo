using System;
using System.Collections.Generic;
using UnityEngine;

public class BresenhamsLineHandler
{
    public HashSet<IGridCell> Line { get; private set; } = new();
    public bool FoundLine { get; private set; }

    public void FindLine_BresenhamsLine(CellData startCell, CellData targetCell)
    {
        ClearData();
        
        if (startCell.Equals(targetCell)) return;

        int x0 = startCell.CellCoordinates.x;
        int y0 = startCell.CellCoordinates.y;
        int x1 = targetCell.CellCoordinates.x;
        int y1 = targetCell.CellCoordinates.y;

        CellData currentCell = startCell;

        bool steep = Math.Abs(y1 - y0) > Math.Abs(x1 - x0);

        CellDirection ProgressDirection = CellDirection.E;
        CellDirection StepDirection = CellDirection.N;

        if (steep)
        {
            (x0, y0) = (y0, x0);
            (x1, y1) = (y1, x1);
            (ProgressDirection, StepDirection) = (StepDirection, ProgressDirection);
        }

        if (x0 > x1)
        {
            (x0, x1) = (x1, x0);
            (y0, y1) = (y1, y0);
            (currentCell, targetCell) = (targetCell, currentCell);
        }

        int dX = (x1 - x0);
        int dY = Math.Abs(y1 - y0);

        int err = (dX / 2);
        int ystep = (y0 < y1 ? 1 : -1);
        if (ystep < 0) StepDirection = StepDirection.Opposite();
        int y = y0;

        int x = x0;
        for (; x < x1; ++x)
        {
            if (StepX()) return;
        }

        FoundLine = true;
        return;

        bool StepX()
        {
            if (currentCell.CellType == CellType.Obstacle)
            {
                return true;
            }

            if (x != x0) Line.Add(currentCell.Cell);

            err -= dY;
            if(StepY()) return true;

            if (!currentCell.TryGetNeighbor(ProgressDirection, out currentCell))
            {
                Debug.LogError($"Null neighbour encountered when moving {ProgressDirection}", (UnityEngine.Object)currentCell.Cell);
                throw new NullReferenceException();
            }

            return false;
        }
        bool StepY()
        {
            if (err >= 0) return false;
            
            y += ystep;
            err += dX;

            if (!currentCell.TryGetNeighbor(StepDirection, out currentCell))
            {
                Debug.LogError($"Null neighbour encountered when moving {StepDirection}", (UnityEngine.Object)currentCell.Cell);
                throw new NullReferenceException();
            }

            if (currentCell.Equals(targetCell))
            {
                FoundLine = true;
                return true;
            }

            if (currentCell.CellType == CellType.Obstacle)
            {
                return true;
            }

            Line.Add(currentCell.Cell);
            return false;
        }
    }

    public void ClearData()
    {
        Line.Clear();
        FoundLine = false;
    }
}