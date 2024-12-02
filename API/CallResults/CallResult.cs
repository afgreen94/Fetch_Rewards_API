namespace API.CallResults
{
    public class CallResult
    {
        private readonly bool failed;
        private readonly string errorText;

        public bool Failed => failed;
        public string ErrorText => errorText;

        public CallResult() { }
        public CallResult(bool failed = false, string errorText = "") { this.failed = failed; this.errorText = errorText; }
    }


    public class CallResult<T> : CallResult
    {
        private readonly T value;

        public T Value => value;

        public CallResult(bool failed, string errorText) : base(failed, errorText) { }
        public CallResult(T value) { this.value = value; }
    }
}
