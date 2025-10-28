using UnityEngine;

public class PathfindingHandler
{
    private readonly Message OutOfRangeMessage = new ("Target Out Of Range");
    private readonly Message UnreachableMessage = new ("Target Unreachable");
    
    private readonly MovementHandler MovementHandler = new();
    private readonly AttackHandler AttackHandler = new();
    
    public bool OutOfRange { get; private set; }
    public bool Unreachable { get; private set; }

    private IGridCell OriginCell;
    private IGridUnit OriginCellUnit => OriginCell.Unit;
    private CellData CellData => OriginCell.Data;
    private int MovementRange => OriginCellUnit.MoveRange;
    private int AttackRange => OriginCellUnit.AttackRange;

    public void HighlightCell(IGridCell cell)
    {
        if (OriginCell == null) return;
        ClearPath();
        var cellData = cell?.Data;
        if (cellData == null) return;

        if (cell.Occupied &&
            cell.Unit.ValidForAttack)
        {
            HighlightAttackPath(cellData);
            HandleMessageDisplay();
            return;
        }
        if (OriginCell != null)
        {
            HighlightMovementPath(cellData);
            HandleMessageDisplay();
            return;
        }
    }

    private void HighlightAttackPath(CellData cellData)
    {
        AttackHandler.ShowRange(cellData, AttackRange);
        if (!AttackHandler.HaveAttackPosition)
        {
            /*MovementHandler.ShowPath(OriginCell, cell, MovementRange);
                AttackHandler.CrossCheckCells();*/
        }
        if (!AttackHandler.HaveAttackPosition) return;
        AttackHandler.ShowPath(cellData);
        if (AttackHandler.AttackPositionCell == CellData) return;
        
        MovementHandler.ShowPath(CellData, AttackHandler.AttackPositionCell, MovementRange);
    }

    private void HighlightMovementPath(CellData cellData)
    {
        MovementHandler.ShowPath(CellData, cellData, MovementRange);
    }

    public void PressCell(IGridCell cell)
    {
        if (OriginCell == cell) return;
        TryRunUnitOrders();
        Clear();
        if (cell?.Data == null) return;
        
        if (!cell.Occupied ||
            !cell.Unit.ValidForSelection) return;
        
        OriginCell = cell;
        MovementHandler.ShowRange(CellData, MovementRange);
    }

    private void TryRunUnitOrders()
    {
        if (OriginCell == null) return;
        if (MovementHandler.HavePath)
        {
            OriginCellUnit.AddOrder(new MoveOrder(MovementHandler.Path));
        }
        if (AttackHandler.HavePath)
        {
            OriginCellUnit.AddOrder(new AttackOrder(AttackHandler.Path));
        }
        OriginCellUnit.RunOrders();
    }

    private void Clear()
    {
        OriginCell = null;
        MovementHandler.ClearRange();
        ClearPath();
    }
    private void ClearPath()
    {
        MovementHandler.ClearPath();
        AttackHandler.ClearRange();
        AttackHandler.ClearPath();
    }
    
    private void HandleMessageDisplay()
    {
        if (Unreachable)
        {
            if (UnreachableMessage.Active) return;
            MessagingHandler.DisplayMessage(UnreachableMessage);
            return;
        }
        
        if (OutOfRange)
        {
            if (OutOfRangeMessage.Active) return;
            MessagingHandler.DisplayMessage(OutOfRangeMessage);
            return;
        }
    }
}