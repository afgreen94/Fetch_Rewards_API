using API.Contracts.CallResults;

namespace API.Contracts
{
    //Defines and applies rules for calculating point value for a Receipt 
    public interface IReceiptPointValueCalculator
    {
        ICalculatePointsForReceiptResult CalculatePointsValueForReceipt(Receipt receipt);

        interface ICalculatePointsForReceiptResult : ICallResult<int> { }
    }
}
