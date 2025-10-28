using UnityEngine;


public interface ICamera
{
    public Camera GameCamera { get; }
    public void Zoom(float value);
    public void MoveBy(Vector2 delta);
    public void MoveTo(Vector2 position);
}