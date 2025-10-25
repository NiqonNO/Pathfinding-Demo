using System;
using System.Collections.Generic;

public abstract class PathfindingSearchHandler
{
	protected readonly HashSet<IGridCell> ValidCells = new HashSet<IGridCell>();
	
	protected Func<IGridCell, bool> CellVisitCondition;
	protected Action<PathfindingCell> OnCellValid;
	protected Action<IGridCell> OnCellCleared;
	
	protected bool HaveData;

	protected PathfindingSearchHandler(Func<IGridCell, bool> cellVisitCondition, 
		Action<PathfindingCell> onCellValid,
		Action<IGridCell> onCellCleared)
	{
		CellVisitCondition = cellVisitCondition;
		OnCellValid = onCellValid;
		OnCellCleared = onCellCleared;
	}
	
	public virtual void ClearData()
	{
		if (!HaveData) return;
        
		foreach (var cells in ValidCells)
		{
			OnCellCleared?.Invoke(cells);
		}

		ValidCells.Clear();
		HaveData = false;
	}
}