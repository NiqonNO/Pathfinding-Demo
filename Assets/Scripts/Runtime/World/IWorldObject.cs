using UnityEngine;

public interface IWorldObject
{
    public Transform Transform { get; }
    public void Destroy();
}