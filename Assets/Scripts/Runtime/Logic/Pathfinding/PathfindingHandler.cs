using System;
using System.Collections.Generic;
using UnityEngine;

public class PathfindingHandler
{
    private readonly HashSet<IGridCell> BFS_CellsInMovementRange = new();
    private readonly HashSet<IGridCell> BFS_CellsInAttackRange = new();
    private readonly HashSet<IGridCell> AStart_VisitedCells = new();

    private readonly SortedSet<IGridCell> ReachableAttackCells = new(Comparer<IGridCell>.Create(EvaluateAttackPosition));

    public readonly List<IGridCell> MovementPath = new();
    public readonly HashSet<IGridCell> AttackPath = new();

    public bool HaveMovementPath { get; private set; }
    public bool HaveAttackPath { get; private set; }

    private bool HaveRangeData;
    private bool HavePathData;
    private bool HaveAttackData;

    public void ClearSelection()
    {
        ClearRange();
        ClearHoover();
    }
    public void ClearHoover()
    {
        ClearMovePath();
        ClearAttackPath();
    }

    public void ShowRange(IGridCell selectedCell)
    {
        if (HaveRangeData) ClearRange();

        GetMovementRange_BFS(selectedCell);
        HaveRangeData = true;
    }
    public void ClearRange()
    {
        if (!HaveRangeData) return;

        foreach (var cells in BFS_CellsInMovementRange)
        {
            cells.PathfindingData.ClearRangeData();
        }

        BFS_CellsInMovementRange.Clear();
        HaveRangeData = false;
    }

    public void ShowMovePath(IGridCell selectedCell, IGridCell hoveredCell)
    {
        if (HavePathData) ClearMovePath();

        FindPath_AStar(selectedCell, hoveredCell);
        ReconstructPath();
        HavePathData = true;
        
        void ReconstructPath()
        {
            while (hoveredCell != null)
            {
                if (hoveredCell.PathfindingData.MovementData.Distance <= selectedCell.Unit.MoveRange)
                {
                    hoveredCell.PathfindingData.IsMovementPath = true;
                    MovementPath.Add(hoveredCell);
                }

                hoveredCell = hoveredCell.PathfindingData.MovementData.Previous;
            }
            MovementPath.Reverse();
        }
    }
    public void ClearMovePath()
    {
        if (!HavePathData) return;

        foreach (var cells in AStart_VisitedCells)
        {
            cells.PathfindingData.ClearMovementData();
        }

        AStart_VisitedCells.Clear();
        MovementPath.Clear();
        HavePathData = false;
        HaveMovementPath = false;
    }

    public void ShowAttackPath(IGridCell selectedCell, IGridCell targetCell)
    {
        if (HaveAttackData) ClearAttackPath();

        int bendPrevention = Mathf.Min(2, selectedCell.Unit.AttackRange);

        /*if (targetCell.PathfindingData.Distance <= bendPrevention ||
            EuclideanDistance(selectedCell, targetCell) <= bendPrevention)
        {
            //GetAttackPath selectedCell -> targetCell
            return;
        }*/

        GetAttackRange_BFS(targetCell, selectedCell);
        FindPathAttackPotion();
        HaveAttackData = true;

        return;
        void FindPathAttackPotion()
        {
            HashSet<IGridCell> path = new HashSet<IGridCell>();
            foreach (var cell in ReachableAttackCells)
            {
                if (!FindLine_BresenhamsLine(cell, targetCell, ref path))
                {
                    path.Clear();
                    continue;
                }

                path.Add(targetCell);
                foreach (var pathCell in path)
                {
                    pathCell.PathfindingData.IsAttack = true;
                    BFS_CellsInAttackRange.Add(pathCell);
                    AttackPath.Add(pathCell);
                }

                if (cell != selectedCell)
                {
                    ShowMovePath(selectedCell, cell);
                }
                return;
            }
        }
    }
    public void ClearAttackPath()
    {
        if (!HaveAttackData) return;

        foreach (var cells in BFS_CellsInAttackRange)
        {
            cells.PathfindingData.ClearAttackData();
        }

        BFS_CellsInAttackRange.Clear();
        ReachableAttackCells.Clear();
        AttackPath.Clear();
        HaveAttackData = false;
        HaveAttackPath = false;
    }

