using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class UITooltipController : MonoBehaviour
{
    [SerializeField] 
    private CanvasGroup TooltipBody;
    [SerializeField] 
    private TMP_Text TooltipLabel;

    private RectTransform TooltipTransform;
    private Canvas Canvas;
    
    void Awake()
    {
        TooltipTransform = TooltipBody.transform as RectTransform;
        Canvas = TooltipLabel.canvas;
    }
    
    private void LateUpdate()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            Canvas.transform as RectTransform,
            mousePos,
            Canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : Canvas.worldCamera,
            out Vector2 localPoint
        );
        
        Vector2 clamped = localPoint;
        //clamped.x = Mathf.Clamp(clamped.x, 0, Canvas.pixelRect.width - TooltipBody.rect.width);
        //clamped.y = Mathf.Clamp(clamped.y, 0, Canvas.pixelRect.height - TooltipBody.rect.height);

        TooltipTransform.anchoredPosition = clamped;
    }

    private void OnEnable()
    {
        MessagingHandler.OnMessageChanged += UpdateLabel;
    }

    private void OnDisable()
    {
        MessagingHandler.OnMessageChanged -= UpdateLabel;
    }

    private void UpdateLabel()
    {
        if (MessagingHandler.GetMessage(out var message))
        {
            TooltipBody.alpha = 1;
            TooltipLabel.text = message;
            return;
        }
        TooltipBody.alpha = 0;
    }
}