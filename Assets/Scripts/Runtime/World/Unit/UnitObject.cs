using System;
using System.Collections;
using System.Collections.Generic;
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

    public Transform Transform => transform;

    public IAnimator UnitAnimator => Display;
    
    public IGridCell Cell { get; private set; }
    public int MoveRange => UnitSettings.MoveRange;
    public int AttackRange => UnitSettings.AttackRange;

    public bool ValidForSelection => UnitType == UnitSpawnType.Friendly;
    public bool ValidForAttack => UnitType == UnitSpawnType.Enemy;

    private UnitSpawnType UnitType;
    private Queue<UnitOrder> UnitOrders = new();
    private Coroutine OrderCommand;
    private bool Selected;

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
        Selected = true;
        OnSelect?.Invoke();
    }

    public void Deselect()
    {
        Selected = false;
        OnDeselect?.Invoke();
    }

    public void AddOrder(UnitOrder order)
    {
        UnitOrders.Enqueue(order);
    }

    public void RunOrders()
    {
        OrderCommand ??= StartCoroutine(ProcessCommands());
    }

    public void Destroy()
    {
        if (OrderCommand != null)
        {
            StopCoroutine(OrderCommand);
            OrderCommand = null;
            UnitOrders.Clear();
        }

        if (Selected)
        {
            Deselect();
        }

        Cell.AssignUnit(null);
        
        GameObject.Destroy(gameObject);
    }

    IEnumerator ProcessCommands()
    {
        while (UnitOrders.Count != 0)
        {
            UnitOrder order = UnitOrders.Dequeue();
            order.Initialize(this);
            while (!order.Completed)
            {
                order.Update();
                yield return new WaitForEndOfFrame();
            }
            order.Finish();
        }
        
        OrderCommand = null;
    }
}