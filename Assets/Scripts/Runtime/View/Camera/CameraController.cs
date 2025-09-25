using Unity.Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour, ICamera
{
    [SerializeField]
    private CinemachineCamera VirtualCamera;
    private CinemachineGroupFraming CameraGroupComposer;

    [field: SerializeField] 
    public Camera GameCamera { get; private set; }
    private Transform CameraTarget;
    private Transform CameraTransform;
    
    [Header("Movement Sensitivity")]
    [SerializeField]
    private float MinMovementSensitivity = 1;
    [SerializeField]
    private float MaxMovementSensitivity = 10;
    
    [Header("Zoom Range")]
    [SerializeField]
    private float MinZoomValue = 10;
    [SerializeField]
    private float MaxZoomValue = 100;
    
    [Header("Zoom Sensitivity")]
    [SerializeField]
    private float ZoomSensitivity = 5;

    private Vector2 MaxPosition;
    private Vector2 MinPosition;
    
    private float CurrentZoom => CameraGroupComposer.OrthoSizeRange.x;
    
    private void Awake()
    {
        CameraTransform = VirtualCamera.transform;
        CameraGroupComposer = VirtualCamera.GetComponent<CinemachineGroupFraming>();
        CameraTarget = VirtualCamera.Follow;
    }
    
    public void SetBounds(Vector2 minPosition, Vector2 maxPosition)
    {
        MinPosition = minPosition;
        MaxPosition = maxPosition;
    }

    public void Zoom(float value)
    {
        float newOrthoSize = Mathf.Clamp(CurrentZoom - (value * ZoomSensitivity), MinZoomValue, MaxZoomValue);
        CameraGroupComposer.OrthoSizeRange.x = CameraGroupComposer.OrthoSizeRange.y = newOrthoSize;
        
    }
    public void MoveBy(Vector2 delta)
    {
        Vector3 rotation = CameraTransform.eulerAngles;

        rotation.x = 0.0f;
        rotation.z = 0.0f;

        float movementSensitivity = Mathf.Lerp(MinMovementSensitivity, MaxMovementSensitivity,
            Mathf.InverseLerp(MinZoomValue, MaxZoomValue, CurrentZoom));
            
        Vector3 delta3D = new Vector3(delta.x, 0.0f, delta.y) * movementSensitivity;

        delta3D = Quaternion.Euler(rotation) * delta3D;

        Vector3 newPosition = CameraTarget.position;

        newPosition.x += delta3D.x;
        newPosition.z += delta3D.z;

        newPosition.x = Mathf.Clamp(newPosition.x, MinPosition.x, MaxPosition.x);
        newPosition.z = Mathf.Clamp(newPosition.z, MinPosition.y, MaxPosition.y);

        CameraTarget.position = newPosition;
    }
    public void MoveTo(Vector2 position)
    {
        position.x = Mathf.Clamp(position.x, MinPosition.x, MaxPosition.x);
        position.y = Mathf.Clamp(position.y, MinPosition.y, MaxPosition.y);

        CameraTarget.position = new Vector3(position.x, 0.0f, position.y);
    }
}