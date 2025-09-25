using System;
using UnityEngine;
using UnityEngine.Events;

public class CellObject : MonoBehaviour, IWorldObject, IHoverable, ITouchable
{
    [SerializeField] 
    private CellDisplay Display;
    
    [field: SerializeField]
    public UnityEvent OnTouch { get; set; }
    [field: SerializeField]
    public UnityEvent OnMouseEnter { get; set; }
    [field: SerializeField]
    public UnityEvent OnMouseExit { get; set; }
    
    public Vector2Int CellCoordinates { get; private set; }
    public CellObject[] Neighbors { get; private set; }
    public CellType CellType { get; private set; }

    public void Initialize(Vector2Int coordinates, CellSettings cellSettings, CellType cellType)
    {
        CellCoordinates = coordinates;
        CellType = cellType;
        Neighbors = new CellObject[4];

        Display.SetSize(cellSettings.CellSize);
        Display.SetCellType(cellType);
    }

    public CellObject GetNeighbor(CellDirection direction)
    {
        return Neighbors[(int)direction];
    }

    public void SetNeighbor(CellDirection direction, CellObject cell)
    {
        Neighbors[(int)direction] = cell;
        cell.Neighbors[(int)direction.Opposite()] = this;
    }
    
    public void Touch()
    {
        OnTouch?.Invoke();
    }
    
    public void PointerHover()
    {
        OnMouseEnter?.Invoke();
    }

    public void PointerLeave()
    {
        OnMouseExit?.Invoke();
    }

    public void Destroy()
    {
    }
}
