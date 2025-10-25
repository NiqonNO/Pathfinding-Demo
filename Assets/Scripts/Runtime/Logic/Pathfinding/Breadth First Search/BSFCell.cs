public class BSFCell : PathfindingCell
{
	public BSFCell(IGridCell cell) : base(cell) { }
	
	public bool TryGetNext(CellDirection direction, out BSFCell newCell)
	{
		newCell = null;
		if (!Cell.TryGetNeighbor(direction, out var next)) return false;
        
		newCell = new BSFCell(next)
		{
			Distance = Distance + 1
		};
		return true;
	}
}