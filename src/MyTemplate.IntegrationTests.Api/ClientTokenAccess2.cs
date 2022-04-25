using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Aertssen.Framework.Api.ServiceAgents;
using Aertssen.Framework.Core.Configurations;
using Aertssen.Framework.Core.Extensions;
using IdentityModel.Client;
using Microsoft.Extensions.Logging;

namespace IM.Scharnier.IntegrationTests.Api
{
    /// <summary>
    /// Client Token Access to request various token from the OpenIdConnect Authorization Server
    /// </summary>
    // ReSharper disable once UnusedMember.Global
    public class ClientTokenAccess2 : IClientTokenAccess
    {
        private readonly ILogger<ClientTokenAccess2> _logger;

        //private readonly IAuthenticationSettings _authenticationSettings;
        private readonly IAzureAdSettings _azureAdSettings;
        private readonly DiscoveryCache _discoveryCache;

        private const string TokenType = "urn:ietf:params:oauth:token-type:access_token";
        /// <summary>
        /// We use a static HttpClient that will be shared by all ClientTokenAccess instances.
        /// According to <see ref="https://aspnetmonsters.com/2016/08/2016-08-27-httpclientwrong/"/> it is best practice to reuse the same HttpClient for all calls.
        /// </summary>
        private static readonly HttpClient Client = new HttpClient();

        /// <summary>
        /// Constructor
        /// </summary>
        public ClientTokenAccess2(IAzureAdSettings azureAdSettings, ILogger<ClientTokenAccess2> logger)
        {
            _azureAdSettings = azureAdSettings;
            _logger = logger;

            _discoveryCache = new DiscoveryCache($"{_azureAdSettings.Instance}/{_azureAdSettings.TenantId}/v2.0",
                new DiscoveryPolicy
                {
                    //ValidateIssuerName = false,
                    ValidateEndpoints = false // HACK allow other microsoft domains in here
                });
        }

        /// <inheritDoc/>
        /// <exception cref="TaskCanceledException">The task was canceled.</exception>
        /// <exception cref="OperationCanceledException">The token has had cancellation requested.</exception>
        /// <exception cref="ObjectDisposedException">The associated <see cref="CancellationTokenSource"></see> has been disposed.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="requestScope"/> is <see langword="null"/> or empty</exception>
        public TokenResponse GetClientAccessToken(string requestScope, string audience, CancellationToken cancellationToken = default)
        {
            // get the well-known meta-data document (from cache if possible, otherwise via new request)
            return AsyncHelper.RunSync(() => GetClientAccessTokenAsync(requestScope, audience, cancellationToken), cancellationToken);
        }

        /// <inheritDoc/>
        /// <exception cref="OperationCanceledException">The token has had cancellation requested.</exception>
        /// <exception cref="ObjectDisposedException">The associated <see cref="CancellationTokenSource"></see> has been disposed.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="requestScope"/> is <see langword="null"/> or empty</exception>
        public async Task<TokenResponse> GetClientAccessTokenAsync(string requestScope, string audience, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(requestScope))
                throw new ArgumentNullException(nameof(requestScope));
            //Check if cancellation is requested
            cancellationToken.ThrowIfCancellationRequested();
            var issuerTokenAddress = await GetTokenIssuerAddressAsync(cancellationToken).ConfigureAwait(false);

            //Check if cancellation is requested
            cancellationToken.ThrowIfCancellationRequested();
            var tokenResponse = await Client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                ClientCredentialStyle = ClientCredentialStyle.AuthorizationHeader,
                Address = issuerTokenAddress,
                ClientId = _azureAdSettings.ClientId,
                ClientSecret = _azureAdSettings.ClientSecret,
                Scope = requestScope,
                //Resource = new List<string>()
                //{
                //    audience
                //}
            }, cancellationToken).ConfigureAwait(false);
            return tokenResponse;
        }


        /// <inheritDoc/>
        /// <exception cref="TaskCanceledException">The task was canceled.</exception>
        /// <exception cref="ObjectDisposedException">The provided <see cref="CancellationToken"></see> has already been disposed.</exception>
        /// <exception cref="OperationCanceledException">The token has had cancellation requested.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="refreshToken"/> is <see langword="null"/> or empty</exception>
        /// <exception cref="UnauthorizedAccessException">Failed to refresh the access token</exception>
        public TokenResponse RenewAccessToken(string refreshToken, CancellationToken cancellationToken = default)
        {
            return AsyncHelper.RunSync(() => RenewAccessTokenAsync(refreshToken, cancellationToken), cancellationToken);
        }

        /// <inheritDoc/>
        /// <exception cref="OperationCanceledException">The token has had cancellation requested.</exception>
        /// <exception cref="ObjectDisposedException">The associated <see cref="CancellationTokenSource"></see> has been disposed.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="refreshToken"/> is <see langword="null"/> or empty</exception>
        /// <exception cref="UnauthorizedAccessException">Failed to refresh the access token</exception>
        public async Task<TokenResponse> RenewAccessTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(refreshToken))
                throw new ArgumentNullException(nameof(refreshToken));
            var tokenIssuerAddress = await GetTokenIssuerAddressAsync(cancellationToken).ConfigureAwait(false);
            //Check if cancellation is requested
            cancellationToken.ThrowIfCancellationRequested();
            var tokenResponse = await Client
                .RequestRefreshTokenAsync(
                    new RefreshTokenRequest
                    {
                        ClientCredentialStyle = ClientCredentialStyle.AuthorizationHeader,
                        Address = tokenIssuerAddress,
                        ClientId = _azureAdSettings.ClientId,
                        ClientSecret = _azureAdSettings.ClientSecret,
                        RefreshToken = refreshToken
                    }, cancellationToken).ConfigureAwait(false);
            if (tokenResponse.IsError)
            {
                throw new UnauthorizedAccessException("Failed to refresh the access token: " + tokenResponse.ErrorDescription);
            }
            return tokenResponse;
        }

        private async Task<string> GetTokenIssuerAddressAsync(CancellationToken cancellationToken)
        {
            //Check if cancellation is requested
            cancellationToken.ThrowIfCancellationRequested();
            var discoveryResponse = await _discoveryCache.GetAsync().ConfigureAwait(false);
            // TODO Add this again
            //discoveryResponse.VerifyDiscoveryResponse(_azureAdSettings.OpenIdConnectDiscoveryDocument);
            return discoveryResponse.TokenEndpoint;
        }


        /// <inheritDoc/>
        //public TokenResponse GetResourceOwnerClientAccesstoken(string requestScope, string username, string password)
        //{
        //    // get the well-known metadata document (from cache if possible, otherwise via new request)
        //    var discoveryResponse = _discoveryCache.GetAsync().Result;
        //    var issuerTokenAddress = discoveryResponse.TokenEndpoint;

        //    var tokenClient = new HttpClient();

        //    var tokenResponse = tokenClient.RequestPasswordTokenAsync(
        //        new PasswordTokenRequest
        //        {
        //            UserName = username,
        //            Password = password,
        //            ClientId = _authenticationSettings.AppClientId,
        //            ClientSecret = _authenticationSettings.AppClientSecret,
        //            Scope = requestScope,
        //            Address = issuerTokenAddress
        //        }).GetAwaiter().GetResult();

        //    _logger?.LogDebug($"tokenResponse - Error: {tokenResponse.Error}");
        //    _logger?.LogDebug($"tokenResponse - AccessToken: {tokenResponse.AccessToken}");

        //    if (tokenResponse.AccessToken == null)
        //        throw new Exception($"Could not retrieve accestoken for {username} and {password}");
        //    return tokenResponse;
        //}
    }
}