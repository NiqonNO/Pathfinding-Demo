public class AStarCellData : PathfindingCellData
{
	public AStarCellData Previous { get; set; } = null;
	public int Distance { get; set; } = int.MaxValue;
	public int Estimation { get; set; } = int.MaxValue;
	public bool IsOutOfRange { get; set; } = false;
	
	public AStarCellData(CellData cell) : base(cell) { }
	
	public bool TryGetNext(CellDirection direction, out AStarCellData newCellData)
	{
		newCellData = null;
		if (!Cell.TryGetNeighbor(direction, out var next)) return false;
		if (!IsValid(next)) return false;
		
		newCellData = next.MovementPathData;
		return true;
	}
	
	public bool IsValid(CellData cell)
	{
		if(cell.CellType != CellType.Traversable) return false;
		if (cell.Occupied) return false;
		return true;
	}
	public override void Clear()
	{
		Distance = int.MaxValue;
		Estimation = int.MaxValue;
		Previous = null;
		IsOutOfRange = false;
		base.Clear();
	}
}