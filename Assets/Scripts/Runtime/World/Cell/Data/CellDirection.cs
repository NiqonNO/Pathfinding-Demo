public enum CellDirection
{
    N = 0,
    E = 1,
    S = 2,
    W = 3
}

public static class CellDirectionExtensions {

    public static CellDirection Opposite (this CellDirection direction) {
        return (int)direction < 2 ? direction + 2 : direction - 2;
    }
    
    public static CellDirection Previous (this CellDirection direction) {
        return direction == CellDirection.N ? CellDirection.E : direction - 1;
    }

    public static CellDirection Next (this CellDirection direction) {
        return direction == CellDirection.W ? CellDirection.N : direction + 1;
    }
}