using System;
using UnityEngine;

public class CellPathfindingData
{
    public event Action OnDataUpdate;

    public RangeData MoveRangeData { get; private set; } = new();
    public PathData MovePathData { get; private set; } = new();
    public RangeData AttackRangeData { get; private set; } = new();

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

    public int Distance => MovePathData.Visited ? MovePathData.Distance : MoveRangeData.Distance;

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

    public void SetDistance(RangePathfindingType type, int distance)
    {
        switch (type)
        {
            case RangePathfindingType.Move:
                MoveRangeData.Distance = distance;
                break;
            case RangePathfindingType.Attack:
                AttackRangeData.Distance = distance;
                break;
            default:
                return;
        }
    }
    public int GetDistance(RangePathfindingType type)
    {
        return type switch
        {
            RangePathfindingType.Move => MoveRangeData.Distance,
            RangePathfindingType.Attack => AttackRangeData.Distance,
            _ => int.MaxValue
        };
    }
}
public class RangeData
{
    public bool Active { get; set; } = false;
    public int Distance { get; set; } = 0;

    public void Clear()
    {
        Active = false;
        Distance = 0;
    }
}
public class PathData
{
    public bool Active { get; set; } = false;
    public bool Visited { get; set; } = false;
    public bool IsOutOfRange { get; set; } = false;
    public int Distance { get; set; } = int.MaxValue;
    public int Estimation { get; set; } = int.MaxValue;
    public IGridCell Previous { get; set; } = null;
    
    public void Clear()
    {
        Active = false;
        Visited = false;
        IsOutOfRange = false;
        Distance = int.MaxValue;
        Estimation = int.MaxValue;
        Previous = null;
    }
}