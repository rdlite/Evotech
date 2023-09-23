namespace Core.StateMachine
{
    public interface IPayloadState<TPayload> : IExitableState
    {
        void Enter(TPayload payload);
    }
}