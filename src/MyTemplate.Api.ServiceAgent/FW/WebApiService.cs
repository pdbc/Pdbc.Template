using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Aertssen.Framework.Api.Contracts;
using Aertssen.Framework.Api.ServiceAgents;
using Microsoft.Extensions.Logging;

namespace MyTemplate.Api.ServiceAgent.FW
{
    /// <summary>
    /// Base class for implementing WEB API Rest methods.  This service abstracts the <see cref="IWebApiClientProxy"/> from the AER Framework
    /// to include all features developed in there (OIDC security, ...)
    /// </summary>
    public abstract class WebApiService
    {
        private readonly string _routePrefix;
        private readonly ILogger<WebApiService> _logger;

        /// <summary>
        /// The web API client proxy to be used.
        /// </summary>
        protected Func<IWebApiClientProxy> WebApiClientFactory;

        /// <summary>
        /// The web API client proxy to be used.
        /// </summary>
        private IWebApiClientProxy _webApiClient;

        /// <summary>
        /// Creates a new web api client
        /// </summary>
        /// <returns></returns>
        protected IWebApiClientProxy GetWebApiClient()
        {
            if (_webApiClient != null)
            {
                // TODO Verify status of proxy => retry creation otherwise?
                return _webApiClient;
            }

            if (WebApiClientFactory == null)
                throw new ApplicationException("WebApiClient cannot be created");

            _webApiClient = WebApiClientFactory();

            return _webApiClient;
        }

        /// <summary>
        /// Constructor to create a
        /// </summary>
        /// <param name="webApiClientFactory"></param>
        /// <param name="routePrefix"></param>
        protected WebApiService(Func<IWebApiClientProxy> webApiClientFactory, string routePrefix, ILogger<WebApiService> logger)
        {
            WebApiClientFactory = webApiClientFactory;
            _routePrefix = routePrefix;
            _logger = logger;
        }


        /// <summary>
        /// Helper method to deserialize the WEB API response to a type specific Response object.
        /// </summary>
        /// <typeparam name="TResponse">The type of the response.</typeparam>
        /// <param name="result">The result.</param>
        /// <returns></returns>
        protected TResponse Deserialize<TResponse>(HttpResponseMessage result) where TResponse : AertssenResponse, new()
        {
            result.LogResponseInformation(_logger);

            // If we have an HTTP200 variant :
            if (result.StatusCode == HttpStatusCode.OK)
            {
                try
                {
                    return result.GetResponse<TResponse>();
                }
                catch (UnsupportedMediaTypeException)
                {
                    return result.GetDefaultErrorResponse<TResponse>("UnsupportedMediaTypeException");
                }
            }

            result.HandleFailedHttpResponse<TResponse>();
            var exception = result.BuildWebApiClientExceptionFor();
            throw exception;
        }

        protected TResponse DeserializeListResponse<TResponse, TItem>(HttpResponseMessage result) where TResponse : AertssenListResponse<TItem>, new()
        {
            result.LogResponseInformation(_logger);

            // If we have an HTTP200 veriant :
            if (result.StatusCode == HttpStatusCode.OK)
            {
                try
                {
                    var response = result.GetResponse<ODataResponse<TItem>>();
                    return new TResponse()
                    {
                        Items = response.Items.AsQueryable(),
                        Notifications = new ValidationResult()
                    };
                }
                catch (UnsupportedMediaTypeException)
                {
                    return result.GetDefaultErrorResponse<TResponse>("UnsupportedMediaTypeException");
                }
            }


            result.HandleFailedHttpResponse<TResponse>();
            var exception = result.BuildWebApiClientExceptionFor();
            throw exception;
        }


        // TODO GET vs LIST (Get must provide Id, List is for overview screens)
        // TODO GET must not have oData variant
        #region GET & LIST

        /// <summary>
        /// REST API Get method to a specific route
        /// </summary>
        /// <typeparam name="TResponse">The type of the response.</typeparam>
        /// <param name="subroute">The subroute.</param>
        /// <returns></returns>
        protected TResponse Get<TResponse>(string subroute = null) where TResponse : AertssenResponse, new()
        {
            var route = AddSubroute(_routePrefix, subroute);

            _logger.LogInformation($"WebApiService.Get {route}");

            var result = GetWebApiClient().CallAsync(s => s.GetAsync(route))
                .GetAwaiter()
                .GetResult();

            return Deserialize<TResponse>(result);
        }

        /// <summary>
        /// REST API Get method to a specific route
        /// </summary>
        /// <typeparam name="TResponse">The type of the response.</typeparam>
        /// <param name="subroute">The subroute.</param>
        /// <returns></returns>
        protected async Task<TResponse> GetAsync<TResponse>(string subroute = null) where TResponse : AertssenResponse, new()
        {
            var route = AddSubroute(_routePrefix, subroute);

            _logger.LogInformation($"WebApiService.Get {route}");

            var result = await GetWebApiClient().CallAsync(s => s.GetAsync(route));

            return Deserialize<TResponse>(result);
        }

        protected TResponse Get<TRequest, TResponse>(TRequest request, string subroute = null)
            where TRequest : ITransformToUriQueryStringParameters
            where TResponse : AertssenResponse, new()
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            var route = SetupInitialRoute(request);
            route = AddSubroute(route, subroute);
            route = request.AddRequestQueryStringParametersToRoute(route);

            _logger.LogInformation($"WebApiService.List {route}");

            var result = GetWebApiClient().CallAsync(s => s.GetAsync(route))
                .GetAwaiter()
                .GetResult();

            return Deserialize<TResponse>(result);
        }

