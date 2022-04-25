using System;
using System.Globalization;
using System.Net.Http;
using System.Security.Claims;
using Aertssen.Framework.Api.ServiceAgents;
using IdentityModel.Client;

namespace IM.Scharnier.IntegrationTests.Api.FW
{
    /// <summary>
    /// Extended WebApi Client proxy via .NET HttpClient, that will request access token from the Authorization Server,
    /// and add this access token to every web api call.
    /// </summary>
    public class ScharnierTokenAccessWebApiClientProxy : ScharnierWebApiClientProxy
    {
        private readonly IClientTokenAccess _clientTokenAccess;
        private readonly ITokenAccessRefresh _tokenAccessRefresh;
        private string _accessToken;
        private string _refreshToken;
        private DateTime? _expiresAt;

        private bool CanBeRefreshed => !string.IsNullOrEmpty(_refreshToken);

        private bool AccessTokenIsAlmostExpired => _expiresAt != null && DateTime.UtcNow > _expiresAt.Value.AddSeconds(-60);

        /// <summary>
        /// Initializes the <see cref="TokenAccessWebApiClientProxy"/>
        /// </summary>
        /// <param name="clientTokenAccess">IClientTokenAccess implementation to possibly call the Authorization Server to refresh the tokens</param>
        /// <param name="tokenAccessRefresh">ITokenAccessRefresh implementation (optional), to decide what needs to be done if a new access token and refresh token have been acquired.</param>
        /// <param name="claimsUser">The current ClaimsPrincipal, that should possess access and refresh token in claims</param>
        /// <param name="baseAddress">The Web Api Url</param>
        /// <param name="timeoutInMilliseconds">The timeout in milliseconds to wait before the request times out.</param>
        public ScharnierTokenAccessWebApiClientProxy(IClientTokenAccess clientTokenAccess, ITokenAccessRefresh tokenAccessRefresh,
            ClaimsPrincipal claimsUser,
            Uri baseAddress,
            int timeoutInMilliseconds = 30 * 1000)
            : base(baseAddress, timeoutInMilliseconds)
        {
            _clientTokenAccess = clientTokenAccess;
            _tokenAccessRefresh = tokenAccessRefresh;

            _accessToken = claimsUser?.FindFirst("access_token")?.Value;
            _refreshToken = claimsUser?.FindFirst("refresh_token")?.Value;

            var expiresAtDateString = claimsUser?.FindFirst("expires_at")?.Value;
            if (!string.IsNullOrWhiteSpace(expiresAtDateString))
            {
                _expiresAt = DateTime.Parse(expiresAtDateString, null, DateTimeStyles.RoundtripKind);
            }

        }

        /// <summary>
        /// Initializes the <see cref="TokenAccessWebApiClientProxy"/>
        /// </summary>
        /// <param name="clientTokenAccess">IClientTokenAccess implementation to possibly call the Authorization Server to refresh the tokens</param>
        /// <param name="tokenAccessRefresh">ITokenAccessRefresh implementation (optional), to decide what needs to be done if a new access token and refresh token have been acquired.</param>
        /// <param name="tokenResponse">A previously requested Token Response from the Authorization Server</param>
        /// <param name="baseAddress">The Web Api Url</param>
        /// <param name="timeoutInMilliseconds">The timeout to wait before the request times out.</param>

        public ScharnierTokenAccessWebApiClientProxy(IClientTokenAccess clientTokenAccess, ITokenAccessRefresh tokenAccessRefresh,
            TokenResponse tokenResponse, Uri baseAddress, int timeoutInMilliseconds = 30 * 1000)
            : base(baseAddress, timeoutInMilliseconds)
        {
            _clientTokenAccess = clientTokenAccess;
            _tokenAccessRefresh = tokenAccessRefresh;

            _accessToken = tokenResponse?.AccessToken;
            _refreshToken = tokenResponse?.RefreshToken;

            if (tokenResponse?.ExpiresIn != null)
                _expiresAt = DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn);
        }



        /// <summary>
        /// Backwards compatibility constrcutur SDFW6
        /// </summary>
        /// <param name="clientTokenAccess"></param>
        /// <param name="tokenAccessRefresh"></param>
        /// <param name="claimsUser"></param>
        /// <param name="baseAddress"></param>
        /// <param name="timeoutInMinutes"></param>
        public ScharnierTokenAccessWebApiClientProxy(IClientTokenAccess clientTokenAccess,
            ITokenAccessRefresh tokenAccessRefresh,
            ClaimsPrincipal claimsUser, String baseAddress, int timeoutInMilliseconds = 30 * 1000) : this(clientTokenAccess,
            tokenAccessRefresh, claimsUser, new Uri(baseAddress), timeoutInMilliseconds)
        {

        }

        /// <summary>
        /// Backwards compatibility constrcutur SDFW6
        /// </summary>
        /// <param name="clientTokenAccess"></param>
        /// <param name="tokenAccessRefresh"></param>
        /// <param name="tokenResponse"></param>
        /// <param name="baseAddress"></param>
        /// <param name="timeoutInMinutes"></param>
        public ScharnierTokenAccessWebApiClientProxy(IClientTokenAccess clientTokenAccess, ITokenAccessRefresh tokenAccessRefresh,
            TokenResponse tokenResponse, String baseAddress, int timeoutInMilliseconds = 30 * 1000) : this(clientTokenAccess,
            tokenAccessRefresh, tokenResponse, new Uri(baseAddress), timeoutInMilliseconds)
        {

        }



        /// <inheritDoc/>
        protected override HttpClient GetHttpClient()
        {
            var httpClient = base.GetHttpClient();

            // check: should we refresh the access token?
            if (AccessTokenIsAlmostExpired && CanBeRefreshed)
            {
                var tokenResponse = _clientTokenAccess.RenewAccessToken(_refreshToken);

                // set the new access token
                _accessToken = tokenResponse.AccessToken;
                // set the new refresh token
                _refreshToken = tokenResponse.RefreshToken;
                // set the new expiration time
                _expiresAt = DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn);

                // let caller decide what else needs to be done with these new tokens
                _tokenAccessRefresh?.SetNewTokensFrom(tokenResponse);
            }

            // add authorization header with the correct access token
            httpClient.SetBearerToken(_accessToken);

            return httpClient;
        }
    }
}