using System;

public class CellPathfindingData
{
    public event Action OnDataUpdate;

    public RangePathfinding RangeData { get; private set; } = new();
    public MovementPathfinding MovementData { get; private set; } = new();
    public AttackPathfinding AttackData { get; private set; } = new();

    public bool IsRange
    {
        get => RangeData is { Active: true };
        set
        {
            RangeData.Active = true;
            OnDataUpdate?.Invoke();
        }
    }

    public bool IsMovementPath
    {
        get => MovementData is { Active: true };
        set
        {
            MovementData.Active = true;
            OnDataUpdate?.Invoke();
        }
    }

    public bool IsAttack
    {
        get => AttackData is { Active: true };
        set
        {
            AttackData.Active = true;
            OnDataUpdate?.Invoke();
        }
    }

    public int Distance => IsMovementPath ? MovementData.Distance : IsRange ? RangeData.Distance : 0;
    
    public void ClearRangeData() 
    {
        RangeData = new RangePathfinding();
        OnDataUpdate?.Invoke();
    }
    public void ClearMovementData()
    {
        MovementData = new MovementPathfinding();
        OnDataUpdate?.Invoke();
    }
    public void ClearAttackData()
    {
        AttackData = new AttackPathfinding();
        OnDataUpdate?.Invoke();
    }
}

public class RangePathfinding
{
    public bool Active { get; set; } = false;
    public int Distance { get; set; } = 0;
}
public class MovementPathfinding
{
    public bool Active { get; set; } = false;
    public int Distance { get; set; } = int.MaxValue;
    public int Estimation { get; set; } = int.MaxValue;
    public IGridCell Previous { get; set; } = null;
}

public class AttackPathfinding
{
    public bool Active { get; set; } = false;
    public int Distance { get; set; } = int.MaxValue;
}