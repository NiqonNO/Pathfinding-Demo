using UnityEngine;

[CreateAssetMenu(fileName = "New Cell Settings", menuName = "Pathfinding Demo/Data/Cell", order = 0)]
public class CellSettings : ScriptableObject
{
   private static readonly Color ErrorTint = Color.magenta;

   [field: SerializeField, Min(0.01f)] 
   public float CellSize { get; private set; } = 1f;

   [field: SerializeField]
   private Color TraversableColor { get; set; } = Color.green;
   [field: SerializeField]
   private Color ObstacleColor { get; set; } = Color.red;
   [field: SerializeField]
   private Color CoverColor { get; set; } = Color.blue;
   
   [field: SerializeField]
   public Color RangeHighlightColor { get; private set; } = Color.yellow;
   [field: SerializeField]
   public Color MovementHighlightColor { get; private set; } = Color.green;
   [field: SerializeField]
   public Color AttackHighlightColor { get; private set; } = Color.red;

   public Color GetFillColor(CellType cellType)
   {
      return cellType switch
      {
         CellType.Traversable => TraversableColor,
         CellType.Obstacle => ObstacleColor,
         CellType.Cover => CoverColor,
         _ => ErrorTint
      };
   }
}
