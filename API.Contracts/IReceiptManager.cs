using API.Contracts.CallResults;

namespace API.Contracts
{
    //manages api-provided receipt objects including:
        //validation of Receipts
        //Calculation of points values for Receipts
    public interface IReceiptManager 
    {
        IValidateReceiptResult ValidateReceipt(Receipt processRequest);
        ICalculatePointsValueForReceiptResult CalculatePointsValueForReceipt(Receipt processRequest, bool suppressValidateReceipt = false);
    }
}
