using UnityEngine;

[CreateAssetMenu(fileName = "New Unit Settings", menuName = "Pathfinding Demo/Data/Unit", order = 2)]
public class UnitSettings : ScriptableObject
{
    public int MoveRange = 6;
    public int AttackRange = 10;
    public float MoveSpeed = 0.5f;
}