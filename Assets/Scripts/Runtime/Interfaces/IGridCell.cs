using UnityEngine;

public interface IGridCell
{
    public Vector2Int CellCoordinates { get; }
    public CellType CellType { get; }
    
    public CellPathfindingData PathfindingData { get; }
    public bool TryGetNeighbor(CellDirection direction, out IGridCell cell);
    
    public IGridUnit Unit { get; }
    public bool Occupied { get; }
    public void AssignUnit(IGridUnit gridUnit);
}