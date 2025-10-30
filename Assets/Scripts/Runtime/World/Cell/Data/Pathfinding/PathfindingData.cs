public class PathfindingData : IBFSData, IAStarData
{
	public CellData Cell { get; }

	public IAStarData Previous { get; set; } = null;
	public int X => Cell.CellCoordinates.x;
	public int Y => Cell.CellCoordinates.y;
	int IBFSData.Distance { get; set; } = 0;
	int IAStarData.Distance { get; set; } = int.MaxValue;
	public int Distance => IsPath ? ((IAStarData)this).Distance : ((IBFSData)this).Distance;
	public int Estimation { get; set; } = int.MaxValue;
	public bool IsOutOfRange { get; set; } = false;


	public bool IsRange { get; private set; }
	public bool IsPath { get; private set; }
	public bool Valid => IsRange || IsPath;

	public PathfindingData(CellData cell)
	{
		Cell = cell;
	}
	
	public bool TryGetNext(CellDirection direction, out IBFSData newCellData) => TryGetNext<IBFSData>(direction, out newCellData);
	public bool TryGetNext(CellDirection direction, out IAStarData newCellData) => TryGetNext<IAStarData>(direction, out newCellData);
	private bool TryGetNext<T>(CellDirection direction, out T newCellData) where T : class
	{
		newCellData = default;
		if (!Cell.TryGetNeighbor(direction, out var next)) return false;

		newCellData = next.MovementData as T;
		return newCellData != null;
	}

	public bool IsTraversable()
	{
		if(Cell.CellType != CellType.Traversable) return false;
		if (Cell.Occupied) return false;
		return true;
	}

	void IAStarData.OnValid()
	{
		IsPath = true;
		
		Cell.UpdateDisplay();
	}
	void IBFSData.OnValid()
	{
		IsRange = true;
		
		Cell.UpdateDisplay();
	}
	void IAStarData.Clear()
	{
		IsPath = false;

		((IAStarData)this).Distance = int.MaxValue;
		Estimation = int.MaxValue;
		IsOutOfRange = false;
		Previous = null;
		
		Cell.UpdateDisplay();
	}
	void IBFSData.Clear()
	{
		IsRange = false;
		
		((IBFSData)this).Distance = 0;
		Cell.UpdateDisplay();
	}

	public void OnValid()
	{
		((IBFSData)this).OnValid();
		((IAStarData)this).OnValid();
	}

	public void Clear()
	{
		((IBFSData)this).Clear();
		((IAStarData)this).Clear();
	}
}