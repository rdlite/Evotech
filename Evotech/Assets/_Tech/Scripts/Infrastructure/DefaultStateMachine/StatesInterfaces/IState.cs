namespace Core.StateMachines
{
    public interface IState : IExitableState
    {
        void Enter();
    }
}