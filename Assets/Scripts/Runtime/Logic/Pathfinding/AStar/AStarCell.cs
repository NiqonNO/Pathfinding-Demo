public class AStarCell : PathfindingCell
{
	public override int Distance { get; set;  } = int.MaxValue;
	public int Estimation { get; set; } = int.MaxValue;
	public AStarCell Previous { get; set; } = null;
	public bool IsOutOfRange { get; set; } = false;

	public AStarCell(IGridCell cell, AStarCell previous) : this(cell)
	{
		Previous = previous;
	}
	public AStarCell(IGridCell cell) : base(cell) { }
}