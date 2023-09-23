namespace Core.StateMachine
{
    public interface IUpdateState : IExitableState
    {
        void Update();
    }
}