        protected async Task<TResponse> GetAsync<TRequest, TResponse>(TRequest request, string subroute = null)
            where TRequest : ITransformToUriQueryStringParameters
            where TResponse : AertssenResponse, new()
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var route = SetupInitialRoute(request);
            route = AddSubroute(route, subroute);
            route = request.AddRequestQueryStringParametersToRoute(route);

            _logger.LogInformation($"WebApiService.List {route}");

            var result = await GetWebApiClient().CallAsync(s => s.GetAsync(route));

            return Deserialize<TResponse>(result);
        }

        protected async Task<TResponse> GetAsyncOData<TRequest, TResponse, TItem>(TRequest request, string subroute = null)
            where TRequest : ITransformToUriQueryStringParameters
            where TResponse : AertssenListResponse<TItem>, new()
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var route = SetupInitialRouteOData(request);
            route = AddSubroute(route, subroute);
            route = request.AddRequestQueryStringParametersToRoute(route);

            _logger.LogInformation($"WebApiService.List {route}");

            var result = await GetWebApiClient().CallAsync(s => s.GetAsync(route));

            var response = DeserializeListResponse<TResponse, TItem>(result);
            return response;
        }

        #endregion

        #region POST
        protected TResponse Post<TRequest, TResponse>(TRequest request, string subRoute = "")
        {
            return PostAsync<TRequest, TResponse>(request, subRoute).GetAwaiter().GetResult();
        }

        ///// <summary>
        ///// REST API POST method to create an object, with optional a specific route
        ///// </summary>
        ///// <typeparam name="TRequest">The type of the request.</typeparam>
        ///// <typeparam name="TResponse">The type of the response.</typeparam>
        ///// <param name="request">The request.</param>
        ///// <param name="subRoute">The sub route.</param>
        ///// <returns></returns>
        protected Task<TResponse> PostAsync<TRequest, TResponse>(TRequest request, string subRoute = "")
        {
            var route = AddSubroute(_routePrefix, subRoute);
            _logger.LogInformation($"WebApiService.Create {route}");
            var result = GetWebApiClient().PostAsync<TRequest, TResponse>(route, request);

            return result;
        }
        #endregion

        #region PUT


        ///// <summary>
        ///// REST API POST method to create an object, with optional a specific route
        ///// </summary>
        ///// <typeparam name="TRequest">The type of the request.</typeparam>
        ///// <typeparam name="TResponse">The type of the response.</typeparam>
        ///// <param name="request">The request.</param>
        ///// <param name="subRoute">The sub route.</param>
        ///// <returns></returns>
        protected Task<TResponse> PutAsync<TRequest, TResponse>(TRequest request, string subRoute = "")
        {
            var route = AddSubroute(_routePrefix, subRoute);
            _logger.LogInformation($"WebApiService.Create {route}");
            var result = GetWebApiClient().PutAsync<TRequest, TResponse>(route, request);

            return result;
        }

        #endregion

        #region DELETE

        /// <summary>
        /// REST API DELETE method to delete an object, with optional a specific route.
        /// </summary>
        /// <typeparam name="TResponse">The type of the response.</typeparam>
        /// <param name="id">The identifier.</param>
        /// <param name="subRoute">The sub route.</param>
        /// <returns></returns>
        protected async Task<TResponse> DeleteAsync<TResponse>(long id, string subRoute = "")
            where TResponse : AertssenResponse, new()
        {
            var route = string.IsNullOrWhiteSpace(subRoute) ? $"{_routePrefix}/{id}" : $"{_routePrefix}/{subRoute}/{id}";
            _logger.LogInformation($"WebApiService.Delete {route}");

            var result = await GetWebApiClient().CallAsync(s => s.DeleteAsync(route));
            return Deserialize<TResponse>(result);
        }

        #endregion

        private static string AddIdentifier(string route, string identifier)
        {
            if (string.IsNullOrEmpty(route))
            {
                return identifier;
            }

            if (string.IsNullOrEmpty(identifier))
            {
                return route;
            }

            return $"{route}/{identifier}";
        }

        private static string AddSubroute(string route, string subroute)
        {
            if (string.IsNullOrEmpty(route))
            {
                return subroute;
            }

            if (string.IsNullOrEmpty(subroute))
            {
                return route;
            }

            return $"{route}/{subroute}";
        }

        private string SetupInitialRoute(object request)
        {
            var result = string.Empty;
            if (request is IProvideIdentifierForUrl provideIdentifierForUrlRequest)
            {
                result = AddIdentifier(_routePrefix, provideIdentifierForUrlRequest.GetIdentifier());
            }
            else
            {
                result = _routePrefix;
            }

            return result;
        }

        private string SetupInitialRouteOData(object request)
        {
            var route = $"odata/{_routePrefix}";

            string result;
            if (request is IProvideIdentifierForUrl provideIdentifierForUrlRequest)
            {
                result = AddIdentifier(route, provideIdentifierForUrlRequest.GetIdentifier());
            }
            else
            {
                result = route;
            }

            return result;
        }

        //private static string AddRequestQueryStringParameters(string route, ITransformToUriQueryStringParameters queryStringParameters)
        //{
        //    var queryString = queryStringParameters.ToUriQueryStringParameters();
        //    if (queryString.SafeTrim().IsNullOrEmpty())
        //        return route;

        //    if (route.EndsWith("/"))
        //    {
        //        route = route.Substring(0, route.Length - 1);
        //    }

        //    return $"{route}?{queryString}";
        //}
    }

    public class ODataResponse<TItem>
    {
        [Newtonsoft.Json.JsonProperty(propertyName: "value")]
        public List<TItem> Items { get; set; }

        [Newtonsoft.Json.JsonProperty(propertyName: "@odata.context")]
        public string Context { get; set; }

        [Newtonsoft.Json.JsonProperty(propertyName: "@odata.count")]
        public int Count { get; set; }
    }
}