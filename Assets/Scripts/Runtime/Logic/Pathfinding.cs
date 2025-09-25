using System.Collections.Generic;

public class Pathfinding
{
    private Dictionary<CellObject, CellPathfindingData> VisitedCells;
    public IEnumerable<CellObject> CellsInRange => VisitedCells.Keys;

    Pathfinding()
    {
        VisitedCells = new Dictionary<CellObject, CellPathfindingData>();
    }
    
    public void GetCellsInRange(CellObject startCell, int range)
    {
        Queue<CellObject> frontier = new Queue<CellObject>();
        VisitedCells.Add(startCell, new CellPathfindingData());
        while (frontier.Count > 0)
        {
            CellObject current = frontier.Dequeue();
            if(VisitedCells[current].MovementDistance >= 6) continue;
            
            for (CellDirection direction = CellDirection.N; direction <= CellDirection.W; direction++)
            {
                CellObject neighbor = current.GetNeighbor(direction);
                if (neighbor == null ||
                    VisitedCells.ContainsKey(neighbor) ||
                    neighbor.CellType != CellType.Traversable) continue;

                VisitedCells.Add(neighbor, new CellPathfindingData() {MovementDistance = VisitedCells[current].MovementDistance});
                frontier.Enqueue(neighbor);
            }
        }
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