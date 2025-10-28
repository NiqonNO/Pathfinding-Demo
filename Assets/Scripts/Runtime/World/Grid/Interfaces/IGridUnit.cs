 public interface IGridUnit : IWorldObject
 {
     public int MoveRange { get; }
     public int AttackRange { get; }
     
     public void AddOrder(UnitOrder order);
     public void RunOrders();
     
     public IGridCell Cell { get; }
     public void AssignCell(IGridCell gridUnit);
     
     public IAnimator UnitAnimator { get; }
     
     public bool ValidForSelection { get; }
     public bool ValidForAttack { get; }
     public void Select();
     public void Deselect();
 }