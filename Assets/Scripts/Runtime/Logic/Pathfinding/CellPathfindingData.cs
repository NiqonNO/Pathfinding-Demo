using System;
using UnityEngine;

public class CellPathfindingData
{
    public event Action OnDataUpdate;

    public DisplayData MoveRangeData { get; private set; } = new();
    public DisplayData MovePathData { get; private set; } = new();
    public DisplayData AttackRangeData { get; private set; } = new();

    public bool IsRange
    {
        get => MoveRangeData is { Active: true };
        set
        {
            MoveRangeData.Active = true;
            OnDataUpdate?.Invoke();
        }
    }
    public bool IsMovementPath
    {
        get => MovePathData is { Active: true };
        set
        {
            MovePathData.Active = true;
            OnDataUpdate?.Invoke();
        }
    }
    public bool IsAttack
    {
        get => AttackRangeData is { Active: true };
        set
        {
            AttackRangeData.Active = true;
            OnDataUpdate?.Invoke();
        }
    }
    public bool IsVisibility
    {
        get => AttackRangeData is { Active: true };
        set
        {
            AttackRangeData.Active = true;
            OnDataUpdate?.Invoke();
        }
    }

    public string Distance => 
        MovePathData.Active ? MovePathData.DisplayText : 
        MoveRangeData.Active ? MoveRangeData.DisplayText : 
        AttackRangeData.DisplayText;

    public void ClearRangeData() 
    {
        MoveRangeData.Clear();
        OnDataUpdate?.Invoke();
    }
    public void ClearMovementData()
    {
        MovePathData.Clear();
        OnDataUpdate?.Invoke();
    }
    public void ClearAttackData()
    {
        AttackRangeData.Clear();
        OnDataUpdate?.Invoke();
    }
    public void ClearVisibilityData()
    {
        AttackRangeData.Clear();
        OnDataUpdate?.Invoke();
    }
    
}
public class DisplayData
{
    public bool Active { get; set; } = false;
    public string DisplayText { get; set; } = string.Empty;
    public Color DisplayColor { get; set; } = Color.black;

    public void Clear()
    {
        Active = false;
        DisplayText = string.Empty;
        DisplayColor = Color.black;
    }
}