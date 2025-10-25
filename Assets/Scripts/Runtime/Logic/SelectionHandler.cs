using UnityEngine;

public class SelectionHandler
{
    private readonly Message OutOfRangeMessage = new ("Target Out Of Range");
    private readonly Message UnreachableMessage = new ("Target Unreachable");
    
    private readonly PathfindingHandler PathfindingHandler = new();
    
    private IGridCell SelectedCell;
    private IHoverable HoveredCell;

    public void OnHover(IWorldObject entity)
    {
        switch (entity)
        {
            case IHoverable hoverable:
                HoverCell(hoverable);
                break;
            case null:
                HoverCell(null);
                break;
        }
    }
    public void OnSelect(IWorldObject entity)
    {
        if(entity == null)
        {
            SelectCell(null);
            return;
        }
        
        if (entity is ITouchable touchable)
        {
            touchable.Touch();
        }

        if (entity is not IGridCell cell) return;
        
        if(cell.Occupied &&
           cell.Unit.ValidForSelection)
        {
            SelectCell(cell);
            return;
        }
        if (SelectedCell is null or {Occupied: false}) return;
        IGridUnit unit = SelectedCell.Unit;
        
        if (PathfindingHandler.HaveMovementPath)
        {
            unit.AddOrder(new MoveOrder(PathfindingHandler.MovementPath));
        }
        if (PathfindingHandler.HaveAttackPath)
        {
            unit.AddOrder(new AttackOrder(cell));
        }
 
        DeselectCurrent();
        unit.RunOrders();
    }

    private void HoverCell(IHoverable entity)
    {
        if (HoveredCell == entity) return;

        if (HoveredCell != null)
        {
            HoveredCell.PointerLeave();
            PathfindingHandler.ClearHoover();
        }

        HoveredCell = entity;

        if (HoveredCell == null)
        {
            MessagingHandler.ClearMessage();
            return;
        }

        HoveredCell.PointerHover();
        if (SelectedCell == null ||
            HoveredCell is not IGridCell targetCell)
        {
            MessagingHandler.ClearMessage();
            return;
        }

        if (!targetCell.Occupied)
        {
            PathfindingHandler.ClearAttackPath();
            PathfindingHandler.ShowMovePath(SelectedCell, targetCell);
            HandleMessageDisplay();
            return;
        }
        
        if (targetCell.Unit.ValidForAttack)
        {
            PathfindingHandler.ClearMovePath();
            PathfindingHandler.ShowAttackPath(SelectedCell, targetCell);
            HandleMessageDisplay();
            return;
        }
        
        PathfindingHandler.ClearHoover();
        MessagingHandler.ClearMessage();
    }

    public void DeselectCurrent() => SelectCell(null);
    private void SelectCell(IGridCell entity)
    {
        if (SelectedCell == entity) return;
        
        if (SelectedCell != null)
        {
            if (SelectedCell.Occupied)
            {
                SelectedCell.Unit.Deselect();
            }
            PathfindingHandler.ClearSelection();
        }
        
        SelectedCell = entity;
        
        if (SelectedCell != null)
        {
            if (SelectedCell.Occupied)
            {
                SelectedCell.Unit.Select();
            }
            PathfindingHandler.ShowRange(SelectedCell);
        }
    }
    
    private void HandleMessageDisplay()
    {
        if (PathfindingHandler.Unreachable)
        {
            if (UnreachableMessage.Active) return;
            MessagingHandler.DisplayMessage(UnreachableMessage);
            return;
        }
        
        if (PathfindingHandler.OutOfRange)
        {
            if (OutOfRangeMessage.Active) return;
            MessagingHandler.DisplayMessage(OutOfRangeMessage);
            return;
        }
        
        MessagingHandler.ClearMessage();
    }
}