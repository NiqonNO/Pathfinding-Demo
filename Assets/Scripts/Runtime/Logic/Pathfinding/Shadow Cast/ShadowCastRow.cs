class ShadowCastRow
{
	public ShadowCastCell RowOrigin { get; set; }
	public float MinSlope { get; set; }
	public float MaxSlope { get; set; }
	public CellDirection Direction { get; }

	public ShadowCastRow(ShadowCastCell rowOrigin, float minSlope, float maxSlope, CellDirection direction)
	{
		RowOrigin = rowOrigin;
		MinSlope = minSlope;
		MaxSlope = maxSlope;
		Direction = direction;
	}

	public bool TryGetNext(out ShadowCastRow newRow)
	{
		newRow = null;
		if (!RowOrigin.Cell.TryGetNeighbor(Direction, out var next)) return false;

		var newOrigin = new ShadowCastCell(next, RowOrigin.Depth + 1, RowOrigin.Column, RowOrigin.Direction);
		newRow = new ShadowCastRow(newOrigin, MinSlope, MaxSlope, Direction);
		return true;
	}
}