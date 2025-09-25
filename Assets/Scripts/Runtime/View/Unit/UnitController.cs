using UnityEngine;

public class UnitController : MonoBehaviour
{
    [SerializeField] 
    private UnitObject UnitPrefab;
    
    public void CreateUnit(CellObject cell, UnitSpawnType unitType)
    {
        UnitObject unit = Instantiate(UnitPrefab, cell.transform.position, Quaternion.identity, transform);
        unit.Initialize(unitType);
        cell.OnTouch.AddListener(unit.Select);
    }
}