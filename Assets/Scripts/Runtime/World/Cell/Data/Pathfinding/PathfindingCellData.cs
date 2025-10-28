public abstract class PathfindingCellData
{
	public bool Active { get; private set; }
	public CellData Cell { get; }

	protected PathfindingCellData(CellData cell)
	{
		Cell = cell;
	}

	public virtual void Clear()
	{
		Active = false;
		Cell.UpdateDisplay();
	}
	
	public void Display()
	{
		Active = true;
		Cell.UpdateDisplay();
	}
}