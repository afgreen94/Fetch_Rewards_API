using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;
using API.Contracts;
using API.Services;
using Microsoft.Extensions.DependencyInjection;
using API.Contracts.CallResults;

namespace API.Test
{

    //unit testing
    //exposes test cases with expected "points" results for use by API.IntegrationTest.Console

    public partial class Tests
    {

        private const string AppsettingsPath = "appsettings.test.json";

        private IServiceProvider serviceProvider;
        private IConfiguration configuration;


        [Fact]
        public void ReceiptPointsValueCalculatorTests()
        {

            this.Initialize();

            using var scope = this.serviceProvider.CreateScope();

            var calculator = scope.ServiceProvider.GetRequiredService<IReceiptPointValueCalculator>();

            foreach(var testCase in TestCases)
            {
                var calculateResult = calculator.CalculatePointsValueForReceipt(testCase.Receipt);

                Assert.False(calculateResult.Failed);
                Assert.Equal(calculateResult.Value, testCase.Points);
            }
        }


        [Fact]
        public void ReceiptValidatorTests()
        {
            this.Initialize();

            using var scope = this.serviceProvider.CreateScope();

            var validator = scope.ServiceProvider.GetRequiredService<IReceiptValidator>();

            foreach(var testCase in TestCases)
            {
                var validateResult = validator.ValidateReceipt(testCase.Receipt);

                Assert.False(validateResult.Failed);
            }
        }



        [Fact]
        public void ReceiptManagerTests()
        {

            this.Initialize();

            using var scope = this.serviceProvider.CreateScope();

            var manager = scope.ServiceProvider.GetRequiredService<IReceiptManager>();

            foreach (var testCase in TestCases)
            {
                var calculateResult = manager.CalculatePointsValueForReceipt(testCase.Receipt, suppressValidateReceipt: false); //will also validate 

                Assert.False(calculateResult.Failed);
                Assert.Equal(calculateResult.Value.Points, testCase.Points);
            }
        }


        private void Initialize()
        {
            this.configuration = new ConfigurationBuilder().AddJsonFile(AppsettingsPath).Build();

            var serviceCollection = new ServiceCollection();

            new DefaultServiceRegistrar().RegisterServices(serviceCollection, this.configuration);

            this.serviceProvider = serviceCollection.BuildServiceProvider();
        }
    }
}