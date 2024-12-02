using Microsoft.AspNetCore.Components.Endpoints;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using API.Contracts;
using System.Text;
using MS = Microsoft.Extensions.Configuration;

namespace API.Controllers
{

    [ApiController]
    [Route("receipts")]
    public class ServiceController(IReceiptManager receiptManager, 
                                   ServiceController.ICache cache,
                                   ILogger<ServiceController> logger,
                                   IConfiguration configuration) : ControllerBase  
    {
        private readonly IReceiptManager receiptManager = receiptManager;
        private readonly ICache cache = cache;
        private readonly IConfiguration configuration;
        private readonly ILogger<ServiceController> logger = logger;

        [HttpPost]
        [Route("process")]
        public IActionResult ProcessAsync(Receipt receipt)
        {
            try
            {
                //validate process request
                var validateResult = this.receiptManager.ValidateReceipt(receipt);

                if (validateResult.Failed)
                {
                    var message = ErrorMessages.BuildInvalidProcessRequestMessage(validateResult.ErrorText);
                    this.logger.LogError(message);
                    return this.BadRequest(message);
                }

                //translate process request to receipt, calculate point value 
                var receiptPointsWrapperResult = this.receiptManager.CalculatePointsValueForReceipt(receipt, suppressValidateReceipt: true);

                if (receiptPointsWrapperResult.Failed)
                {
                    //not exposing calculate-related exceptions 
                    this.logger.LogError(receiptPointsWrapperResult.ErrorText);
                    return BadRequest();
                }

                //assign id, cache values 
                var idReceiptPointsWrapper = new IdReceiptPointsWrapper() 
                { 
                    Id = Guid.NewGuid(), 
                    Receipt = receiptPointsWrapperResult.Value.Receipt, 
                    Points = receiptPointsWrapperResult.Value.Points 
                };

                this.cache.Add(idReceiptPointsWrapper.Id, idReceiptPointsWrapper);

                //return id 
                return this.Ok(new ProcessCallResponse() { Id = idReceiptPointsWrapper.Id });
            }
            catch (Exception ex) 
            {
                //log exception
                //do not expose exception details to caller
                this.logger.LogCritical(ex.Message);
                return this.BadRequest(ex.Message);  
            }
        }

        [HttpGet(template: "{id}/points")]
        public IActionResult PointsAsync(Guid id)
        {
            try
            {
                //check if id corresponds to receipt in cache
                //return if true,
                //otherwise request is bad 
                if (!this.cache.TryGetValue(id, out var idReceiptPointsWrapper) || !idReceiptPointsWrapper.Points.HasValue)
                {
                    var message = ErrorMessages.BuildUnknownIdMessage(id);
                    this.logger.LogError(message);
                    return this.BadRequest(message);
                }

                return this.Ok(new PointsCallResponse() { Points = idReceiptPointsWrapper.Points.Value });
            }
            catch (Exception ex) 
            {
                this.logger.LogCritical(ex.Message);
                return this.BadRequest();
            } 
        }

        //for service controller settings
        public interface IConfiguration { }
        public class Configuration(MS.IConfiguration configuration) : MicrosoftConfigurationWrapper(configuration), IConfiguration { }



        //handles caching of receipts with ids and points values concurrently 
        public interface ICache 
        {
            void Add(Guid key, IdReceiptPointsWrapper value);
            bool TryGetValue(Guid key, out IdReceiptPointsWrapper value);
        }
        public class Cache : ICache 
        {
            private readonly Dictionary<Guid, IdReceiptPointsWrapper> innerDict = new();

            public void Add(Guid key, IdReceiptPointsWrapper wrapper)
            {
                lock (this.innerDict)
                    this.innerDict.Add(key, wrapper);
            }

            public bool TryGetValue(Guid key, out IdReceiptPointsWrapper wrapper) => this.innerDict.TryGetValue(key, out wrapper);
        }

        //error messages 
        private class ErrorMessages
        {
            private static readonly string invalidProcessRequestTemplate = "Invalid process request: {0}";
            private static readonly string unknownIdTemplate = "Id: {0} does not correspond to known receipt.";

            public static string BuildInvalidProcessRequestMessage(string processRequestValidatorErrorMessage) => string.Format(invalidProcessRequestTemplate, processRequestValidatorErrorMessage);
            public static string BuildUnknownIdMessage(Guid id) => string.Format(unknownIdTemplate, id);
        }
    }

}