    private void GetMovementRange_BFS(IGridCell startCell)
    {
        Queue<IGridCell> frontier = new Queue<IGridCell>();
        frontier.Enqueue(startCell);
        BFS_CellsInMovementRange.Add(startCell);

        while (frontier.Count > 0)
        {
            IGridCell current = frontier.Dequeue();
            if (current.PathfindingData.RangeData.Distance >= startCell.Unit.MoveRange) continue;

            foreach (CellDirection direction in Enum.GetValues(typeof(CellDirection)))
            {
                CheckNeighbors(current, direction);
            }
        }
        return;

        void CheckNeighbors(IGridCell current, CellDirection direction)
        {
            if (!current.TryGetNeighbor(direction, out var neighbor) ||
                BFS_CellsInMovementRange.Contains(neighbor) ||
                neighbor.CellType != CellType.Traversable ||
                neighbor.Occupied) return;

            neighbor.PathfindingData.RangeData.Distance = current.PathfindingData.RangeData.Distance + 1;
            neighbor.PathfindingData.IsRange = true;

            frontier.Enqueue(neighbor);
            BFS_CellsInMovementRange.Add(neighbor);
        }
    }
    private void GetAttackRange_BFS(IGridCell enemyCell, IGridCell friendlyCell)
    {
        Queue<IGridCell> frontier = new Queue<IGridCell>();
        frontier.Enqueue(enemyCell);
        BFS_CellsInAttackRange.Add(enemyCell);

        while (frontier.Count > 0)
        {
            IGridCell current = frontier.Dequeue();
            if (current.PathfindingData.AttackData.Distance >= enemyCell.Unit.AttackRange) continue;

            foreach (CellDirection direction in Enum.GetValues(typeof(CellDirection)))
            {
                CheckNeighbors(current, direction);
            }
        }
        return;

        void CheckNeighbors(IGridCell current, CellDirection direction)
        {
            if (!current.TryGetNeighbor(direction, out var neighbor) ||
                BFS_CellsInAttackRange.Contains(neighbor) ||
                neighbor.CellType == CellType.Obstacle ||
                (neighbor.Occupied && neighbor != friendlyCell)) return;

            neighbor.PathfindingData.AttackData.Distance = current.PathfindingData.AttackData.Distance + 1;
            if (BFS_CellsInMovementRange.Contains(neighbor))
            {
                ReachableAttackCells.Add(neighbor);
            }

            frontier.Enqueue(neighbor);
            BFS_CellsInAttackRange.Add(neighbor);
        }
    }
    private void FindPath_AStar(IGridCell startCell, IGridCell endCell)
    {
        var frontier = new PriorityQueue<IGridCell>();
        startCell.PathfindingData.MovementData.Distance = 0;
        startCell.PathfindingData.MovementData.Estimation = EuclideanDistance(startCell, endCell);
        frontier.Enqueue(startCell, startCell.PathfindingData.MovementData.Estimation);
        AStart_VisitedCells.Add(startCell);

        while (frontier.Count > 0)
        {
            IGridCell current = frontier.Dequeue();
            if (current == endCell) return;

            foreach (CellDirection direction in Enum.GetValues(typeof(CellDirection)))
            {
                CheckNeighbours(current, direction);
            }
        }
        return;

        void CheckNeighbours(IGridCell current, CellDirection direction)
        {
            if (!current.TryGetNeighbor(direction, out var neighbor) ||
                neighbor.CellType != CellType.Traversable ||
                neighbor.Occupied)
                return;

            int tentativeG = current.PathfindingData.MovementData.Distance + 1;
            if (tentativeG >= neighbor.PathfindingData.MovementData.Distance) return;

            neighbor.PathfindingData.MovementData.Previous = current;
            neighbor.PathfindingData.MovementData.Distance = tentativeG;
            neighbor.PathfindingData.MovementData.Estimation = tentativeG + EuclideanDistance(neighbor, endCell);

            if (frontier.Contains(neighbor))
            {
                frontier.UpdatePriority(neighbor, neighbor.PathfindingData.MovementData.Estimation);
                return;
            }

            frontier.Enqueue(neighbor, neighbor.PathfindingData.MovementData.Estimation);
            AStart_VisitedCells.Add(neighbor);
        }
    }
    private bool FindLine_BresenhamsLine(IGridCell startCell, IGridCell targetCell, ref HashSet<IGridCell> line)
    {
        if (startCell.Equals(targetCell)) return false;

        int x0 = startCell.CellCoordinates.x;
        int y0 = startCell.CellCoordinates.y;
        int x1 = targetCell.CellCoordinates.x;
        int y1 = targetCell.CellCoordinates.y;

        IGridCell currentCell = startCell;

        bool steep = Math.Abs(y1 - y0) > Math.Abs(x1 - x0);

        CellDirection ProgressDirection = CellDirection.E;
        CellDirection StepDirection = CellDirection.N;

        if (steep)
        {
            (x0, y0) = (y0, x0);
            (x1, y1) = (y1, x1);
            (ProgressDirection, StepDirection) = (StepDirection, ProgressDirection);
        }

        if (x0 > x1)
        {
            (x0, x1) = (x1, x0);
            (y0, y1) = (y1, y0);
            (currentCell, targetCell) = (targetCell, currentCell);
        }

        int dX = (x1 - x0);
        int dY = Math.Abs(y1 - y0);

        int err = (dX / 2);
        int ystep = (y0 < y1 ? 1 : -1);
        if (ystep < 0) StepDirection = StepDirection.Opposite();
        int y = y0;
        bool foundBresenhamsLine = false;

        int x = x0;
        for (; x < x1; ++x)
        {
            bool finished = StepX(ref line);
            if (finished)
            {
                return foundBresenhamsLine;
            }
        }
        return true;

        bool StepX(ref HashSet<IGridCell> line)
        {
            if (currentCell.CellType == CellType.Obstacle)
            {
                foundBresenhamsLine = false;
                return true;
            }

            if (x != x0) line.Add(currentCell);

            err -= dY;
            bool finished = StepY(ref line);
            if (finished)
            {
                return true;
            }

            if (!currentCell.TryGetNeighbor(ProgressDirection, out currentCell))
            {
                Debug.LogError($"Null neighbour encountered when moving {ProgressDirection}", (UnityEngine.Object)currentCell);
                throw new NullReferenceException();
            }

            return false;
        }
        bool StepY(ref HashSet<IGridCell> line)
        {
            if (err >= 0) return false;
            
            y += ystep;
            err += dX;

            if (!currentCell.TryGetNeighbor(StepDirection, out currentCell))
            {
                Debug.LogError($"Null neighbour encountered when moving {StepDirection}", (UnityEngine.Object)currentCell);
                throw new NullReferenceException();
            }

            if (currentCell.Equals(targetCell))
            {
                foundBresenhamsLine = true;
                return true;
            }

            if (currentCell.CellType == CellType.Obstacle)
            {
                foundBresenhamsLine = false;
                return true;
            }

            line.Add(currentCell);
            return false;
        }
    }
    private int EuclideanDistance(IGridCell startCell, IGridCell endCell)
    {
        int dx = Mathf.Abs(startCell.CellCoordinates.x - endCell.CellCoordinates.x);
        int dy = Mathf.Abs(startCell.CellCoordinates.y - endCell.CellCoordinates.y);
        return dx + dy;
    }
    private static int EvaluateAttackPosition(IGridCell x, IGridCell y)
    {
        if (ReferenceEquals(x, y)) return 0;
        if (x == null) return -1;
        if (y == null) return 1;

        int rangeCompare = x.PathfindingData.RangeData.Distance.CompareTo(y.PathfindingData.RangeData.Distance);
        if (rangeCompare != 0) return rangeCompare;
        int attackCompare = x.PathfindingData.AttackData.Distance.CompareTo(y.PathfindingData.AttackData.Distance);
        if (attackCompare != 0) return attackCompare;
        return x.GetHashCode().CompareTo(y.GetHashCode());
    }
}