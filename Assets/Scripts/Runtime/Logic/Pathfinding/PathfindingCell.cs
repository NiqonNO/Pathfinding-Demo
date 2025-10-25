public abstract class PathfindingCell
{
	public IGridCell Cell { get; }
	public virtual int Distance { get; set; } = 0;
	
	protected PathfindingCell(IGridCell cell)
	{
		Cell = cell;
	}
}