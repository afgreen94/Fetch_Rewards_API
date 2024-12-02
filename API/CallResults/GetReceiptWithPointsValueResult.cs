using API.Contracts;
using API.Contracts.CallResults;


namespace API.CallResults
{
    public class CalculatePointValueForReceiptResult : CallResult<ReceiptPointValueWrapper>, ICalculatePointsValueForReceiptResult
    {
        public CalculatePointValueForReceiptResult(bool failed, string errorText) : base(failed, errorText) { }
        public CalculatePointValueForReceiptResult(ReceiptPointValueWrapper value) : base(value) { }
    }
}
