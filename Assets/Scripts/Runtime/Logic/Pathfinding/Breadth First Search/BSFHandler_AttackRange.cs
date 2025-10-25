using System.Collections.Generic;

public class BSFHandler_AttackRange //: BFSHandler
{
    /*private readonly BSFHandler_MovementRange MovementRangeHandler;
    private readonly BresenhamsLineHandler AttackPathHandler;
    private readonly SortedSet<IGridCell> ReachableAttackCells = new(Comparer<IGridCell>.Create(EvaluateAttackPosition));

    public HashSet<IGridCell> Line => AttackPathHandler.Line;
    public IGridCell AttackPosition { get; private set; }
    public bool FoundAttackPosition { get; private set; }
    public bool NeedToMove { get; private set; }
    public bool NeedToMoveOutOfRange { get; private set; }
    public bool HaveValidAttackPoint => FoundAttackPosition && !NeedToMoveOutOfRange;

    private IGridCell SelectedCell;
    
    public BSFHandler_AttackRange(BSFHandler_MovementRange movementRangeHandler)
    {
        MovementRangeHandler = movementRangeHandler;
        AttackPathHandler = new BresenhamsLineHandler();
    }

    public void GetRange_BFS(IGridCell selectedCell, IGridCell targetCell, int range)
    {
        SelectedCell = selectedCell;
        GetRange_BFS(targetCell, range);

        FindValidAttackPotion(targetCell);
    }
    public void CrossCheckCells(AStarHandler movementPathHandler, IGridCell targetCell)
    {
        NeedToMoveOutOfRange = true;
        foreach (var cell in VisitedCells)
        {
            if (movementPathHandler.VisitedCells.Contains(cell))
                ReachableAttackCells.Add(cell);
        }
        FindValidAttackPotion(targetCell);
    }
    private void FindValidAttackPotion(IGridCell targetCell)
    {
        if (ReachableAttackCells.Count == 0) return;
        
        foreach (var cell in ReachableAttackCells)
        {
            AttackPathHandler.FindLine_BresenhamsLine(cell, targetCell);
            if (!AttackPathHandler.FoundLine) continue;

            Line.Add(targetCell);
            AttackPosition = cell;
            NeedToMove = cell != SelectedCell;
            FoundAttackPosition = true;
            foreach (var lineCell in Line)
            {
                VisitedCells.Add(lineCell);
                lineCell.PathfindingData.IsAttack = true;
            }
            return;
        }
    }

    public override void ClearData()
    {
        if (!HaveData) return;

        base.ClearData();

        ReachableAttackCells.Clear();
        Line.Clear();
        SelectedCell = null;
        AttackPosition = null;
        FoundAttackPosition = false;
        NeedToMove = false;
        NeedToMoveOutOfRange = false;

        AttackPathHandler.ClearData();
    }

    protected override bool CellVisitCondition(IGridCell cell) => cell.CellType != CellType.Obstacle && (!cell.Occupied || cell == SelectedCell);
    protected override void OnCellVisited(IGridCell cell) { if (MovementRangeHandler.VisitedCells.Contains(cell)) ReachableAttackCells.Add(cell); }
    protected override void OnCellCleared(IGridCell cell) => cell.PathfindingData.ClearAttackData();
    protected override int GetCellDistance(IGridCell cell) => cell.PathfindingData.AttackRangeData.Distance;
    protected override void SetCellDistance(IGridCell cell, int distance) => cell.PathfindingData.AttackRangeData.Distance = distance;

    private static int EvaluateAttackPosition(IGridCell x, IGridCell y)
    {
        if (ReferenceEquals(x, y)) return 0;
        if (x == null) return -1;
        if (y == null) return 1;

        int rangeCompare = x.PathfindingData.Distance.CompareTo(y.PathfindingData.Distance);
        if (rangeCompare != 0) return rangeCompare;
        int attackCompare = x.PathfindingData.AttackRangeData.Distance.CompareTo(y.PathfindingData.AttackRangeData.Distance);
        if (attackCompare != 0) return attackCompare;
        return x.GetHashCode().CompareTo(y.GetHashCode());
    }*/
}