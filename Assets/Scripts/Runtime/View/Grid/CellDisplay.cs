using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CellDisplay : MonoBehaviour
{
    [SerializeField] 
    private RectTransform Canvas;
    [SerializeField] 
    private Transform Geometry;
    [Space]
    [SerializeField] 
    private TMP_Text Label;
    [SerializeField] 
    private Image Fill;
    
    public void SetSize(float cellSize)
    {
        Vector3 canvasSize = Canvas.sizeDelta;
        canvasSize.x = cellSize;
        canvasSize.y = cellSize;
        Canvas.sizeDelta = canvasSize;
        
        Vector3 geometryScale = Geometry.localScale;
        geometryScale.x = cellSize;
        geometryScale.z = cellSize;
        Geometry.localScale = geometryScale;
    }

    public void SetCellType(CellType cellType)
    {
        Fill.color = cellType switch
        {
            CellType.Traversable => Color.green,
            CellType.Obstacle => Color.red,
            CellType.Cover => Color.blue,
            _ => Fill.color
        };
    }

    public void UpdateLabel(string content, Color color)
    {
        Label.text = content;
        Label.color = color;
    }
}