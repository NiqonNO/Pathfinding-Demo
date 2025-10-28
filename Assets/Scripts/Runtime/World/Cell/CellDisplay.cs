using System;
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
        Vector3 targetScale = Vector3.one * 1.5f;
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            Outline.transform.localScale = Vector3.Lerp(Vector3.one, targetScale, t*t);
            yield return null;
        }

        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            Outline.transform.localScale = Vector3.Lerp(targetScale, Vector3.one, Mathf.Sqrt(t));
            yield return null;
        }

        Outline.transform.localScale = Vector3.one;
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
        
        if (data.IsAttack)
        {
            cellColor = CellSettings.AttackHighlightColor;
        }
        else if (data.IsVisibility)
        {
            cellColor = CellSettings.VisibilityHighlightColor;
            if (data.IsRange)
            {
                cellText = "X";
            }
            else
            {
                cellColor.a = 0.5f;
            }
        }
        else if (data.IsMovementPath)
        {
            cellColor = CellSettings.MovementHighlightColor;
            cellText = data.MovementPathData.Distance.ToString();
            if(data.MovementPathData.IsOutOfRange)
                textColor = Color.red;
        }
        else if (data.IsRange)
        {
            cellColor = CellSettings.RangeHighlightColor;
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