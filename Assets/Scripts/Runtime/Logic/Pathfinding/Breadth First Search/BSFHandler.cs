using System;
using System.Collections.Generic;

public class BSFHandler : PathfindingSearchHandler<BSFCellData>
{
	private readonly Queue<BSFCellData> Frontier = new();
	public bool HaveRange => HaveData;
    
	public void GetRange(CellData startCell, int range)
	{
		ClearData();
        
		HaveData = true;
		var originCell = startCell.MovementRangeData;
		Frontier.Enqueue(originCell);
		VisitedCells.Add(originCell);
		originCell.Display();

		while (Frontier.Count > 0)
		{
			BSFCellData current = Frontier.Dequeue();
			if (current.Distance >= range) continue;
			for (CellDirection direction = CellDirection.N; direction <= CellDirection.W; direction++)
			{
				ScanCell(current, direction);
			}
		}
	}
	void ScanCell(BSFCellData current, CellDirection direction)
	{
		if (!current.TryGetNext(direction, out var neighbor) ||
		    !VisitedCells.Add(neighbor)) return;

		neighbor.Distance = current.Distance + 1;
		Frontier.Enqueue(neighbor);
		neighbor.Display();
	}
    
	public override void ClearData()
	{
		base.ClearData();
		Frontier.Clear();
	}
}