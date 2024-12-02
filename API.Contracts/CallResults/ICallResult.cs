namespace API.Contracts.CallResults
{
    public interface ICallResult
    {
        bool Failed { get; }
        string ErrorText { get; }
    }

    public interface ICallResult<T> : ICallResult
    {
        T Value { get; }
    }
}
