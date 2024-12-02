using API.Contracts.CallResults;

namespace API.CallResults
{
    public class ValidateReceiptResult : CallResult, IValidateReceiptResult
    {
        public ValidateReceiptResult() : base() { }
        public ValidateReceiptResult(bool failed = false, string errorText = "") : base(failed, errorText) { }
    }
}
