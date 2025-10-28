public class SelectionHandler
{
	private readonly PathfindingHandler PathfindingHandler = new();
    
	private ISelectable SelectedCell;
	private IHoverable HoveredCell;

	public void OnHover(IWorldObject entity)
	{
		HoverCell(entity as IHoverable);
	}
	public void OnSelect(IWorldObject entity)
	{
		(entity as ITouchable)?.Touch();
		SelectCell(entity as ISelectable);
	}
    
	private void HoverCell(IHoverable hoverable)
	{
		if (HoveredCell == hoverable) return;

		HoveredCell?.PointerLeave();
		HoveredCell = hoverable;
		HoveredCell?.PointerHover();

		PathfindingHandler.HighlightCell(hoverable as IGridCell);
	}
    
	private void SelectCell(ISelectable selectable)
	{
		if (SelectedCell == selectable) return;

		SelectedCell?.Deselect();
		SelectedCell = selectable;
		SelectedCell?.Select();

		PathfindingHandler.PressCell(selectable as IGridCell);
	}

	public void DeselectCurrent() => OnSelect(null);
}