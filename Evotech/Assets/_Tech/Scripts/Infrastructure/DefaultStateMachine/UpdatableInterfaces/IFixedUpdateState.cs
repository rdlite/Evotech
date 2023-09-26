namespace Core.StateMachines
{
    public interface IFixedUpdateState : IExitableState
    {
        void FixedUpdate();
    }
}