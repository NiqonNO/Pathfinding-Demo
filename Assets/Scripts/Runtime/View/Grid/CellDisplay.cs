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
    [SerializeField] 
    private Image Outline;
    
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

    public void UpdateDisplay(CellPathfindingData data)
    {
        Outline.color = Color.clear;
        Label.text = string.Empty;
        if (data.IsAttack)
        {
            Outline.color = Color.red;
        }
        else if (data.IsRange || data.IsMovementPath)
        {
            Label.text = data.Distance.ToString();
            Outline.color = data.IsMovementPath ? Color.blue : Color.yellow;
        }
    }
}