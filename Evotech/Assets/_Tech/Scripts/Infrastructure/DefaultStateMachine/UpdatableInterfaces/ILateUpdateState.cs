namespace Core.StateMachine
{
    public interface ILateUpdateState : IExitableState
    {
        void LateUpdate();
    }
}