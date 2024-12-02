using API.CallResults;
using API.Contracts;
using API.Contracts.CallResults;
using MS = Microsoft.Extensions.Configuration;

namespace API.Services
{
    public class DefaultReceiptValidator : IReceiptValidator
    {

        private readonly IConfiguration configuration;

        public DefaultReceiptValidator() { }

        public DefaultReceiptValidator(IConfiguration configuration) { this.configuration = configuration; }

        public IValidateReceiptResult ValidateReceipt(Receipt receipt)
        {

            //logic for validating receipts 

            return new ValidateReceiptResult();
        }


        //for validator settings 
        public interface IConfiguration { }
        public class Configuration(MS.IConfiguration configuration) : MicrosoftConfigurationWrapper(configuration), IConfiguration { }
    }
}
