using API.Contracts.CallResults;

namespace API.Contracts
{
    //Defines and applies rules for validating a Receipt not already specified by the Receipt model attributes 
    public interface IReceiptValidator { IValidateReceiptResult ValidateReceipt (Receipt receipt); }
}
