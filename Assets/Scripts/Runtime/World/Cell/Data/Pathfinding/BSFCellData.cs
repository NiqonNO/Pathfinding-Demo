public class BSFCellData : PathfindingCellData
{
	public int Distance { get; set; } = 0;
	
	public BSFCellData(CellData cell) : base(cell) { }
	
	public bool TryGetNext(CellDirection direction, out BSFCellData newCellData)
	{
		newCellData = null;
		if (!Cell.TryGetNeighbor(direction, out var next)) return false;
		if (!IsValid(next)) return false;

		newCellData = next.MovementRangeData;
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
		Distance = 0;
		base.Clear();
	}
}