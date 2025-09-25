using UnityEngine;
using UnityEngine.Events;

public class UnitObject : MonoBehaviour, ISelectable
{
    [field: SerializeField]
    public UnityEvent OnSelect { get; set; }
    [field: SerializeField]
    public UnityEvent OnDeselect { get; set; }
    
    public void Initialize(UnitSpawnType unitType)
    {
    }

    public void Select()
    {
        OnSelect?.Invoke();
    }

    public void Deselect()
    {
        OnDeselect?.Invoke();
    }
}