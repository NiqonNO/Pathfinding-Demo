using UnityEngine;

public class UnitDisplay : MonoBehaviour, IAnimator
{
    private static readonly int TintColor = Shader.PropertyToID("_BaseColor");
    
    [SerializeField] 
    private Renderer MeshRenderer;

    [field: SerializeField] 
    public Animator Animator { get; private set; }

    private MaterialPropertyBlock PropertyBlock;

    public void SetUnitColor(Color color)
    {
        if (PropertyBlock != null)
        {
            PropertyBlock.SetColor(TintColor, color);
            return;
        }

        PropertyBlock = new MaterialPropertyBlock();
        PropertyBlock.SetColor(TintColor, color);
        MeshRenderer.SetPropertyBlock(PropertyBlock);
    }
}
