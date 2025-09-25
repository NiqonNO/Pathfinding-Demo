using System;
using UnityEngine;

[CreateAssetMenu(fileName = "New Grid Data", menuName = "Pathfinding Demo/Data/Grid", order = 1)]
public class GridSettings : ScriptableObject
{
    [SerializeField] 
    private CellSettings _CellSettings;
    public CellSettings CellSettings => _CellSettings;
    
    [SerializeField, Min(1)] 
    private int _GridWidth = 2;
    public int GridWidth
    {
        get => _GridWidth;
        private set
        {
            var lastWidth = GridWidth;
            _GridWidth = value;
            ResizeCellArray(lastWidth, GridHeight);
        }
    }

    [SerializeField, Min(1)] 
    private int _GridHeight = 2;
    public int GridHeight
    {
        get => _GridHeight;
        private set
        {
            var lastHeight = GridHeight;
            _GridHeight = value;
            ResizeCellArray(GridWidth, lastHeight);
        }
    }

    public float CellSize => CellSettings ? CellSettings.CellSize : 1.0f;
    private int GridSize => GridHeight * GridWidth;
    
    [SerializeField, HideInInspector]
    private CellData[] CellsData = new CellData[1];
    
    [field: SerializeField, HideInInspector]
    public int PlayerUnitCount { get; private set; }
    [field: SerializeField, HideInInspector]
    public int EnemyUnitCount { get; private set; }
    
    public void ResizeCellArray(int lastWidth, int lastHeight)
    {
        PlayerUnitCount = 0;
        EnemyUnitCount = 0;
        
        var resizedCellData = new CellData[GridSize];
        for (int x = 0; x < Math.Min(lastWidth, GridWidth); x++)
        {
            for (int y = 0; y < Math.Min(lastHeight, GridHeight); y++)
            {
                int oldIndex = lastWidth * y + x;
                int newIndex = GridWidth * y + x;
                resizedCellData[newIndex] = CellsData[oldIndex];
                IncreaseUnitCount(resizedCellData[newIndex].UnitToUnitSpawn);
            }
        }

        CellsData = resizedCellData;
    }

    public CellType GetCellType(int coordX, int coordY) => GetCellType(CoordinatesToIndex(coordX, coordY));
    public CellType GetCellType(int index) => CellsData[index].CellType;
    public void SetCellType(int coordX, int coordY, CellType type) => SetCellType(CoordinatesToIndex(coordX, coordY), type);
    public void SetCellType(int index, CellType type) => CellsData[index].CellType = type;

    public UnitSpawnType GetUnitSpawnType(int coordX, int coordY) => GetUnitSpawnType(CoordinatesToIndex(coordX, coordY));
    public UnitSpawnType GetUnitSpawnType(int index) => CellsData[index].UnitToUnitSpawn;

    public void SetUnitSpawnType(int coordX, int coordY, UnitSpawnType type) => SetUnitSpawnType(CoordinatesToIndex(coordX, coordY), type);
    public void SetUnitSpawnType(int index, UnitSpawnType type)
    {
        ReduceUnitCount(CellsData[index].UnitToUnitSpawn);
        CellsData[index].UnitToUnitSpawn = type;
        IncreaseUnitCount(CellsData[index].UnitToUnitSpawn);
    }

    private void IncreaseUnitCount(UnitSpawnType type) => ModifyUnitCount(type, 1);
    private void ReduceUnitCount(UnitSpawnType type) => ModifyUnitCount(type, -1);
    private void ModifyUnitCount(UnitSpawnType type, int modifyValue)
    {
        switch (type)
        {
            case UnitSpawnType.Friendly:
                PlayerUnitCount += modifyValue;
                break;
            case UnitSpawnType.Enemy:
                EnemyUnitCount += modifyValue;
                break;
        }
    }

    public int CoordinatesToIndex(int coordX, int coordY)
    {
        return GridWidth * coordY + coordX;
    }
}