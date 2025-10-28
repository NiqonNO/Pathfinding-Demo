using UnityEngine;

[CreateAssetMenu(fileName = "New Unit Settings", menuName = "Pathfinding Demo/Data/Unit", order = 2)]
public class UnitSettings : ScriptableObject
{
    private static readonly Color ErrorTint = Color.magenta;
    
    [field: SerializeField] 
    public int MoveRange { get; private set; } = 6;
    [field: SerializeField]
    public int AttackRange { get; private set; } = 10;
    
    [field: SerializeField]
    private Color NeutralTint { get; set; } = Color.white;
    [field: SerializeField]
    private Color FriendlyTint { get; set; } = Color.green;
    [field: SerializeField]
    private Color EnemyTint { get; set; } = Color.red;

    public Color GetUnitTint(UnitSpawnType type)
    {
        return type switch
        {
            UnitSpawnType.Friendly => FriendlyTint,
            UnitSpawnType.Enemy => EnemyTint,
            UnitSpawnType.None => NeutralTint,
            _ => ErrorTint
        };
    }
}