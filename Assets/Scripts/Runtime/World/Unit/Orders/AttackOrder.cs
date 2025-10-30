using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AttackOrder : UnitOrder
{
    private const float RotationSpeed = 1000.0f;
    private const float ErrorMargin = 1f;
    
    public override bool Completed { get; protected set; }

    private List<ICellSearchData> AttackPath;
    private Transform UnitTransform;
    private Quaternion AttackRotation;
    
    public AttackOrder(List<ICellSearchData> attackPath)
    {
        AttackPath = new List<ICellSearchData>(attackPath);
    }
    
    public override void Initialize(IGridUnit myself)
    {
        UnitTransform = myself.Transform;
        Vector3 attackDirection = AttackPath.Last().Cell.GridObject.Transform.position - myself.Transform.position;
        AttackRotation = Quaternion.LookRotation(attackDirection, Vector3.up);
    }

    public override void Update()
    {
        UnitTransform.rotation = Quaternion.RotateTowards(
            UnitTransform.rotation, AttackRotation,
            RotationSpeed * Time.deltaTime);

        if (!(Quaternion.Angle(UnitTransform.rotation, AttackRotation) <= ErrorMargin)) return;
        
        UnitTransform.rotation = AttackRotation;
        Completed = true;
    }

    public override void Finish()
    {
        AttackPath.Last().Cell.GridObject.Unit.Destroy();
    }
}