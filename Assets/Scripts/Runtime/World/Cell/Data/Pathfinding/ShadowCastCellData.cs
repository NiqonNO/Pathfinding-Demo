using System;

public class ShadowCastCellData : PathfindingCellData
{
	public ShadowCastCellData RowOrigin  { get; set; }
	public int Depth { get; private set; }
	public int Column { get; private set; }
	public float MinSlope { get; set; }
	public float MaxSlope { get; set; }
	public Visibility Visible { get; set; }
	public Visibility PreviousVisible { get; private set; }
	
	public int Distance => Depth + Column;
	
	private CellDirection DepthDirection;
	private CellDirection ScanDirection;
	
	public ShadowCastCellData(CellData cell) : base(cell) { }

	public void GetDirectionsForOctant(Octant octant)
	{
		(DepthDirection, ScanDirection) = octant switch
		{
			Octant.NE => (CellDirection.N, CellDirection.E),
			Octant.EN => (CellDirection.E, CellDirection.N),
			Octant.ES => (CellDirection.E, CellDirection.S),
			Octant.SE => (CellDirection.S, CellDirection.E),
			Octant.SW => (CellDirection.S, CellDirection.W),
			Octant.WS => (CellDirection.W, CellDirection.S),
			Octant.WN => (CellDirection.W, CellDirection.N),
			Octant.NW => (CellDirection.N, CellDirection.W),
			_ => throw new ArgumentOutOfRangeException(nameof(octant), octant, null)
		};
	}
	
	public bool TryGetNextRow(out ShadowCastCellData newCellData)
	{
		if (!TryGetNext(DepthDirection, Visibility.Inapplicable, Depth + 1, Column, 0.0f, 1.0f, out newCellData)) return false;
		newCellData.RowOrigin = newCellData;
		return true;
	}

	public bool TryGetNextCell(out ShadowCastCellData newCellData)
	{
		if (!TryGetNext(ScanDirection, Visible, Depth, Column + 1, MinSlope, MaxSlope, out newCellData)) return false;
		newCellData.RowOrigin = RowOrigin;
		return true;

	}
	
	private bool TryGetNext(CellDirection direction, Visibility previousVisible, int depth, int column, float minSlope, float maxSlope, out ShadowCastCellData newCellData)
	{
		newCellData = null;
		if (!Cell.TryGetNeighbor(direction, out var next)) return false;

		newCellData = next.AttackRangeData;
		newCellData.DepthDirection = DepthDirection;
		newCellData.ScanDirection = ScanDirection;
		newCellData.Visible = Visibility.Inapplicable;
		newCellData.PreviousVisible = previousVisible;
		newCellData.Depth = depth;
		newCellData.Column = column;
		newCellData.MinSlope = minSlope;
		newCellData.MaxSlope = maxSlope;
		return true;
	}
	
	public bool IsVisible()
	{
		if(Cell.CellType == CellType.Obstacle) return false;
		return true;
	}
	public override void Clear()
	{
		Depth = 0;
		Column = 0;
		MinSlope = 0;
		MaxSlope = 1;
		Visible = Visibility.Inapplicable;
		RowOrigin = null;
		base.Clear();
	}
}