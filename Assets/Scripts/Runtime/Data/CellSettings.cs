using UnityEngine;

[CreateAssetMenu(fileName = "New Cell Settings", menuName = "Pathfinding Demo/Data/Cell", order = 0)]
public class CellSettings : ScriptableObject
{
   [SerializeField, Min(0.01f)] 
   private float _CellSize = 1f;
   public float CellSize => _CellSize;
}
