using System.Collections.Generic;

public class BFSHandler : CellSearchHandler<IBFSData>
{
	private readonly Queue<IBFSData> Frontier = new();
	public bool HaveRange => HaveData;
    
	public void GetRange(IBFSData startCell, int range)
	{
		ClearData();
		HaveData = true;
		
		startCell.Distance = 0;
		
		OnValid(startCell);
		VisitedCells.Add(startCell);
		Frontier.Enqueue(startCell);

		while (Frontier.Count > 0)
		{
			IBFSData current = Frontier.Dequeue();
			if (current.Distance >= range) continue;
			for (CellDirection direction = CellDirection.N; direction <= CellDirection.W; direction++)
			{
				ScanCell(current, direction);
			}
		}
	}
	void ScanCell(IBFSData current, CellDirection direction)
	{
		if (!current.TryGetNext(direction, out var neighbor) ||
		    !neighbor.IsTraversable() ||
		    !VisitedCells.Add(neighbor)) return;

		neighbor.Distance = current.Distance + 1;
		
		OnValid(neighbor);
		Frontier.Enqueue(neighbor);
	}
	
	public override void ClearData()
	{
		base.ClearData();
		Frontier.Clear();
	}
	
	protected override void ValidateCell(IBFSData cell) => cell.OnValid();
	protected override void ClearCell(IBFSData cell) => cell.Clear();
}