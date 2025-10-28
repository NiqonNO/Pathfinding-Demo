using System;
using UnityEngine;

[Serializable]
public struct CellProperties
{
    [field: SerializeField]
    public CellType CellType { get; set; }
    
    [field: SerializeField]
    public UnitSpawnType UnitToUnitSpawn { get; set; }
}