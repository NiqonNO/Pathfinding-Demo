public interface IBFSData : ICellSearchData
{
	int Distance { get; set; }
	
	bool TryGetNext(CellDirection direction, out IBFSData newCellData);
	bool IsTraversable();
	void OnValid();
	void Clear();
}