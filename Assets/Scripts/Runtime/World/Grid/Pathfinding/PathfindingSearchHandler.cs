using System;
using System.Collections.Generic;

public abstract class PathfindingSearchHandler<T> where T : PathfindingCellData
{
	protected readonly HashSet<T> VisitedCells = new HashSet<T>();
	
	protected bool HaveData;
	
	public virtual void ClearData()
	{
		if (!HaveData) return;
        
		foreach (var cell in VisitedCells)
		{
			cell.Clear();
		}

		VisitedCells.Clear();
		HaveData = false;
	}
}