using System.Collections.Generic;

public class AttackHandler
{
	private readonly ShadowCastHandler RangeHandler;

	public HashSet<IGridCell> Path => null;//PathHandler.Path;
	public bool HavePath => false;//PathHandler.HaveValidPath;
	
	/*public HashSet<IGridCell> AttackPath => AttackVisibilityHandler.Line;
	public bool HaveAttackPath => AttackVisibilityHandler.HaveValidAttackPoint;*/
	
	public AttackHandler()
	{
		RangeHandler = new ShadowCastHandler(CellVisitCondition, OnCellVisited, OnCellCleared);
		//AttackRangeHandler = new BSFHandler_AttackRange(MovementRangeHandler);
	}
	
	public void ShowPath(IGridCell selectedCell, IGridCell targetCell)
	{
		RangeHandler.GetVisibility(targetCell, 20/*selectedCell.Unit.AttackRange*/);
		/*AttackVisibilityHandler.GetRange_BFS(selectedCell, targetCell, selectedCell.Unit.AttackRange);
		if (AttackVisibilityHandler.FoundAttackPosition)
		{
			if (AttackVisibilityHandler.NeedToMove)
				ShowMovePath(selectedCell, AttackVisibilityHandler.AttackPosition);
			return;
		}
		TryToShowAttackMovePath(selectedCell, targetCell);*/
	}
	/*private void TryToShowAttackMovePath(IGridCell selectedCell, IGridCell targetCell)
	{
		MovementPathHandler.FindPath_AStar(selectedCell, targetCell);
		AttackVisibilityHandler.CrossCheckCells(MovementPathHandler, targetCell);
		if (AttackVisibilityHandler.FoundAttackPosition)
		{
			MovementPathHandler.ReconstructPath(AttackVisibilityHandler.AttackPosition, selectedCell.Unit.MoveRange);
			OutOfRange = MovementPathHandler.OutOfRange;
			return;
		}
		//Unreachable = true;
	}*/
	public void ClearPath()
	{
		RangeHandler.ClearData();
		//AttackRangeHandler.ClearData();
	}
	
	
	private bool CellVisitCondition(IGridCell cell) => cell != null && cell.CellType != CellType.Obstacle;
	private void OnCellVisited(PathfindingCell cell)
	{
		cell.Cell.PathfindingData.AttackRangeData.DisplayText = cell.Distance.ToString();
		cell.Cell.PathfindingData.IsVisibility = true;
	}
	private void OnCellCleared(IGridCell cell) => cell.PathfindingData.ClearVisibilityData();
}