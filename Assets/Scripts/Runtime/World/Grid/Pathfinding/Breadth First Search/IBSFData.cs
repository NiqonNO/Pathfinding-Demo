public interface IBSFData : ICellSearchData
{
	int Distance { get; set; }
	
	bool TryGetNext(CellDirection direction, out IBSFData newCellData);
	bool IsTraversable();
	void OnValid();
	void Clear();
}