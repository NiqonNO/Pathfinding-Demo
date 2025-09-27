using System;
using UnityEngine;
using UnityEngine.Events;

public class CellObject : MonoBehaviour, IHoverable, ITouchable, IGridCell
{
    [SerializeField] 
    private CellDisplay Display;
    
    [field: SerializeField]
    public UnityEvent OnTouch { get; set; }
    [field: SerializeField]
    public UnityEvent OnMouseEnter { get; set; }
    [field: SerializeField]
    public UnityEvent OnMouseExit { get; set; }
    
    public Transform Transform => transform;
    
    public CellType CellType { get; private set; }
    public Vector2Int CellCoordinates { get; private set; }
    public IGridUnit Unit { get; private set; }
    public bool Occupied => Unit != null;
    public CellPathfindingData PathfindingData { get; private set; }
    
    private CellObject[] Neighbors;

    public void Initialize(Vector2Int coordinates, CellSettings cellSettings, CellType cellType)
    {
        CellCoordinates = coordinates;
        CellType = cellType;
        Neighbors = new CellObject[4];
        PathfindingData = new CellPathfindingData();
        PathfindingData.OnDataUpdate += UpdateVisuals;

        Display.SetSettings(cellSettings, cellType);
    }

    public bool TryGetNeighbor(CellDirection direction, out IGridCell cell)
    {
        cell = Neighbors[(int)direction];
        return cell != null;
    }

    public void SetNeighbor(CellDirection direction, CellObject cell)
    {
        Neighbors[(int)direction] = cell;
        cell.Neighbors[(int)direction.Opposite()] = this;
    }
    
    public void AssignUnit(IGridUnit gridUnit)
    {
        if (Unit == gridUnit) return;
        if (gridUnit != null && Occupied)
        {
            Debug.LogError("Cell cannot accept new cellObject, it is still occupied", this);
            return;
        }
        
        Unit = gridUnit;
        Unit?.AssignCell(this);
    }
    
    public void Touch() => OnTouch?.Invoke();
    public void PointerHover() => OnMouseEnter?.Invoke();
    public void PointerLeave() => OnMouseExit?.Invoke();
    
    private void UpdateVisuals() => Display.UpdateDisplay(PathfindingData);

    public void Destroy()
    {
    }
}
