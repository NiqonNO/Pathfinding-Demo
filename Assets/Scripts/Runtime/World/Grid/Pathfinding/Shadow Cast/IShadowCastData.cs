public interface IShadowCastData : ICellSearchData
{
	IShadowCastData RowOrigin { get; set; }
	int X { get; set; }
	int Y { get; set; }
	int Distance { get; }
	float MinSlope { get; set; }
	float MaxSlope { get; set; }
	VisibilityState Visible { get; set; }
	TransparencyState Transparency { get; set; }
	TransparencyState PreviousTransparency { get; set; }
	
	CellDirection DepthDirection { get; set; }
	CellDirection ScanDirection { get; set; }

	bool TryGetNext(CellDirection direction, out IShadowCastData newCellData);
	bool IsTransparent();
	void OnValid();
	void Clear();
}