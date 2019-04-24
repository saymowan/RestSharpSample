using System;
using System.Collections;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using NUnit.Framework;
using RestSharp;
using RestSharpSample.Helpers;

namespace RestSharpSample.Base
{
    public class TestBase
    {
        //TODO: Prepare a report of the results of the test executions later with the annotations OneTimeSetUp, SetUp, TearDown, OneTimeTearDown

        /// <summary>
        /// RestClient object to have the base common URL to PokeAPI
        /// resources and be shared throughout this test class
        /// </summary>
        public static RestClient restClient = new RestClient("https://pokeapi.co/api/v2");

        /// <summary>
        /// Returns a JSchema to validate each of the API endpoint pagination functionality
        /// </summary>
        public static JSchema apiPaginationjSchema = JSchema.Parse(Resources.ValidationSchemas.apiPaginationjSchemaString);

        /// <summary>
        /// Test case data generator to health check all the API resources firstly
        /// </summary>
        public static IEnumerable GetAllResources
        {
            get
            {
                //Sends a request to the root of the API to list all its endpoints as expected
                var response = restClient.Execute(new RestRequest());

                //Checks if the above request succeded
                if (response.IsSuccessful)
                {
                    JToken jToken = JToken.Parse(response.Content);
                    foreach (JProperty property in jToken)
                    {
                        yield return new TestCaseData(property.Name.ToString());
                    }
                }
                //In case of the request failing, throw an exception to alert it
                else throw new Exception("Something went wrong trying to list all PokeApi resources: " +
                    ResponseSummarization.GetInfo(response));
            }
        }
    }
}
