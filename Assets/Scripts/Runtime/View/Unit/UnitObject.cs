using UnityEngine;
using UnityEngine.Events;

public class UnitObject : MonoBehaviour, IGridUnit
{
    [SerializeField] 
    private UnitSettings UnitSettings;
    
    [field: SerializeField]
    public UnityEvent OnSelect { get; set; }
    [field: SerializeField]
    public UnityEvent OnDeselect { get; set; }

    public IGridCell Cell { get; private set; }
    public int MoveRange => UnitSettings.MoveRange;
    public int AttackRange => UnitSettings.AttackRange;
    private bool Friendly;

    public bool ValidForSelection => Friendly;
    
    public void Initialize(UnitSpawnType unitType)
    {
        Friendly = unitType == UnitSpawnType.Friendly;
    }
    
    public void AssignCell(IGridCell gridCell)
    {
        if (Cell == gridCell) return;
        
        Cell?.AssignUnit(null);
        Cell = gridCell;
        Cell?.AssignUnit(this);
    }
    
    public void Select()
    {
        OnSelect?.Invoke();
    }

    public void Deselect()
    {
        OnDeselect?.Invoke();
    }

    public void Destroy()
    {
        
    }
}