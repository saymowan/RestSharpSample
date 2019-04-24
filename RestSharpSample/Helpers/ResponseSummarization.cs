using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace RestSharpSample.Helpers
{

    public static class ResponseSummarization
    {
        /// <summary>
        /// Transform an IRestResponse into a structured string for outputting useful information
        /// </summary>
        /// <param name="response">The IRestResponse object to transform</param>
        /// <returns>Returns the structured string for information output purposes</returns>
        public static string GetInfo(IRestResponse response)
        {
            //Gets part of request information
            string requestOutput =
                "Request Resource: " + response.Request.Resource +
                "\nMethod: " + response.Request.Method +
                "\nNumber of Attempts: " + response.Request.Attempts +
                "\nDefined Timeout: " + response.Request.Timeout +
                "\nParameters: \n";

            //Gets each of the request parameters and format them with indentation
            foreach (RestSharp.Parameter p in response.Request.Parameters)
                requestOutput += JsonConvert.SerializeObject(p, Formatting.Indented) + "\n";

            //Gests part of the response information
            string responseOutput =
                "\n\nResponse Server: " + response.Server +
                "\nUri: " + response.ResponseUri +
                "\nTransport Status: " + response.ResponseStatus +
                "\nStatus: (" + (int)response.StatusCode + ") - " + response.StatusDescription +
                "\nProtocol Version: " + response.ProtocolVersion +
                "\nContent Type: " + response.ContentType +
                "\nContent Encoding: " + response.ContentEncoding +
                "\nContent Length: " + response.ContentLength +
                "\n\nHeaders: \n";

            //Gets each of the request headers and format them with indentation
            foreach (RestSharp.Parameter p in response.Headers)
                requestOutput += JsonConvert.SerializeObject(p, Formatting.Indented) + "\n";

            //Gets the rest of the response information formating the content with indentation
            responseOutput +=
                "\n\nContent: " + JsonConvert.SerializeObject(
                                    JToken.Parse(response.Content),
                                    Formatting.Indented) +
                "\n\nError Exception: " + response.ErrorException +
                "\n\nError Message: " + response.ErrorMessage + "\n\n";

            //Returns a string with the combination of both
            return requestOutput + responseOutput;
        }

    }
}
