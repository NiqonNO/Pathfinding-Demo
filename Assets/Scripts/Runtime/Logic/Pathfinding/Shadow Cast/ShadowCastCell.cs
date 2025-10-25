class ShadowCastCell : PathfindingCell
{
	public CellDirection Direction  { get; }
	public int Depth { get; }
	public int Column { get; }
	public Visibility Visible { get; set; }
	
	private readonly ShadowCastCell Previous;
	public Visibility PreviousVisible => Previous?.Visible ?? Visibility.Inapplicable;
	public override int Distance => Depth + Column;

	private ShadowCastCell(IGridCell cell, ShadowCastCell previous, int depth, int column, CellDirection direction) : this(cell, depth, column, direction)
	{
		Previous = previous;
	}
	public ShadowCastCell(IGridCell cell, int depth, int column, CellDirection direction) : this(cell)
	{
		Depth = depth;
		Column = column;
		Direction = direction;
	}
	public ShadowCastCell(IGridCell cell) : base(cell) { }

	public bool TryGetNext(out ShadowCastCell newCol)
	{
		newCol = null;
		if (!Cell.TryGetNeighbor(Direction, out var next)) return false;
        
		newCol = new ShadowCastCell(next, this, Depth, Column + 1, Direction);
		return true;
	}
}