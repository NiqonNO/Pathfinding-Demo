using System;
using UnityEngine;
using UnityEngine.Events;

public class CellObject : MonoBehaviour, IHoverable, ITouchable, ISelectable, IGridCell
{
    [SerializeField] 
    private CellDisplay Display;
    
    [field: SerializeField]
    public UnityEvent OnTouch { get; set; }
    [field: SerializeField]
    public UnityEvent OnMouseEnter { get; set; }
    [field: SerializeField]
    public UnityEvent OnMouseExit { get; set; }
    [field: SerializeField]
    public UnityEvent OnSelected { get; set; }
    [field: SerializeField]
    public UnityEvent OnDeselected{ get; set; }
    
    public CellData Data { get; private set; }
    
    public Transform Transform => transform;

    public IGridUnit Unit { get; private set; }
    public bool Occupied => Unit != null;

    public void Initialize(Vector2Int coordinates, CellSettings cellSettings, CellType cellType)
    {
        Display.SetSettings(cellSettings, cellType);
        Data = new CellData(this, coordinates, cellType, Display.UpdateDisplay);
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
    public void Select() => OnSelected?.Invoke();
    public void Deselect() => OnDeselected?.Invoke();

    public void Destroy()
    {
    }
}
