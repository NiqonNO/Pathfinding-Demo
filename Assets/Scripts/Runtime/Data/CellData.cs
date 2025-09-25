using System;
using UnityEngine;

[Serializable]
public struct CellData
{
    [field: SerializeField]
    public CellType CellType { get; set; }
    
    [field: SerializeField]
    public UnitSpawnType UnitToUnitSpawn { get; set; }
}