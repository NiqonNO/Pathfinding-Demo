using UnityEngine;

public class DDACellData : PathfindingCellData
{
	public int X => Cell.CellCoordinates.x;
	public int Y => Cell.CellCoordinates.y;

	public int StartX { get; private set; }
	public int StartY { get; private set; }
	
	public int EndX { get; private set; }
	public int EndY { get; private set; }

	public DDACellData(CellData cell) : base(cell) { }
	
	public void SetInitialData(int startX, int startY, int endX, int endY)
	{
		StartX = startX;
		StartY = startY;
		EndX = endX;
		EndY = endY;
	}
	
	public bool TryGetNext(CellDirection direction, out DDACellData newCellData)
	{
		newCellData = null;
		if (!Cell.TryGetNeighbor(direction, out var next)) return false;

		newCellData = next.AttackPathData;
		newCellData.SetInitialData(StartX, StartY, EndX, EndY);
		return true;
	}

	public override void Clear()
	{
		base.Clear();
		StartX = 0;
		StartY = 0;
		EndX = 0;
		EndY = 0;
	}
}