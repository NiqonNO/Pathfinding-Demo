using System.Collections.Generic;
using UnityEngine;

public class MovementHandler
{
	private readonly BSFHandler RangeHandler = new();
	private readonly AStarHandler PathHandler = new();
	
	public List<IGridCell> Path => PathHandler.Path;
	public bool HavePath => PathHandler.HaveValidPath;

	public void ShowRange(CellData selectedCell, int range)
	{
		RangeHandler.GetRange(selectedCell, range);
	}
	public void ClearRange()
	{
		RangeHandler.ClearData();
	}
    
	public void ShowPath(CellData selectedCell, CellData targetCell, int range)
	{
		PathHandler.FindPath(selectedCell, targetCell);
		if (!PathHandler.FoundPath) return;
		PathHandler.GetPath(targetCell, range);
	}
	public void ClearPath()
	{
		PathHandler.ClearData();
	}
	
	/*private bool BSFHandler_CellVisitCondition(IGridCell cell) => cell.CellType == CellType.Traversable && !cell.Occupied;
	private void BSFHandler_OnCellVisited(PathfindingCell cell)
	{
		cell.Cell.PathfindingData.MoveRangeData.DisplayText = cell.Distance.ToString();
		cell.Cell.PathfindingData.IsRange = true;
	}
	private void BSFHandler_OnCellCleared(IGridCell cell) => cell.PathfindingData.ClearRangeData();
	
	private bool AStarHandler_CellVisitCondition(IGridCell cell) => cell.CellType == CellType.Traversable && !cell.Occupied;
	private void AStarHandler_OnCellVisited(PathfindingCell cell)
	{
		cell.Cell.PathfindingData.MovePathData.DisplayText = cell.Distance.ToString();
		if (cell is AStarCell { IsOutOfRange: true })
			cell.Cell.PathfindingData.MovePathData.DisplayColor = Color.red;
		cell.Cell.PathfindingData.IsMovementPath = true;
	}
	private void AStarHandler_OnCellCleared(IGridCell cell) => cell.PathfindingData.ClearMovementData();*/
}