using System;
using System.Collections.Generic;

public class PathfindingHandler
{
    private readonly BSFHandler_MovementRange MovementRangeHandler;
    private readonly BSFHandler_AttackRange AttackRangeHandler;
    private readonly AStarHandler MovementPathHandler;

    public List<IGridCell> MovementPath => MovementPathHandler.Path;
    public HashSet<IGridCell> AttackPath => AttackRangeHandler.Line;

    public bool HaveMovementPath => MovementPathHandler.HaveValidPath;
    public bool HaveAttackPath => AttackRangeHandler.HaveValidAttackPoint;

    public PathfindingHandler()
    {
        MovementRangeHandler = new BSFHandler_MovementRange();
        AttackRangeHandler = new BSFHandler_AttackRange(MovementRangeHandler);
        MovementPathHandler = new AStarHandler();
    }

    public void ShowRange(IGridCell selectedCell)
    {
        MovementRangeHandler.GetRange_BFS(selectedCell, selectedCell.Unit.MoveRange);
    }
    public void ClearRange()
    {
        MovementRangeHandler.ClearData();
    }
    
    public void ShowMovePath(IGridCell selectedCell, IGridCell targetCell)
    {
        MovementPathHandler.FindPath_AStar(selectedCell, targetCell);
        if (!MovementPathHandler.FoundPath) return;
        
        MovementPathHandler.ReconstructPath(targetCell, selectedCell.Unit.MoveRange);
    }
    public void ClearMovePath()
    {
        MovementPathHandler.ClearData();
    }
    
    public void ShowAttackPath(IGridCell selectedCell, IGridCell targetCell)
    {
        AttackRangeHandler.GetRange_BFS(selectedCell, targetCell, selectedCell.Unit.AttackRange);
        if (AttackRangeHandler.FoundAttackPosition)
        {
            if (AttackRangeHandler.NeedToMove)
                ShowMovePath(selectedCell, AttackRangeHandler.AttackPosition);
            return;
        }
        TryToShowAttackMovePath(selectedCell, targetCell);
    }
    private void TryToShowAttackMovePath(IGridCell selectedCell, IGridCell targetCell)
    {
        MovementPathHandler.FindPath_AStar(selectedCell, targetCell);
        AttackRangeHandler.CrossCheckCells(MovementPathHandler, targetCell);
        if (AttackRangeHandler.FoundAttackPosition)
        {
            MovementPathHandler.ReconstructPath(AttackRangeHandler.AttackPosition, selectedCell.Unit.MoveRange);
        }
    }
    public void ClearAttackPath()
    {
        AttackRangeHandler.ClearData();
    }
    
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