using System;
using System.Collections.Generic;
using UnityEngine;

public class ShadowCastHandler : PathfindingSearchHandler
{
    private readonly Stack<ShadowCastRow> RowFrontier = new();
    private readonly Stack<ShadowCastCell> CellFrontier = new();
    
    public ShadowCastHandler(Func<IGridCell, bool> cellVisitCondition, Action<PathfindingCell> onCellValid, Action<IGridCell> onCellCleared) : 
        base(cellVisitCondition, onCellValid, onCellCleared) { }

    public async void GetVisibility(IGridCell start, int range)
    {
        ClearData();
        {
            UnityEditor.SceneView.duringSceneGui += OnScene;
            Origin = start;
        }
        HaveData = true;
        OnCellValid?.Invoke(new ShadowCastCell(start));
        ValidCells.Add(start);
        for (Octant octant = Octant.NE; octant <= Octant.NW; octant++)
        {
            var (depthDir, scanDir) = GetDirectionsForOctant(octant);
            
            if (start.TryGetNeighbor(depthDir, out var next))
            {
                var origin = new ShadowCastCell(next, 1, 0, scanDir);
                var row = new ShadowCastRow(origin, 0.0f, 1.0f, depthDir);
                RowFrontier.Push(row);
            }

            while (RowFrontier.Count > 0)
            {
                ShadowCastRow row = RowFrontier.Pop();
                await ScanRow(row, range);
            }
        }
    }

    private async Awaitable ScanRow(ShadowCastRow row, int range)
    {
        ShadowCastCell cell = row.RowOrigin;
        CellFrontier.Push(cell);
        while (CellFrontier.Count > 0)
        {
            cell = CellFrontier.Pop();
            await ScanCell(row, cell, range);
        }
        
        if (cell.Visible == Visibility.Transparent)
            TryEnqueueNextRow(row, row.MinSlope, row.MaxSlope, range);
    }

    private async Awaitable ScanCell(ShadowCastRow currentRow, ShadowCastCell currentCell, int range)
    {
        {
            CurrentRow = currentRow;
            DepthDirection = currentRow.Direction;
            ScanDirection = currentCell.Direction;
            CurrentCell = currentCell;
            await System.Threading.Tasks.Task.Delay(1000);
        }

        float enterSlope = (currentCell.Column - 0.5f) / (currentCell.Depth + 0.5f);
        float exitSlope = (currentCell.Column + 0.5f) / (currentCell.Depth - 0.5f);

        bool underSlope = exitSlope < currentRow.MinSlope;
        bool overSlope = enterSlope > currentRow.MaxSlope;

        if (underSlope)
        {
            TryEnqueueNextCell(currentCell, currentRow.MinSlope, currentRow.MaxSlope, range);
            return;
        }

        bool visible = !overSlope && (CellVisitCondition?.Invoke(currentCell.Cell) ?? true);
        if (visible)
        {
            currentCell.Visible = Visibility.Transparent;
            OnCellValid?.Invoke(currentCell);
            ValidCells.Add(currentCell.Cell);
            await System.Threading.Tasks.Task.Delay(500);
        }
        else
        {
            currentCell.Visible = Visibility.Opaque;
            if (currentCell.PreviousVisible == Visibility.Transparent)
                TryEnqueueNextRow(currentRow, currentRow.MinSlope, Math.Min(currentRow.MaxSlope, enterSlope), range);
            
            currentRow.MinSlope = Math.Max(currentRow.MinSlope, exitSlope);
            currentRow.RowOrigin = currentCell;
        }

        if (overSlope) return;
        TryEnqueueNextCell(currentCell, currentRow.MinSlope, currentRow.MaxSlope, range);
    }

