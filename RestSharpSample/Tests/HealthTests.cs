using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using NUnit.Framework;
using RestSharp;
using RestSharpSample.Base;
using RestSharpSample.Helpers;
using System;
using System.Collections.Generic;

namespace PokeApiTesting.Tests
{
    /// <summary>
    /// Test class to comprehend all validations related to health checks of this API
    /// </summary>
    [TestFixture]
    public class HealthTests : TestBase
    {
        /// <summary>
        /// Health test each API endpoint listing functionality 
        /// to health check each one's pagination capability
        /// </summary>
        /// <param name="resourceName">Receives a resourceName string parameter from the
        /// IEnumerable GetAllResources TestCaseData generator</param>
        [Test,TestCaseSource("GetAllResources"),Parallelizable,
            Description("Health test each API endpoint listing functionality" +
            " to health check each one's pagination capability")]
        public void Check_Endpoint(string resourceName)
        {
            //Sends a GET request for the resourceName gotten from GetAllResoruces IEnumerale
            var response = restClient.Get(new RestRequest(resourceName));
            
            //Prepares an output of the summarization of the request and response for each test
            string output = ResponseSummarization.GetInfo(response);

            //Asserts the response was successful
            Assert.That(response.IsSuccessful, output);

            //Asserts the content body matches the expected jSchema for it, if not, grabs the errors
            Assert.That(JToken.Parse(response.Content).IsValid(apiPaginationjSchema, out IList<string> errorMessages)
                , output + "\n" + string.Join("\n", errorMessages));

            //Outputs the summarization of the request and response even if the test succeeded for logging purposes
            Console.WriteLine(output);
        }

        /// <summary>
        /// Health test the first data from each resource of the API endpoints
        /// </summary>
        /// <param name="resourceName">Receives a resourceName string parameter from the
        /// IEnumerable GetAllResources TestCaseData generator</param>
        [Test, TestCaseSource("GetAllResources"), Parallelizable,
            Description("Health test the first data from each resource of the API endpoints")]
        public void Check_Resource(string resourceName)
        {
            //Prepares a GET request for the first data from a resource API endpoint
            var response = restClient.Get(new RestRequest(resourceName + "/1"));
            
            //Prepares an output of the summarization of the request and response for each test
            string output = ResponseSummarization.GetInfo(response);

            //Asserts if it was successful
            Assert.That(response.IsSuccessful);

            //Outputs the summarization of the request and response even if the test succeeded for logging purposes
            Console.WriteLine(output);
        }
    }
}