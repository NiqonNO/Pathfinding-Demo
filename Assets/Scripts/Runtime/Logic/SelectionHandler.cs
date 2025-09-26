using UnityEngine;

public class SelectionHandler
{
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
        
        if (entity is IGridCell cell )
        {
            if(cell.Occupied)
            {
                if (cell.Unit.ValidForSelection)
                {
                    SelectCell(cell);
                }
                else if (SelectedCell != null)
                {
                    //PathfindingHandler.AttackPath(SelectedCell, cell);
                }
            }
            else
            {
                //PathfindingHandler.MovementPath(SelectedCell, cell);
            }
        }
    }

    private void HoverCell(IHoverable entity)
    {
        if (HoveredCell == entity) return;

        if (HoveredCell != null)
        {
            HoveredCell.PointerLeave();
            PathfindingHandler.ClearMovePath();
        }

        HoveredCell = entity;

        if (HoveredCell != null)
        {
            HoveredCell.PointerHover();
            if (SelectedCell != null &&
                HoveredCell is IGridCell targetCell)
            {
                if (targetCell.Occupied && !targetCell.Unit.ValidForSelection)
                {
                    PathfindingHandler.ShowAttackPath(SelectedCell, targetCell);
                }
                else
                {
                    PathfindingHandler.ShowMovePath(SelectedCell, targetCell);
                }
            }
        }
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
            PathfindingHandler.ClearRange();
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
}