    void TryEnqueueNextRow(ShadowCastRow currentRow, float minSlope, float maxSlope, int range)
    {
        if (currentRow.RowOrigin.Distance >= range ||
            minSlope >= maxSlope ||
            !currentRow.TryGetNext(out var newRow)) return;
        newRow.MinSlope = minSlope;
        newRow.MaxSlope = maxSlope;
        RowFrontier.Push(newRow);
    }
    void TryEnqueueNextCell(ShadowCastCell currentCell, float minSlope, float maxSlope, int range)
    {
        if (currentCell.Column >= currentCell.Depth ||
            currentCell.Distance >= range ||
            minSlope >= maxSlope ||
            !currentCell.TryGetNext(out var newCell)) return;
        CellFrontier.Push(newCell);
    }

    private (CellDirection depthDir, CellDirection scanDir) GetDirectionsForOctant(Octant octant)
    {
        return octant switch
        {
            Octant.NE => (CellDirection.N, CellDirection.E),
            Octant.EN => (CellDirection.E, CellDirection.N),
            Octant.ES => (CellDirection.E, CellDirection.S),
            Octant.SE => (CellDirection.S, CellDirection.E),
            Octant.SW => (CellDirection.S, CellDirection.W),
            Octant.WS => (CellDirection.W, CellDirection.S),
            Octant.WN => (CellDirection.W, CellDirection.N),
            Octant.NW => (CellDirection.N, CellDirection.W),
            _ => throw new ArgumentOutOfRangeException(nameof(octant), octant, null)
        };
    }

    public override void ClearData()
    {
        base.ClearData();
        RowFrontier.Clear();
        CellFrontier.Clear();
        UnityEditor.SceneView.duringSceneGui -= OnScene;
    }

    #region DEBUG

    private ShadowCastRow CurrentRow;
    private ShadowCastCell _CurrentCell;
    private ShadowCastCell CurrentCell
    {
        get => _CurrentCell;
        set { _CurrentCell = value; UnityEditor.SceneView.RepaintAll(); }
    }

    private IGridCell Origin;
    private CellDirection ScanDirection;
    private CellDirection DepthDirection;

    private void OnScene(UnityEditor.SceneView obj)
    {
        if (CurrentCell == null) return;
        
        IGridCell originCell = Origin;
        IGridCell currentCell = CurrentCell.Cell;
        float minSlope = CurrentRow.MinSlope;
        float maxSlope = CurrentRow.MaxSlope;
        CellDirection depthDirection = DepthDirection;
        CellDirection scanDirection = ScanDirection;
        float tileSize = 1f; // change if your tiles have different world size

        if (originCell == null || currentCell == null) return;

        Vector3 originPos = originCell.Transform.position;
        Vector3 currentPos = currentCell.Transform.position;

        // Convert grid directions to world-space unit vectors (change axes as your grid uses)
        Vector3 depthVec = DirectionToVector(depthDirection); // should be unit length
        Vector3 scanVec = DirectionToVector(scanDirection); // should be unit length

        // Vector from origin to current cell, in world units
        Vector3 v = currentPos - originPos;

        // Project v onto depth/scan axes to get depth and column in tiles
        float depth = CurrentCell.Depth;
        float col = CurrentCell.Column;

        // guard: if depth is very small, don't attempt right-slope formula (avoid division by zero)
        if (depth <= 0.001f) return;

        // compute the tile's left/right slopes (the formulas you already used)
        float leftSlope = (col - 0.5f) / (depth + 0.5f);
        float rightSlope = (depth - 0.5f) > 0f ? (col + 0.5f) / (depth - 0.5f) : float.PositiveInfinity;

        // choose a visualization length (how far to draw the lines)
        float visualDepth = Mathf.Max(1.0f, depth + col); // draw at least one tile beyond
        float scale = visualDepth * tileSize;

        // compute world-space directions for the left/right edges
        // direction = depthVec + scanVec * slope (we normalize to draw nicely)
        Vector3 leftDir = (depthVec + scanVec * leftSlope).normalized;
        Vector3 rightDir = (depthVec + scanVec * rightSlope).normalized;

        // compute intersection points at roughly the same depth distance for clearer visualization
        Vector3 leftPoint = originPos + leftDir * scale;
        Vector3 rightPoint = originPos + rightDir * scale;

        // draw the wedge bounds (min/max slope) if you want to visualize the wedge too
        Vector3 minDir = (depthVec + scanVec * minSlope).normalized;
        Vector3 maxDir = (depthVec + scanVec * maxSlope).normalized;
        Vector3 minPoint = originPos + minDir * 1000;
        Vector3 maxPoint = originPos + maxDir * 1000;

        // draw lines and markers
        UnityEditor.Handles.color = Color.cyan;
        UnityEditor.Handles.DrawLine(originPos, leftPoint, 1); // left edge line
        UnityEditor.Handles.DrawLine(originPos, rightPoint, 1); // right edge line

        UnityEditor.Handles.color = Color.cyan;
        UnityEditor.Handles.DrawSolidDisc(leftPoint, Vector3.up, 0.06f * tileSize); // marker at left
        UnityEditor.Handles.DrawSolidDisc(rightPoint, Vector3.up, 0.06f * tileSize); // marker at right

        // optionally draw the current wedge min/max too

        UnityEditor.Handles.color = Color.red;
        UnityEditor.Handles.DrawLine(originPos, minPoint, 2);
        UnityEditor.Handles.color = Color.blue;
        UnityEditor.Handles.DrawLine(originPos, maxPoint, 2);
        UnityEditor.Handles.color = GetColorForOctant(DepthDirection, ScanDirection);
        UnityEditor.Handles.DrawAAConvexPolygon(originPos, minPoint, maxPoint);

        // draw a small label with numeric info
        UnityEditor.Handles.color = Color.white;
        UnityEditor.Handles.Label(currentPos + Vector3.up * 0.1f, $"d={depth:F2} c={col:F2}\nL={leftSlope:F2} R={rightSlope:F2}");
        UnityEditor.Handles.Label(originPos + Vector3.up * 0.1f, $"MIN={minSlope:F2} MAX={maxSlope:F2}");

        // Optionally draw the current tile rectangle (helps see where edges land)
        DrawTileRect(currentPos, tileSize);
    }

