using System.Collections.Generic;

public class AttackHandler
{
	private readonly ShadowCastHandler RangeHandler;
	private readonly DDAHandler PathHandler;

	public CellData AttackPositionCell { get; set; }
	public bool HaveAttackPosition => AttackPositionCell != null;

	public List<ICellSearchData> Path => PathHandler.Path;
	public bool HavePath => PathHandler.HaveValidPath;

	public AttackHandler()
	{
		RangeHandler = new ShadowCastHandler();
		PathHandler = new DDAHandler();
		RangeHandler.OnCellValid += EvaluateAttackPosition;
	}
	
	public void ShowRange(CellData selectedCell, int range)
	{
		RangeHandler.GetVisibility(selectedCell.AttackData, range);
	}
	public void ClearRange()
	{
		AttackPositionCell = null;
		RangeHandler.ClearData();
	}
	
	public void ShowPath(CellData targetCell)
	{
		if (AttackPositionCell == null) return;
		PathHandler.ShowPath(targetCell.AttackData, AttackPositionCell.AttackData);
	}
	public void ClearPath()
	{
		PathHandler.ClearData();
	}
	
	private void EvaluateAttackPosition(ICellSearchData tested)
	{
		var cellData = tested.Cell;
		if (!cellData.MovementData.Valid || cellData.AttackData.Visible != VisibilityState.Visible) return;
		
		if (ReferenceEquals(AttackPositionCell, cellData)) return;
		if (AttackPositionCell == null)
		{
			AttackPositionCell = cellData;
			return;
		}

		int rangeCompare = AttackPositionCell.MovementData.Distance.CompareTo(cellData.MovementData.Distance);
		if (rangeCompare == -1) return;
		if (rangeCompare == 1)
		{
			AttackPositionCell = cellData;
			return;
		}

		int attackCompare = AttackPositionCell.AttackData.Distance.CompareTo(cellData.AttackData.Distance);
		if (attackCompare == 1)
		{
			AttackPositionCell = cellData;
		}
	}
}