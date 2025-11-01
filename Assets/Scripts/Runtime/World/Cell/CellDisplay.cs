using System.Collections;
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
    [SerializeField] 
    private Image Marker;
    
    private CellSettings CellSettings;
    private bool Selected;
    private Coroutine TouchAnimation;

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

    public void HoverStart()
    {
        Marker.color = Color.cyan;
    }
    public void HoverEnd()
    {
        Marker.color = Color.clear;
    }
    
    public void Touch()
    {
        if (TouchAnimation != null)
        {
            StopCoroutine(TouchAnimation);
        }

        TouchAnimation = StartCoroutine(Pulse());
    }

    IEnumerator Pulse()
    {
        float duration = 0.2f;
        Vector3 targetScale = Vector3.one * 1.3f;
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            Marker.pixelsPerUnitMultiplier = Mathf.Lerp(50, 25, t);
            Marker.transform.localScale = Vector3.Lerp(Vector3.one, targetScale, t*t);
            yield return null;
        }

        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            Marker.pixelsPerUnitMultiplier = Mathf.Lerp(25, 50, t);
            Marker.transform.localScale = Vector3.Lerp(targetScale, Vector3.one, Mathf.Sqrt(t));
            yield return null;
        }

        Marker.transform.localScale = Vector3.one;
        TouchAnimation = null;
    }

    public void SelectionStart()
    {
        Outline.color = Color.yellow;
        Selected = true;
    }
    public void SelectionEnd()
    {
        Outline.color = Color.clear;
        Selected = false;
    }
    
    public void UpdateDisplay(CellData data)
    {
        var cellColor = Color.clear;
        var cellText = string.Empty;
        var textColor = Color.black;

        if (data.MovementData.Valid)
        {
            cellText = data.MovementData.Distance.ToString();
        }
        
        if(data.AttackData.IsRange)
        {
            switch (data.AttackData.Visible)
            {
                case VisibilityState.Obscured:
                    break;
                case VisibilityState.Partial:
                    cellColor = CellSettings.VisibilityHighlightColor;
                    cellText = "X";
                    break;
                case VisibilityState.Visible:
                    cellColor = CellSettings.VisibilityHighlightColor;
                    break;
            }
        }
        
        if (data.MovementData.IsRange)
        {
            cellColor = CellSettings.RangeHighlightColor;
        }
        
        if (data.MovementData.IsPath)
        {
            cellColor = CellSettings.MovementHighlightColor;
            if(data.MovementData.IsOutOfRange)
                textColor = Color.red;
        }
        
        if (data.AttackData.IsPath)
        {
            cellColor = CellSettings.AttackHighlightColor;
        }
        
        if (Selected)
        {
            cellColor = Color.yellow;
        }
        
        Outline.color = cellColor;
        Label.text = cellText;
        Label.color = textColor;
    }
}