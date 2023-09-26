namespace Core.StateMachines
{
    public interface IUpdateState : IExitableState
    {
        void Update();
    }
}