    private Color GetColorForOctant(CellDirection depthDirection, CellDirection scanDirection)
    {
        return depthDirection switch
        {
            CellDirection.N when scanDirection == CellDirection.W => new Color(0.50f, 0.75f, 0.50f, 0.5f),
            CellDirection.N when scanDirection == CellDirection.E => new Color(0.50f, 0.50f, 1.00f, 0.5f),
            CellDirection.E when scanDirection == CellDirection.N => new Color(0.50f, 1.00f, 0.50f, 0.5f),
            CellDirection.E when scanDirection == CellDirection.S => new Color(1.00f, 0.50f, 0.50f, 0.5f),
            CellDirection.S when scanDirection == CellDirection.E => new Color(1.00f, 1.00f, 0.50f, 0.5f),
            CellDirection.S when scanDirection == CellDirection.W => new Color(1.00f, 0.50f, 1.00f, 0.5f),
            CellDirection.W when scanDirection == CellDirection.S => new Color(0.50f, 1.00f, 1.00f, 0.5f),
            CellDirection.W when scanDirection == CellDirection.N => new Color(1.00f, 0.75f, 0.50f, 0.5f),
            _ => Color.black
        };
    }
    private Vector3 DirectionToVector(CellDirection dir)
    {
        // Adjust this mapping to your world axes; here +X is East, +Z is North (common Unity layout)
        return dir switch
        {
            CellDirection.N => Vector3.forward,
            CellDirection.S => Vector3.back,
            CellDirection.E => Vector3.right,
            CellDirection.W => Vector3.left,
            _ => Vector3.zero
        };
    }
    private void DrawTileRect(Vector3 center, float size)
    {
        Vector3 half = new Vector3(size * 0.5f, 0f, size * 0.5f);
        Vector3 bl = center - half; // bottom-left (in XZ)
        Vector3 br = center + new Vector3(half.x, 0, -half.z);
        Vector3 tr = center + half;
        Vector3 tl = center + new Vector3(-half.x, 0, half.z);

        UnityEditor.Handles.DrawSolidRectangleWithOutline(new[] { bl, br, tr, tl }, new Color(0, 1, 0, 0.5f), Color.green);
    }

    #endregion
}