using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionController : MonoBehaviour
{
    private const float MoveScale = 0.04f;
    private const float ScrollScale = 5f;
    private const int RaycastBuffer = 3;
    
    [SerializeField] 
    private InputReferences PlayerInput;

    private ICamera Camera;
    private IWorldObject UnderCursorObject;

    private SelectionHandler SelectionHandler;
    
    RaycastHit[] RaycastHits;
    
    private bool IsContextPressed = false;
    private bool ShouldUpdateObjectUnderCursor = false;
    
    public void Initialize(ICamera cameraController)
    {
        Camera = cameraController;
        RaycastHits = new RaycastHit[RaycastBuffer];
        SelectionHandler = new SelectionHandler();
    }
    
    private void Awake()
    {
        PlayerInput.moveMouseInput.action.performed += PlayerInput_OnMoveMouse;
        PlayerInput.moveCameraInput.action.performed += PlayerInput_OnMoveCamera;
        PlayerInput.contextInput.action.performed += PlayerInput_OnContext;
        PlayerInput.returnInput.action.performed += PlayerInput_OnReturn;
        PlayerInput.selectInput.action.performed += PlayerInput_OnSelect;
        PlayerInput.zoomCameraInput.action.performed += PlayerInput_OnZoomCamera;

        PlayerInput.moveMouseInput.action.Enable();
        PlayerInput.moveCameraInput.action.Enable();
        PlayerInput.contextInput.action.Enable();
        PlayerInput.returnInput.action.Enable();
        PlayerInput.selectInput.action.Enable();
        PlayerInput.zoomCameraInput.action.Enable();
    }
    
    private void Update()
    {
        if (ShouldUpdateObjectUnderCursor)
        {
            UpdateObjectUnderCursor();
            SelectionHandler.OnHover(UnderCursorObject);
            ShouldUpdateObjectUnderCursor = false;
        }
    }
    
    private void OnDestroy()
    {
        PlayerInput.moveMouseInput.action.performed -= PlayerInput_OnMoveMouse;
        PlayerInput.moveCameraInput.action.performed -= PlayerInput_OnMoveCamera;
        PlayerInput.contextInput.action.performed -= PlayerInput_OnContext;
        PlayerInput.returnInput.action.performed -= PlayerInput_OnReturn;
        PlayerInput.selectInput.action.performed -= PlayerInput_OnSelect;
        PlayerInput.zoomCameraInput.action.performed -= PlayerInput_OnZoomCamera;
    }
    
    private void UpdateObjectUnderCursor()
    {
        Ray ray = Camera.GameCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        var size = Physics.RaycastNonAlloc(ray, RaycastHits);

        UnderCursorObject = null;
        float distanceToObject = float.MaxValue;

        for (var i = 0; i < size; i++)
        {
            var hit = RaycastHits[i];
            if (hit.distance > distanceToObject) continue;

            if (!hit.collider.TryGetComponent(out IWorldObject hitObject))
            {
                if (hit.rigidbody is null ||
                    !hit.rigidbody.TryGetComponent(out hitObject)) continue;
            }

            distanceToObject = hit.distance;
            UnderCursorObject = hitObject;
        }
    }

    private void PlayerInput_OnZoomCamera(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.phase != InputActionPhase.Performed)
        {
            return;
        }

        float zoom = callbackContext.ReadValue<float>() * ScrollScale * Time.deltaTime;

        Camera?.Zoom(zoom);

        ShouldUpdateObjectUnderCursor = true;
    }
    private void PlayerInput_OnSelect(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.phase != InputActionPhase.Performed)
        {
            return;
        }

        if (callbackContext.ReadValueAsButton())
        {
            SelectionHandler.OnSelect(UnderCursorObject);
        }
    }
    private void PlayerInput_OnReturn(InputAction.CallbackContext callbackContext)
    {
        SelectionHandler.DeselectCurrent();
    }
    private void PlayerInput_OnContext(InputAction.CallbackContext callbackContext)
    {

        if (callbackContext.phase != InputActionPhase.Performed)
        {
            return;
        }

        IsContextPressed = callbackContext.ReadValueAsButton();
    }
    private void PlayerInput_OnMoveCamera(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.phase != InputActionPhase.Performed)
        {
            return;
        }

        if (!IsContextPressed)
        {
            return;
        }

        Vector2 delta = callbackContext.ReadValue<Vector2>() * MoveScale;

        Camera?.MoveBy(-delta);

        ShouldUpdateObjectUnderCursor = true;
    }
    private void PlayerInput_OnMoveMouse(InputAction.CallbackContext callbackContext)
    {

        if (callbackContext.phase != InputActionPhase.Performed)
        {
            return;
        }

        ShouldUpdateObjectUnderCursor = true;
    }
}
