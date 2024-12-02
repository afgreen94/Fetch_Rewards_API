using API.CallResults;
using API.Contracts;
using API.Contracts.CallResults;
using MS = Microsoft.Extensions.Configuration;

namespace API.Services
{
    //handles process requests (Receipts)
    //validation 
    //calculation of point value for "Recipt" object using "IReceiptPointValueCalculator"
    public class DefaultReceiptManager : IReceiptManager
    {
        private readonly IReceiptValidator receiptValidator;
        private readonly IReceiptPointValueCalculator receiptPointsValueCalculator;
        private readonly IConfiguration configuration;

        public DefaultReceiptManager(IReceiptValidator receiptValidator,
                                     IReceiptPointValueCalculator receiptPointValueCalculator,
                                     IConfiguration configuration)
        {
            this.receiptValidator = receiptValidator;
            this.receiptPointsValueCalculator = receiptPointValueCalculator;
            this.configuration = configuration;
        }

        public IValidateReceiptResult ValidateReceipt(Receipt receipt) => this.receiptValidator.ValidateReceipt(receipt);

        public ICalculatePointsValueForReceiptResult CalculatePointsValueForReceipt(Receipt receipt, bool suppressValidateReceipt = false)
        {
            if (!suppressValidateReceipt)
            {
                var validateResult = this.receiptValidator.ValidateReceipt(receipt);

                if (validateResult.Failed)
                    return new CalculatePointValueForReceiptResult(true, $"Invalid Receipt: {validateResult.ErrorText}");
            }

            var wrapper = new ReceiptPointValueWrapper() { Receipt = receipt };

            var calculateResult = this.receiptPointsValueCalculator.CalculatePointsValueForReceipt(wrapper.Receipt);

            if (calculateResult.Failed)
                return new CalculatePointValueForReceiptResult(true, $"Failed to Calculate Points Value: {calculateResult.ErrorText}");

            wrapper.Points = calculateResult.Value;

            return new CalculatePointValueForReceiptResult(wrapper);
        }


        //for manager settings 
        public interface IConfiguration { }
        public class Configuration(MS.IConfiguration configuration) : MicrosoftConfigurationWrapper(configuration), IConfiguration { }

    }

}
