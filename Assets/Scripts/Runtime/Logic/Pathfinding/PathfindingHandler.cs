using System;
using System.Collections.Generic;

public class PathfindingHandler
{
    private readonly MovementHandler MovementHandler;
    private readonly AttackHandler AttackHandler;
    
    public bool OutOfRange { get; private set; }
    public bool Unreachable { get; private set; }
    
    public List<IGridCell> MovementPath => MovementHandler.Path;
    public bool HaveMovementPath => MovementHandler.HavePath;
    
    public HashSet<IGridCell> AttackPath => AttackHandler.Path;
    public bool HaveAttackPath => AttackHandler.HavePath;

    public PathfindingHandler()
    {
        MovementHandler = new MovementHandler();
        AttackHandler = new AttackHandler();
    }

    public void ShowRange(IGridCell selectedCell) => MovementHandler.ShowRange(selectedCell);
    public void ClearRange()=> MovementHandler.ClearRange();
    
    public void ShowMovePath(IGridCell selectedCell, IGridCell targetCell) => MovementHandler.ShowPath(selectedCell, targetCell);
    public void ClearMovePath() => MovementHandler.ClearPath();
    
    public void ShowAttackPath(IGridCell selectedCell, IGridCell targetCell) => AttackHandler.ShowPath(selectedCell, targetCell);
    public void ClearAttackPath() => AttackHandler.ClearPath();
    
    public void ClearSelection()
    {
        ClearRange();
        ClearHoover();
    }
    public void ClearHoover()
    {
        ClearMovePath();
        ClearAttackPath();
    }
}