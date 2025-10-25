using System.Collections.Generic;
using UnityEngine;

public class MovementHandler
{
	private readonly BSFHandler RangeHandler;
	private readonly AStarHandler PathHandler;
	
	public List<IGridCell> Path => PathHandler.Path;
	public bool HavePath => PathHandler.HaveValidPath;
	
	public MovementHandler()
	{
		RangeHandler = new BSFHandler(BSFHandler_CellVisitCondition, BSFHandler_OnCellVisited, BSFHandler_OnCellCleared);
		PathHandler = new AStarHandler(AStarHandler_CellVisitCondition, AStarHandler_OnCellVisited, AStarHandler_OnCellCleared);
	}
	
	public void ShowRange(IGridCell selectedCell)
	{
		RangeHandler.GetRange_BFS(selectedCell, selectedCell.Unit.MoveRange);
	}
	public void ClearRange()
	{
		RangeHandler.ClearData();
	}
    
	public void ShowPath(IGridCell selectedCell, IGridCell targetCell)
	{
		PathHandler.FindPath_AStar(selectedCell, targetCell);
		if (!PathHandler.FoundPath)
		{
			//Unreachable = true;
			return;
		}
        
		PathHandler.ReconstructPath(targetCell, selectedCell.Unit.MoveRange);
		//OutOfRange = PathHandler.OutOfRange;
	}
	public void ClearPath()
	{
		PathHandler.ClearData();
		//Unreachable = false;
		//OutOfRange = false;
	}
	
	private bool BSFHandler_CellVisitCondition(IGridCell cell) => cell.CellType == CellType.Traversable && !cell.Occupied;
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
	private void AStarHandler_OnCellCleared(IGridCell cell) => cell.PathfindingData.ClearMovementData();
}