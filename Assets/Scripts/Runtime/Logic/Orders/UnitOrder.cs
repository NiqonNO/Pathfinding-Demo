public abstract class UnitOrder
{
    public abstract bool Completed { get; protected set; }

    public abstract void Initialize(IGridUnit myself);
    public abstract void Update();
    public abstract void Finish();
}