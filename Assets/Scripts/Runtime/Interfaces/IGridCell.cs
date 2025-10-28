using UnityEngine;

public interface IGridCell : IWorldObject
{
    public CellData Data { get; }
    public IGridUnit Unit { get; }
    public bool Occupied { get; }
    public void AssignUnit(IGridUnit gridUnit);
}