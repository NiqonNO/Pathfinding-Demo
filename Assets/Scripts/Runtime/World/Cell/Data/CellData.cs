using System;
using UnityEngine;

public class CellData
{
    private event Action<CellData> OnDataUpdate;
    private readonly CellData[] Neighbors = new CellData[4];
    
    public IGridCell GridObject { get; }

    public PathfindingData MovementData { get; }
    public VisibilityData AttackData { get; }
    
    public CellType CellType { get; private set; }
    public Vector2Int CellCoordinates { get; private set; }
    
    public bool Occupied => GridObject.Occupied;

    public CellData(IGridCell gridObject, Vector2Int coordinates, CellType cellType, Action<CellData> onUpdateData)
    {
        GridObject = gridObject;
        CellCoordinates = coordinates;
        CellType = cellType;
        OnDataUpdate = onUpdateData;
        
        MovementData = new PathfindingData(this);
        AttackData = new VisibilityData(this);
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