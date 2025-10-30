using System;

public enum OctantDirection
{
	NE = 0,
	EN = 1,
	ES = 2,
	SE = 3,
	SW = 4,
	WS = 5,
	WN = 6,
	NW = 7
}

public static class OctantDirectionExtensions
{
	public static (CellDirection, CellDirection) GetDirectionsForOctant(this OctantDirection octant)
	{
		return octant switch
		{
			OctantDirection.NE => (CellDirection.N, CellDirection.E),
			OctantDirection.EN => (CellDirection.E, CellDirection.N),
			OctantDirection.ES => (CellDirection.E, CellDirection.S),
			OctantDirection.SE => (CellDirection.S, CellDirection.E),
			OctantDirection.SW => (CellDirection.S, CellDirection.W),
			OctantDirection.WS => (CellDirection.W, CellDirection.S),
			OctantDirection.WN => (CellDirection.W, CellDirection.N),
			OctantDirection.NW => (CellDirection.N, CellDirection.W),
			_ => throw new ArgumentOutOfRangeException(nameof(octant), octant, null)
		};
	}
}