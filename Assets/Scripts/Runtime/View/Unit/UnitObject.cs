using System;
using UnityEngine;
using UnityEngine.Events;

public class UnitObject : MonoBehaviour, IGridUnit
{
    [SerializeField] 
    private UnitSettings UnitSettings;

    [SerializeField] 
    private UnitDisplay Display;
    
    [field: SerializeField]
    public UnityEvent OnSelect { get; set; }
    [field: SerializeField]
    public UnityEvent OnDeselect { get; set; }

    public IGridCell Cell { get; private set; }
    public int MoveRange => UnitSettings.MoveRange;
    public int AttackRange => UnitSettings.AttackRange;
    private UnitSpawnType UnitType;

    public bool ValidForSelection => UnitType == UnitSpawnType.Friendly;
    public bool ValidForAttack => UnitType == UnitSpawnType.Enemy;

    public void Initialize(UnitSpawnType unitType)
    {
        UnitType = unitType;
        Display.SetUnitColor(UnitSettings.GetUnitTint(UnitType));
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