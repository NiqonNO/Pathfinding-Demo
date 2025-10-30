using System;
using System.Collections.Generic;

public abstract class CellSearchHandler<T>
{
	public event Action<T> OnCellValid;
	protected readonly HashSet<T> VisitedCells = new HashSet<T>();

	protected bool HaveData;

	protected virtual void OnValid(T cell)
	{
		ValidateCell(cell);
		OnCellValid?.Invoke(cell);
	}

	public virtual void ClearData()
	{
		if (!HaveData) return;

		foreach (var cell in VisitedCells)
		{
			ClearCell(cell);
		}

		VisitedCells.Clear();
		HaveData = false;
	}

	protected abstract void ValidateCell(T cell);
	protected abstract void ClearCell(T cell);
}