using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Aertssen.Framework.Api.ServiceAgents;
using Aertssen.Framework.Api.ServiceAgents.Exceptions;

namespace IM.Scharnier.IntegrationTests.Api.FW
{
    /// <summary>
    /// Basic WebApi Client proxy via .NET HttpClient, allowing type casts of the request/response to contract entities
    /// </summary>
    public class ScharnierWebApiClientProxy : IWebApiClientProxy
    {
        //private readonly ILog _log = LogManager.GetLogger<WebApiClientProxy>();

        private readonly Uri _baseAddress;
        private HttpClient _client;

        /// <summary>
        /// Default constructor
        /// </summary>
        public ScharnierWebApiClientProxy(Uri baseAddress, int timeoutInMilliseconds = 30 * 1000)
        {
            _baseAddress = baseAddress;
            ClientTimeout = TimeSpan.FromMilliseconds(timeoutInMilliseconds);
        }

        /// <summary>
        /// Gets or sets the timeout to wait before the request times out.
        /// </summary>
        public TimeSpan ClientTimeout { get; private set; }


        /// <summary>
        /// The default message handler to use for the HttpClient
        /// </summary>
        protected virtual HttpClientHandler GetHttpClientHandler()
        {
            return new HttpClientHandler
            {
                UseDefaultCredentials = true,

            };
        }

        /// <summary>
        /// The HttpClient responsible for sending/receiving HTTP requests and responses.
        /// </summary>
        /// <returns></returns>
        protected virtual HttpClient GetHttpClient()
        {
            var result = _client ??= new HttpClient(GetHttpClientHandler()) { Timeout = ClientTimeout, BaseAddress = _baseAddress };
            result.DefaultRequestHeaders.Add("Accept", "application/json");
            //result.DefR.Add("Content-Type", "application/json");

            return result;
        }

        /// <summary>
        /// Posts to the WebApi service method the JSON-formatted TRequest.
        /// If response indicates a Success or Validation status, returns the response read as TResponse.
        /// Throws an Exception with the stringified response if other HTTP status code returned.
        /// </summary>
        public virtual Task<TResponse> PostAsync<TRequest, TResponse>(string method, TRequest request)
        {
            return RequestAsync<TResponse>(GetHttpClient().PostAsJsonAsync(method, request));
        }

        /// <summary>
        /// Puts to the WebApi service method the JSON-formatted TRequest.
        /// If response indicates a Succes or Validation status, returns the response read as TResponse.
        /// Throws an Exception with the stringified response if other HTTP status code returned.
        /// </summary>
        public virtual Task<TResponse> PutAsync<TRequest, TResponse>(string method, TRequest request)
        {
            return RequestAsync<TResponse>(GetHttpClient().PutAsJsonAsync(method, request));
        }

        /// <summary>
        /// Calls the WebApi service with whatever HttpClient call you provide.
        /// </summary>
        public virtual Task<TResponse> CallAsync<TResponse>(Func<HttpClient, Task<TResponse>> httpClientCall)
        {
            return httpClientCall(GetHttpClient());
        }




        private async Task<TResponse> RequestAsync<TResponse>(Task<HttpResponseMessage> httpFunc)
        {
            var response = await httpFunc.ConfigureAwait(false);
            if (response.IsSuccessStatusCode)
            {
                return await Deserialize<TResponse>(response).ConfigureAwait(false);
            } 
            
            if (response.StatusCode.IsValidationFailedStatus())
            {
                var r = await Deserialize<TResponse>(response).ConfigureAwait(false);
                return r;
            }

            return await HandleInvalidResponse<TResponse>(response).ConfigureAwait(false);
        }



        /// <summary>
        /// Deserialization of the response to the expected response + error handling
        /// </summary>
        /// <param name="response"></param>
        /// <typeparam name="TResponse"></typeparam>
        /// <returns></returns>
        protected async Task<TResponse> Deserialize<TResponse>(HttpResponseMessage response)
        {
            try
            {
                //if (response.Content.)
                var result = await response.Content.ReadAsAsync<TResponse>()
                    .ConfigureAwait(false);

                return result;
            }
            catch (Exception ex)
            {
                return await HandleRequestCannotDeserialize<TResponse>(response).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Handling of invalid response (response status is not a success status code)
        /// </summary>
        /// <param name="response"></param>
        /// <typeparam name="TResponse"></typeparam>
        /// <returns></returns>
        protected virtual async Task<TResponse> HandleInvalidResponse<TResponse>(HttpResponseMessage response)
        {
            return await HandleRequestCannotDeserialize<TResponse>(response, response.StatusCode).ConfigureAwait(false);
        }

        /// <summary>
        /// Handling of invalid response (response status is not a success status code)
        /// </summary>
        /// <param name="response"></param>
        /// <typeparam name="TResponse"></typeparam>
        /// <returns></returns>
        protected virtual async Task<TResponse> HandleRequestCannotDeserialize<TResponse>(HttpResponseMessage response,
            HttpStatusCode statusCode = HttpStatusCode.InternalServerError)
        {
            var r = await response.Content.ReadAsStringAsync()
                .ConfigureAwait(false);

            var requestUri = response.RequestMessage.RequestUri;
            var requestMethod = response.RequestMessage.Method;

            var request = await response.RequestMessage.Content.ReadAsStringAsync()
                .ConfigureAwait(false);

            //_log.Warn($"HandleRequestCannotDeserialize: {requestUri} - {requestMethod} - {request}");
            //_log.Warn($"HandleRequestCannotDeserialize: {response.StatusCode} - {response.ReasonPhrase}");
            //_log.Warn($"HandleRequestCannotDeserialize: {r}");

            throw new WebApiClientException($"{r} - {request}") { HttpStatusCode = statusCode };
        }

    }
}
