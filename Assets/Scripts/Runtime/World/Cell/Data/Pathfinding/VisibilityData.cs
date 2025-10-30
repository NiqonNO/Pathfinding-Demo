public class VisibilityData : IShadowCastData, IDDAData
{
	public CellData Cell { get; }

	public IShadowCastData RowOrigin { get; set; } = null;
	public int X { get; set; } = 0;
	public int Y { get; set; } = 0;
	public int Distance => X + Y;
	public float MinSlope { get; set; } = 0.0f;
	public float MaxSlope { get; set; } = 1.0f;
	public VisibilityState Visible { get; set; } = VisibilityState.Obscured;
	public TransparencyState Transparency { get; set; } = TransparencyState.Inapplicable;
	public TransparencyState PreviousTransparency { get; set; } = TransparencyState.Inapplicable;
	public CellDirection DepthDirection { get; set; } = CellDirection.N;
	public CellDirection ScanDirection { get; set; } = CellDirection.E;
	

	public bool IsRange { get; private set; }
	public bool IsPath { get; private set; }
	public bool Valid => IsRange || IsPath;

	public VisibilityData(CellData cell)
	{
		Cell = cell;
	}
	
	public bool TryGetNext(CellDirection direction, out IDDAData newCellData) => TryGetNext<IDDAData>(direction, out newCellData);
	public bool TryGetNext(CellDirection direction, out IShadowCastData newCellData) => TryGetNext<IShadowCastData>(direction, out newCellData);
	private bool TryGetNext<T>(CellDirection direction, out T newCellData) where T : class
	{
		newCellData = default;
		if (!Cell.TryGetNeighbor(direction, out var next)) return false;

		newCellData = next.AttackData as T;
		return newCellData != null;
	}
	
	public bool IsTransparent()
	{
		if(Cell.CellType == CellType.Obstacle) return false;
		return true;
	}

	void IDDAData.OnValid()
	{
		IsPath = true;
		
		Cell.UpdateDisplay();
	}
	void IShadowCastData.OnValid()
	{
		IsRange = true;
		
		Cell.UpdateDisplay();
	}
	
	void IDDAData.Clear()
	{
		IsPath = false;
		
		Cell.UpdateDisplay();
	}
	void IShadowCastData.Clear()
	{
		IsRange = false;
		
		Y = 0;
		X = 0;
		MinSlope = 0;
		MaxSlope = 1;
		Visible = VisibilityState.Obscured;
		Transparency = TransparencyState.Inapplicable;
		PreviousTransparency = TransparencyState.Inapplicable;
		DepthDirection = CellDirection.N;
		ScanDirection = CellDirection.E;
		RowOrigin = null;
		
		Cell.UpdateDisplay();
	}
	
	public void OnValid()
	{
		((IDDAData)this).OnValid();
		((IShadowCastData)this).OnValid();
	}

	public void Clear()
	{
		((IDDAData)this).Clear();
		((IShadowCastData)this).Clear();
	}
	
	/*public void Clear()
	{
		IsRange = false;
		IsPath = false;

		Y = 0;
		X = 0;
		MinSlope = 0;
		MaxSlope = 1;
		Visible = VisibilityState.Obscured;
		Transparency = TransparencyState.Inapplicable;
		PreviousTransparency = TransparencyState.Inapplicable;
		DepthDirection = CellDirection.N;
		ScanDirection = CellDirection.E;
		RowOrigin = null;
		Cell.UpdateDisplay();
	}*/
}