using System;
using System.Collections.Generic;
using UnityEngine;

public class PathfindingHandler
{
    private readonly HashSet<IGridCell> BFS_VisitedCells = new();
    private readonly HashSet<IGridCell> AStart_VisitedCells = new();

    public bool HaveRange { get; private set; }
    public bool HavePath { get; private set; }

    public void ShowRange(IGridCell selectedCell)
    {
        if (HaveRange) ClearRange();
            
        GetMovementRange_BFS(selectedCell);
        HaveRange = true;
    }
    public void ClearRange()
    {
        if (!HaveRange) return;
        
        foreach (var cells in BFS_VisitedCells)
        {
            cells.PathfindingData.ClearRangeData();
        }
        BFS_VisitedCells.Clear();
        HaveRange = false;
    }
    
    public void ShowMovePath(IGridCell selectedCell, IGridCell hoveredCell)
    {
        if (HavePath) ClearMovePath();
        
        FindPath_AStar(selectedCell, hoveredCell);
        HavePath = true;
    }
    public void ShowAttackPath(IGridCell selectedCell, IGridCell targetCell)
    {
    }
    public void ClearMovePath()
    {
        if (!HavePath) return;
        
        foreach (var cells in AStart_VisitedCells)
        {
            cells.PathfindingData.ClearMovementData();
        }
        AStart_VisitedCells.Clear();
        HavePath = false;
    }
    
    private void GetMovementRange_BFS(IGridCell startCell)
    {
        Queue<IGridCell> frontier = new Queue<IGridCell>();
        frontier.Enqueue(startCell);
        BFS_VisitedCells.Add(startCell);
        
        while (frontier.Count > 0)
        {
            IGridCell current = frontier.Dequeue();
            if(current.PathfindingData.RangeData.Distance >= startCell.Unit.MoveRange) continue;
            
            foreach (CellDirection direction in Enum.GetValues(typeof(CellDirection)))
            {
                CheckNeighbors(current, direction);
            }
        }
        return;

        void CheckNeighbors(IGridCell current, CellDirection direction)
        {
            if (!current.TryGetNeighbor(direction, out var neighbor) ||
                BFS_VisitedCells.Contains(neighbor) ||
                neighbor.CellType != CellType.Traversable ||
                neighbor.Occupied) return;

            neighbor.PathfindingData.RangeData.Distance = current.PathfindingData.RangeData.Distance + 1;
            neighbor.PathfindingData.IsRange = true;

            frontier.Enqueue(neighbor);
            BFS_VisitedCells.Add(neighbor);
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
            if (current == endCell)
            {
                ReconstructPath(current);
                return;
            }

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
            }
            else
            {
                frontier.Enqueue(neighbor, neighbor.PathfindingData.MovementData.Estimation);
                AStart_VisitedCells.Add(neighbor);
            }
        }
        void ReconstructPath(IGridCell current)
        {
            while (current != null)
            {
                if (current.PathfindingData.MovementData.Distance <= startCell.Unit.MoveRange)
                {
                    current.PathfindingData.IsMovementPath = true;
                }

                current = current.PathfindingData.MovementData.Previous;
            }
        }
    }

    private int EuclideanDistance(IGridCell startCell, IGridCell endCell)
    {
        int dx = Mathf.Abs(startCell.CellCoordinates.x - endCell.CellCoordinates.x);
        int dy = Mathf.Abs(startCell.CellCoordinates.y - endCell.CellCoordinates.y);
        return dx + dy;
    }

    /*private void CalculateMovementRange(CellObject startCell)
    {

    }
    private void CalculateAttackRange(CellObject startCell)
    {
        for (int i = 0; i < Cells.Length; i++)
        {
            Cells[i].AttackDistance = startCell.DistanceTo(Cells[i]);
        }
    }
    private void ValidateAttackPositions()
    {
        for (int i = 0; i < Cells.Length; i++)
        {
            Cells[i].CanAttack = false;

            if (Cells[i].AttackDistance > 10 ||
                Cells[i].MovementDistance > 6) continue;

            HashSet<CellObject> line = new();
            if(HaveCleanLineOfSight(Cells[i], EnemyCell, ref line))
            {
                Cells[i].CanAttack = true;
                foreach (var cell in line)
                {
                    cell.AttackPath = true;
                }
            }
        }
    }

    private bool HaveCleanLineOfSight(CellObject startCell, CellObject targetCell, ref HashSet<CellObject> line)
    {
        if (startCell.Equals(targetCell)) return false;

        int x0 = startCell.CellCoordinates.x;
        int y0 = startCell.CellCoordinates.y;
        int x1 = targetCell.CellCoordinates.x;
        int y1 = targetCell.CellCoordinates.y;

        CellObject currentCell = startCell;

        bool steep = Math.Abs(y1 - y0) > Math.Abs(x1 - x0);

        CellDirection ProgressDirection = CellDirection.E;
        CellDirection StepDirection = CellDirection.N;

        if (steep)
        {
            Swap(ref x0, ref y0);
            Swap(ref x1, ref y1);
            Swap(ref ProgressDirection, ref StepDirection);

        }
        if (x0 > x1)
        {
            Swap(ref x0, ref x1);
            Swap(ref y0, ref y1);
            Swap(ref currentCell, ref targetCell);
        }

        int dX = (x1 - x0);
        int dY = Math.Abs(y1 - y0);

        int err = (dX / 2);
        int ystep = (y0 < y1 ? 1 : -1);
        if(ystep<0) StepDirection = StepDirection.Opposite();
        int y = y0;

        for (int x = x0; x < x1; ++x)
        {
            if (currentCell.CellType == CellType.Obstacle) return false;

            err -= dY;
            if (err < 0)
            {
                y += ystep;
                err += dX;

                currentCell = currentCell.GetNeighbor(StepDirection);
                if (currentCell.Equals(targetCell)) return true;
                line.Add(currentCell);

                //Uncomment to allow corner traversing for Line of Sight
                //if (currentCell.CellType == CellType.Obstacle) return false;
            }

            currentCell = currentCell.GetNeighbor(ProgressDirection);
            line.Add(currentCell);
        }

        return true;

        void Swap<T>(ref T lhs, ref T rhs)
        {
            T temp;
            temp = lhs;
            lhs = rhs;
            rhs = temp;
        }
    }

    private void CalculateAttackRange(CellObject startCell, params CellDirection[] directions)
    {
        Queue<CellObject> frontier = new Queue<CellObject>();
        startCell.AttackDistance = 0;
        frontier.Enqueue(startCell);
        while (frontier.Count > 0)
        {
            CellObject current = frontier.Dequeue();
            if(current.AttackDistance >= 10) continue;

            foreach (var direction in directions)
            {
                CellObject neighbor = current.GetNeighbor(direction);
                if (neighbor == null ||
                    neighbor.CellType == CellType.Obstacle) continue;

                neighbor.AttackDistance = current.AttackDistance + 1;
                frontier.Enqueue(neighbor);
            }
        }
    }*/
}