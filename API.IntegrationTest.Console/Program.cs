using API;
using API.Contracts;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using API.Test;
using System.Linq.Expressions;
using Xunit.Sdk;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualBasic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using API.Services;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace API.IntegrationTest.Console
{


    //For integration testing with API
    //Runs test cases from API.Test against API endpoints
    //default api url: http://api:8080/receipts

    public class Program
    {

        private const string AppsettingsPath = "appsettings.integrationTest.json";
        private const string ConfigurationKey_EndpointSettings = "EndpointSettings";

        private readonly static ReceiptPointValueWrapper[] testCases = Tests.TestCases;
        private readonly static HttpClient client = new();

        private static string baseUrl;

        public static async Task Main()
        {

            Initialize();

            int i = 0;
            int logLevel = 0;

            Log($"INTEGRATION TESTS (using endpoint [{baseUrl}]):", logLevel++);

            try
            {
                for (; i < Tests.TestCases.Length; i++)
                {
                    var testCase = Tests.TestCases[i];

                    var processResponseCallResult = await PostProcessRequestReturnResultAsync(testCase.Receipt);

                    if(!AssertCallSuccessLogFailure(i, processResponseCallResult, logLevel + 1, out var processResponse)) continue;

                    var pointsResponseCallResult = await GetPointsRequestReturnResultAsync(processResponse.Id).ConfigureAwait(false);

                    if (!AssertCallSuccessLogFailure(i, pointsResponseCallResult, logLevel + 1, out var pointsResponse)) continue;

                    if (pointsResponse.Points != testCase.Points) { BuildAndLogFailureResultMessage(i, $"Points Mismatch Error: Expected: {testCase.Points} Actual: {pointsResponse.Points}\n", 1); continue; }

                    BuildAndLogSuccessResultMessage(i, 1);
                }
            }
            catch (Exception ex) { BuildAndLogFailureResultMessage(i, ex.Message, 1); }

        }

        private static void Initialize()
        {
            var config = new ConfigurationBuilder().AddJsonFile(AppsettingsPath).Build();

            var endpointSettings = config.GetSection(ConfigurationKey_EndpointSettings).Get<EndpointSettings>();

            baseUrl = endpointSettings.BuildUrl();
        }

        private static bool AssertCallSuccessLogFailure<TValue>(int i, HttpCallResult<TValue> callResult, int logLevel, out TValue value)
        {

            if (callResult.Failed)
            {
                BuildAndLogFailureResultMessage(i, callResult.ErrorText, 1);

                value = default;
                return false;
            }

            value = callResult.Value;
            return true;
        }

        #region Http

        private static async Task<HttpCallResult<ProcessCallResponse>> PostProcessRequestReturnResultAsync(Receipt receipt) => await SendMessageReturnResult<ProcessCallResponse>(
                                                                                                                                     new HttpRequestMessage()
                                                                                                                                     {
                                                                                                                                         Method = HttpMethod.Post,
                                                                                                                                         RequestUri = new($"{baseUrl}/process"),
                                                                                                                                         Content = new StringContent(JsonSerializer.Serialize(receipt), Encoding.UTF8, "application/json")
                                                                                                                                     }).ConfigureAwait(false);

        private static async Task<HttpCallResult<PointsCallResponse>> GetPointsRequestReturnResultAsync(Guid id) => await SendMessageReturnResult<PointsCallResponse>(
                                                                                                                    new HttpRequestMessage
                                                                                                                    {
                                                                                                                        Method = HttpMethod.Get,
                                                                                                                        RequestUri = new($"{baseUrl}/{id}/points")
                                                                                                                    }).ConfigureAwait(false);

        private static async Task<HttpCallResult<TResult>> SendMessageReturnResult<TResult>(HttpRequestMessage message)
        {
            var response = await client.SendAsync(message).ConfigureAwait(false);

            var responseString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            if(!response.IsSuccessStatusCode)
            {
                return new HttpCallResult<TResult> { ErrorText = BuildFailedHttpCallMessage(response.ReasonPhrase, responseString) };
            }
            else
                return new HttpCallResult<TResult>() { Value = JsonSerializer.Deserialize<TResult>(responseString) };
        }

        private class HttpCallResult
        {
            public string ErrorText { get; set; }

            public bool Failed => !string.IsNullOrEmpty(this.ErrorText);
        }

        private class HttpCallResult<T> : HttpCallResult
        {
            public T Value { get; set; }
        }

        #endregion Http

        #region Logging


        private static string BuildFailedHttpCallMessage(string reasonPhrase, string content) { return "Http Request Failed:\n\t {response.ReasonPhrase}\n\t{responseString}\n"; }
        private static void BuildAndLogSuccessResultMessage(int i, int logLevel) => BuildAndLogResultMessage(i, "Succeeded\n", logLevel);
        private static void BuildAndLogFailureResultMessage(int i, string errorText, int logLevel) => BuildAndLogResultMessage(i, $"Failed: {errorText}\n", logLevel);
        private static void BuildAndLogResultMessage(int i, string resultMessage, int logLevel)
        {
            //var testCase = Tests.TestCases[i];
            //var receiptJson = JsonSerializer.Serialize(testCase.Receipt);
            //var expectedPoints = testCase.Points;

            //Log($"Test Case {i}:", logLevel);
            //Log($"{receiptJson}", logLevel + 1);
            //Log($"Expected Points: {expectedPoints}", logLevel + 1);

            LogTestCase(i, logLevel);
            Log($"{resultMessage}", logLevel + 1);
        }
        private static void LogTestCase(int i, int logLevel)
        {
            var sb = new StringBuilder();

            var testCase = testCases[i];

            Log($"Test Case {i}:", logLevel);

            Log("Receipt", logLevel + 1);
            Log($"Retailer: {testCase.Receipt.Retailer}", logLevel + 2);
            Log($"Purchase Date: {testCase.Receipt.PurchaseDate.ToString("yyyy-mm-dd")}", logLevel + 2);
            Log($"Purchase Time: {testCase.Receipt.PurchaseTime}", logLevel + 2);
            Log("Items:", logLevel + 2);

            for (int j = 0; j < testCase.Receipt.Items.Length; j++)
            {
                Log($"Item {j}", logLevel + 3);
                Log($"Short Description: {testCase.Receipt.Items[j].ShortDescription}", logLevel + 4);
                Log($"Price: ${testCase.Receipt.Items[j].Price}", logLevel + 4);
            } 

            Log($"Expected Points: {testCase.Points}", logLevel + 1);
        }

        private static void Log(string message, int level)
        {
            for (int i = 0; i < level; i++) System.Console.Write('\t');
            System.Console.WriteLine(message);
        }

        #endregion Logging

        private class EndpointSettings
        {
            public string Protocol { get; set; }
            public string Domain { get; set; }
            public int Port { get; set; }
            public string Subdomain { get; set; }

            public string BuildUrl() => $"{Protocol}://{Domain}:{Port}/{Subdomain}";
        }
    }


    //for main 
    
    //var testSuite = new TestSuite(appsettingsPath, testCases, endpointSettings);

    //await testSuite.LogTestCasesAsync().ConfigureAwait(false);

    //await testSuite.RunUnitTestsAsync().ConfigureAwait(false);

    //await testSuite.RunIntegrationTestsAsync().ConfigureAwait(false);


    //public class TestSuite
    //{
    //    private readonly string appsettingsPath;
    //    private readonly ReceiptPointValueWrapper[] testCases;
    //    private readonly Uri baseUrl;

    //    private readonly HttpClient client = new();

    //    private IConfiguration configuration;
    //    private IServiceProvider serviceProvider;
    //    private bool isInitialized = false;


    //    public TestSuite(string appsettingsPath, ReceiptPointValueWrapper[] testCases, EndpointSettings endpointSettings)
    //    {
    //        this.appsettingsPath = appsettingsPath;
    //        this.testCases = testCases;
    //        this.baseUrl = new(endpointSettings.BuildUrl());
    //    }

    //    public ValueTask LogTestCasesAsync()
    //    {
    //        this.Log($"TEST CASES ({this.testCases.Length}):", 0);

    //        for (int i = 0; i < this.testCases.Length; i++)
    //        {
    //            this.Log($"TEST CASE {i}:", 1);
    //            this.Log(JsonSerializer.Serialize(this.testCases[i].Receipt), 2);
    //            this.Log($"Expected Points: {this.testCases[i].Points}", 2);
    //        }

    //        return new ValueTask();
    //    }

    //    public ValueTask RunUnitTestsAsync()
    //    {
    //        this.Initialize();

    //        var logLevel = 0;

    //        this.Log("UNIT TESTS:", logLevel);

    //        this.ReceiptPointsValueCalculatorTests(logLevel + 1);

    //        return new ValueTask();
    //    }


    //    public async ValueTask RunIntegrationTestsAsync()
    //    {
    //        this.Initialize();

    //        this.Log($"INTEGRATION TESTS (using endpoint: {this.baseUrl}):", 0);

    //        int i = 0;
    //        try
    //        {
    //            for (; i < Tests.TestCases.Length; i++)
    //            {
    //                var testCase = Tests.TestCases[i];

    //                var processResponse = await PostProcessRequestReturnResultAsync(testCase.Receipt);

    //                var pointsResponse = await GetPointsRequestReturnResultAsync(processResponse.Id).ConfigureAwait(false);

    //                if (pointsResponse.Points != testCase.Points) this.BuildLogFailureResultMessage(i, $"Points Mismatch: Expected: {testCase.Points} Actual: {pointsResponse.Points}\n");

    //                this.BuildLogSuccessResultMessage(i);
    //            }
    //        }
    //        catch (Exception ex) { this.BuildLogFailureResultMessage(i, ex.Message); }

    //    }

    //    private void ReceiptPointsValueCalculatorTests(int logLevel)
    //    {
    //        this.Log(nameof(ReceiptPointsValueCalculatorTests), logLevel);

    //        using var scope = this.serviceProvider.CreateScope();

    //        var calculator = scope.ServiceProvider.GetRequiredService<IReceiptPointValueCalculator>();

    //        for(int i = 0; i < this.testCases.Length; i++)
    //        {
    //            this.Log($"Test Case {i}: ", logLevel + 1);

    //            var calculateResult = calculator.CalculatePointsValueForReceipt(this.testCases[i].Receipt);

    //            if (calculateResult.Failed)
    //                this.BuildLogFailureResultMessage(i, calculateResult.ErrorText, logLevel + 1);

    //            if (calculateResult.Value != this.testCases[i].Points)
    //                this.BuildLogFailureResultMessage(i, $"Points Mismatch: Expected: {this.testCases[i].Points} Actual: {calculateResult.Value}", logLevel + 1);
    //            else
    //                this.BuildLogSuccessResultMessage(i, logLevel + 1);
    //        }
    //    }


    //    private void Initialize()
    //    {
    //        if (!this.isInitialized)
    //        {
    //            this.configuration = new ConfigurationBuilder().AddJsonFile(this.appsettingsPath).Build();

    //            var serviceCollection = new ServiceCollection();

    //            new DefaultServiceRegistrar().RegisterServices(serviceCollection, this.configuration);

    //            this.serviceProvider = serviceCollection.BuildServiceProvider();

    //            this.isInitialized = true;
    //        }
    //    }

    //}
}

