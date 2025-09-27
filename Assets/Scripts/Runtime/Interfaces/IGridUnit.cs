 public interface IGridUnit : IWorldObject
 {
     public int MoveRange { get; }
     public int AttackRange { get; }
     
     public IGridCell Cell { get; }
     public void AssignCell(IGridCell gridUnit);
     
     public bool ValidForSelection { get; }
     public bool ValidForAttack { get; }
     public void Select();
     public void Deselect();
 }