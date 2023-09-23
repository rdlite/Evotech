namespace Core.StateMachine
{
    public interface IFixedUpdateState : IExitableState
    {
        void FixedUpdate();
    }
}