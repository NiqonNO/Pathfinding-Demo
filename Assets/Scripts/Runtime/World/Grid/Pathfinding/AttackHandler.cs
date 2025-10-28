using System.Collections.Generic;

public class AttackHandler
{
	private readonly ShadowCastHandler RangeHandler = new();
	private readonly DDAHandler PathHandler = new();

	public CellData AttackPositionCell { get; private set; }
	public bool HaveAttackPosition => AttackPositionCell != null;

	public List<IGridCell> Path => PathHandler.Path;
	public bool HavePath => PathHandler.HaveValidPath;

	public void ShowRange(CellData selectedCell, int range)
	{
		RangeHandler.GetVisibility(selectedCell, range);
	}
	public void ClearRange()
	{
		AttackPositionCell = null;
		RangeHandler.ClearData();
	}
	
	public void ShowPath(CellData targetCell)
	{
		if (AttackPositionCell == null) return;
		PathHandler.ShowPath(AttackPositionCell, targetCell);
	}
	public void ClearPath()
	{
		PathHandler.ClearData();
	}
	
	private void EvaluateAttackPosition(CellData tested)
	{
		/*if (ReferenceEquals(AttackPositionCell, tested)) return;
		if (AttackPositionCell == null)
		{
			AttackPositionCell = tested;
			return;
		}

		int rangeCompare = AttackPositionCell.RangeDistance.CompareTo(tested.RangeDistance);
		if (rangeCompare == -1) return;
		if (rangeCompare == 1)
		{
			AttackPositionCell = tested;
			return;
		}

		int attackCompare = AttackPositionCell.AttackDistance.CompareTo(tested.AttackDistance);
		if (attackCompare == 1)
		{
			AttackPositionCell = tested;
		}*/
	}
	
	/*private bool CellVisitCondition(IGridCell cell) => cell != null && cell.CellType != CellType.Obstacle;
	private void OnCellVisited(PathfindingCell cell)
	{
		cell.Cell.PathfindingData.AttackRangeData.DisplayText = cell.Distance.ToString();
		cell.Cell.PathfindingData.IsVisibility = true;
	}
	private void OnCellCleared(IGridCell cell) => cell.PathfindingData.ClearVisibilityData();*/
}