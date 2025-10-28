using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MoveOrder : UnitOrder
{
    private const float MovementSpeed = 5.0f;
    private const float RotationSpeed = 1000.0f;
    private const float ErrorMargin = 0.01f;
    
    private static readonly int Running = Animator.StringToHash("Speed");
    
    public override bool Completed { get; protected set; }

    private List<IGridCell> MovementPath;
    private int CurrentTargetIndex = 0;
    private Transform UnitTransform;
    private IAnimator UnitAnimator;

    public MoveOrder(List<IGridCell> movementPath)
    {
        MovementPath = new List<IGridCell>(movementPath);
    }
    
    public override void Initialize(IGridUnit myself)
    {
        UnitTransform = myself.Transform;
        UnitAnimator = myself.UnitAnimator;
        myself.AssignCell(MovementPath.Last());
        
        UnitAnimator.TrySetFloat(Running, MovementSpeed);
    }

    public override void Update()
    {
        Vector3 targetPos = MovementPath[CurrentTargetIndex].Transform.position;
        UnitTransform.position = Vector3.MoveTowards(
            UnitTransform.position, targetPos,
            MovementSpeed * Time.deltaTime);

        Vector3 moveDirection = CurrentTargetIndex > 0
            ? targetPos - MovementPath[CurrentTargetIndex - 1].Transform.position
            : MovementPath[CurrentTargetIndex + 1].Transform.position - targetPos;
        
        Quaternion targetRot = Quaternion.LookRotation(moveDirection, Vector3.up);
        UnitTransform.rotation = Quaternion.RotateTowards(
            UnitTransform.rotation, targetRot,
            RotationSpeed * Time.deltaTime);

        if (!(Vector3.Distance(UnitTransform.position, targetPos) < ErrorMargin)) return;
        
        CurrentTargetIndex++;
        if (CurrentTargetIndex < MovementPath.Count) return;
        
        UnitTransform.position = targetPos;
        UnitTransform.rotation = targetRot;
        Completed = true;
    }

    public override void Finish()
    {
        UnitAnimator.TrySetFloat(Running, 0);
    }
}