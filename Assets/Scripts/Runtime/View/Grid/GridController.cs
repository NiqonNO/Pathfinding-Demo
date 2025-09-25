using UnityEngine;

public class GridController : MonoBehaviour
{
    [SerializeField] 
    private GridSettings GridSettings;
    [SerializeField] 
    private CellObject CellPrefab;
    
    private CellObject[] Cells;
    
    public Vector2 SizeInUnits => new Vector2(GridSettings.GridWidth, GridSettings.GridHeight) * GridSettings.CellSize;
    public Vector2 MinPosition => new Vector2(-GridSettings.CellSize, -GridSettings.CellSize) / 2.0f;
    public Vector2 MaxPosition => MinPosition + SizeInUnits;

    public void GenerateLevel(UnitController unitController)
    {
        int width = GridSettings.GridWidth;
        int height = GridSettings.GridHeight;
        Cells = new CellObject[width * height];

        for (int y = 0, i = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                CreateCell(x, y, i++);
            }
        }
        
        void CreateCell(int coordX, int coordY, int index)
        {
            float cellSize = GridSettings.CellSize;
            Vector3 position = new Vector3(
                coordX * cellSize,
                0f,
                coordY * cellSize);
        
            CellObject cell = Cells[index] = Instantiate(CellPrefab, position, Quaternion.identity, transform);
            cell.Initialize(new Vector2Int(coordX, coordY), GridSettings.CellSettings, GridSettings.GetCellType(index));

            var spawnUnit = GridSettings.GetUnitSpawnType(index);
            if (spawnUnit != UnitSpawnType.None)
            {
                unitController.CreateUnit(cell, spawnUnit);
            }
        
            if(coordX > 0) cell.SetNeighbor(CellDirection.W, Cells[index - 1]);
            if(coordY > 0) cell.SetNeighbor(CellDirection.S, Cells[index - GridSettings.GridWidth]);
        }
    }
}