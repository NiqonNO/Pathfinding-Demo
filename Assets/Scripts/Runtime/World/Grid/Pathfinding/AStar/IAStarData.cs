public interface IAStarData : ICellSearchData
{
	IAStarData Previous { get; set; }
	int X { get; }
	int Y { get; }
	int Distance { get; set; }
	int Estimation { get; set; }
	bool IsOutOfRange { get; set; }

	bool TryGetNext(CellDirection direction, out IAStarData newCellData);
	bool IsTraversable();
	void OnValid();
	void Clear();
}