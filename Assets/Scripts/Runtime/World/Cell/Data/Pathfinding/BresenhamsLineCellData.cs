public class BresenhamsLineCellData : PathfindingCellData
{
	public int Distance { get; private set; } = 0;
	
	public BresenhamsLineCellData(CellData cell) : base(cell) { }
}