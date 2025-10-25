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

    private CellSettings CellSettings;

    public void SetSettings(CellSettings cellSettings, CellType cellType)
    {
        CellSettings = cellSettings;
        SetSize(CellSettings.CellSize);
        SetCellType(cellType);
    }

    private void SetSize(float cellSize)
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

    private void SetCellType(CellType cellType)
    {
        Fill.color = CellSettings.GetFillColor(cellType);
    }

    public void UpdateDisplay(CellPathfindingData data)
    {
        Outline.color = Color.clear;
        Label.text = string.Empty;
        Label.color = Color.black;

        if (data.IsVisibility)
        {
            Outline.color = CellSettings.AttackHighlightColor;
            Label.text = data.AttackRangeData.DisplayText;
            Label.color = data.AttackRangeData.DisplayColor;
        }
        else if (data.IsMovementPath)
        {
            Outline.color = CellSettings.MovementHighlightColor;
            Label.text = data.MovePathData.DisplayText;
            Label.color = data.MovePathData.DisplayColor;
        }
        else if (data.IsRange)
        {
            Outline.color = CellSettings.RangeHighlightColor;
            Label.text = data.MoveRangeData.DisplayText;
            Label.color = data.MoveRangeData.DisplayColor;
        }
    }
}