public class BSFHandler_MovementRange : BFSHandler
{
    protected override bool CellVisitCondition(IGridCell cell) => cell.CellType == CellType.Traversable && !cell.Occupied;
    protected override void OnCellVisited(IGridCell cell) => cell.PathfindingData.IsRange = true;
    protected override void OnCellCleared(IGridCell cell) => cell.PathfindingData.ClearRangeData();
    protected override int GetCellDistance(IGridCell cell) => cell.PathfindingData.MoveRangeData.Distance;
    protected override void SetCellDistance(IGridCell cell, int distance) => cell.PathfindingData.MoveRangeData.Distance = distance;
}