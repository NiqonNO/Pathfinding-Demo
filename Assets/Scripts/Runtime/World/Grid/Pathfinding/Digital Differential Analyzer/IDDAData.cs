public interface IDDAData : ICellSearchData
{
	int X { get; }
	int Y { get; }
	CellDirection ScanDirection { get; }
	CellDirection DepthDirection { get; }
	
	bool TryGetNext(CellDirection direction, out IDDAData newCellData);
	void OnValid();
	void Clear();
}