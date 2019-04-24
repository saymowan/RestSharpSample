using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using RestSharp;
using RestSharpSample.Base;
using RestSharpSample.Helpers;
using System;
using System.Collections.Generic;

namespace RestSharpSample.Tests
{
    /// <summary>
    /// Test class to comprehend all functional validations related to the Pokemon endpoint from PokeAPI
    /// </summary>
    [TestFixture]
    public class FunctionalPokemonTests :TestBase
    {
        /// <summary>
        /// Tests the functionality of listing the Pokemon endpoint by name instead of Id
        /// for this case, it's fixed for Pikachu but could've been dynamic just as the health checks were
        /// </summary>
        [Test,Description("Tests the functionality of listing the Pokemon endpoint by name instead of Id " +
            "for this case, it's fixed for Pikachu but could've been dynamic just as the health checks were")]
        public void Search_For_A_Pokemon_By_Its_Name()
        {
            //Prepares a GET request for the searching for a pokemon by its name
            var response = restClient.Get(new RestRequest("pokemon/pikachu"));

            //Prepares an output of the summarization of the request and response for each test
            string output = ResponseSummarization.GetInfo(response);

            //Asserts if the request was successful
            Assert.That(response.IsSuccessful, 
                "Expected true for response.IsSuccessful but status received was: " +
                response.StatusCode);

            //Parses response.Content to JToken for easier json manipulation
            var contentJToken = JToken.Parse(response.Content);

            //Asserts the returned pokemon is indeed Pikachu, if not returns which one it returned
            Assert.That(contentJToken["name"].ToString().Equals("pikachu"),
                "Expected returned pokemon should have been Pikachu but was: " +
                contentJToken["name"].ToString());

            //Outputs the summarization of the request and response even if the test succeeded for logging purposes
            Console.WriteLine(output);
        }
        /// <summary>
        /// Tests the concordance of the value for "location_area_encounters" in a Pokemon endpoint data
        /// agaisn't the data of possible pokemons to be encountered in the "location-area" endpoint data
        /// </summary>
        [Test,Description("Tests the concordance of the value for 'location_area_encounters' in a Pokemon endpoint data" +
            " agaisn't the data of possible pokemons to be encountered in the 'location-area' endpoint data")]
        public void Validate_Location_Area_Of_A_Pokemon()
        {
            //Prepares a GET request for the first data from a resource API endpoint
            var response = restClient.Get(new RestRequest("pokemon/pikachu"));

            //Asserts if the request was successful
            Assert.That(response.IsSuccessful,
                "Expected true for response.IsSuccessful but status received was: " +
                response.StatusCode + "\n\n" + 
                ResponseSummarization.GetInfo(response));

            //Gets the location area for Pikachu and prepares it for issuing a GET Request for its endpoint
            var location_area_encounters_Method = JToken.Parse(response.Content)["location_area_encounters"].ToString();
            location_area_encounters_Method = location_area_encounters_Method.Substring(location_area_encounters_Method.IndexOf("/v2/")+4);

            //Issues a GET Request for its locationAreas
            var locationAreaEncounterResponse = restClient.Get(new RestRequest(location_area_encounters_Method));

            //Asserts if the request was successful
            Assert.That(locationAreaEncounterResponse.IsSuccessful,
                "Expected true for response.IsSuccessful but status received was: " +
                locationAreaEncounterResponse.StatusCode + "\n\n" +
                ResponseSummarization.GetInfo(locationAreaEncounterResponse));

            //Validates if each Location Area indeed has chances of encountering Pikachu
            foreach(JToken locationArea in JToken.Parse(locationAreaEncounterResponse.Content))
            {
                //Issues a GET Request for each location area
                var locationAreaResponse = restClient.Get(new RestRequest(
                    locationArea["location_area"]["url"].ToString().Substring(locationArea["location_area"]["url"].ToString().IndexOf("/v2/")+4)
                    ));

                //Asserts if the request was successful
                Assert.That(locationAreaResponse.IsSuccessful,
                    "Expected true for response.IsSuccessful but status received was: " +
                    locationAreaResponse.StatusCode + "\n\n" +
                    ResponseSummarization.GetInfo(locationAreaResponse));

                //Asserts if the area indeed has chances of encountering Pikachu
                Assert.That(locationAreaResponse.Content.Contains("pikachu") &&
                    new List<JToken>(JToken.Parse(locationAreaResponse.Content).SelectTokens("$..pokemon.name")).Contains("pikachu"),
                    "Expected to find Pikachu as one of the Pokemons encountered in this location area but just encountered: \n" +
                    JsonConvert.SerializeObject(JToken.Parse(locationAreaResponse.Content).SelectTokens("$..pokemon.name"), Formatting.Indented));
            }
        }
    }
}
