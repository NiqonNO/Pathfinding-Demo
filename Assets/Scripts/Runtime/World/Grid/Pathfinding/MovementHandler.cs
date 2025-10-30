using System.Collections.Generic;

public class MovementHandler
{
	private readonly BFSHandler RangeHandler = new();
	private readonly AStarHandler PathHandler = new();
	
	public List<ICellSearchData> Path => PathHandler.Path;
	public bool HavePath => PathHandler.HaveValidPath;

	public void ShowRange(CellData selectedCell, int range)
	{
		RangeHandler.GetRange(selectedCell.MovementData, range);
	}
	public void ClearRange()
	{
		RangeHandler.ClearData();
	}
    
	public void ShowPath(CellData selectedCell, CellData targetCell, int range)
	{
		PathHandler.FindPath(selectedCell.MovementData, targetCell.MovementData);
		if (!PathHandler.FoundPath) return;
		PathHandler.GetPath(targetCell.MovementData, range);
	}
	public void ClearPath()
	{
		PathHandler.ClearData();
	}
}