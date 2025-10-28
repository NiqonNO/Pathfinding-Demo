using System;
using UnityEngine;

public class CellData
{
    private event Action<CellData> OnDataUpdate;
    private readonly CellData[] Neighbors = new CellData[4];
    
    public IGridCell Cell { get; }

    public BSFCellData MovementRangeData { get; }
    public AStarCellData MovementPathData { get; }
    public ShadowCastCellData AttackRangeData { get; }
    public BresenhamsLineCellData AttackPathData { get; }
    
    public CellType CellType { get; private set; }
    public Vector2Int CellCoordinates { get; private set; }
    
    public bool IsRange => MovementRangeData is { Active: true };
    public bool IsMovementPath => MovementPathData is { Active: true };
    public bool IsVisibility  => AttackRangeData is { Active: true };
    public bool IsAttack  => AttackPathData is { Active: true };

    public int MovementDistance =>
        IsRange ? MovementRangeData.Distance :
        IsMovementPath ? MovementPathData.Distance : 0;
    public int AttackDistance =>
        IsVisibility ? AttackRangeData.Distance :
        IsAttack ? AttackPathData.Distance : 0;
    
    public bool Occupied => Cell.Occupied;
    
    public CellData(IGridCell cell, Vector2Int coordinates, CellType cellType, Action<CellData> onUpdateData)
    {
        Cell = cell;
        CellCoordinates = coordinates;
        CellType = cellType;
        OnDataUpdate = onUpdateData;
        
        MovementRangeData = new BSFCellData(this);
        MovementPathData = new AStarCellData(this);
        AttackRangeData = new ShadowCastCellData(this);
        AttackPathData = new BresenhamsLineCellData(this);
    }

    public bool TryGetNeighbor(CellDirection direction, out CellData cell)
    {
        cell = Neighbors[(int)direction];
        return cell != null;
    }

    public void SetNeighbor(CellDirection direction, CellData cell)
    {
        Neighbors[(int)direction] = cell;
        cell.Neighbors[(int)direction.Opposite()] = this;
    }

    public void UpdateDisplay() => OnDataUpdate?.Invoke(this);
}