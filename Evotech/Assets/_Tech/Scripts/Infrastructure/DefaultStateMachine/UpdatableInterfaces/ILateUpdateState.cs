namespace Core.StateMachines
{
    public interface ILateUpdateState : IExitableState
    {
        void LateUpdate();
    }
}