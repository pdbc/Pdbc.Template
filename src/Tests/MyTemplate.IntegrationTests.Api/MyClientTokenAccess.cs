using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Aertssen.Framework.Api.ServiceAgents.Configuration;
using Aertssen.Framework.Api.ServiceAgents.Exceptions;
using Aertssen.Framework.Api.ServiceAgents.Extensions;
using Aertssen.Framework.Core.Extensions;
using IdentityModel.Client;
using Microsoft.Extensions.Logging;

namespace IM.Scharnier.IntegrationTests.Api
{
    /// <summary>
    /// Client Token Access to request various token from the OpenIdConnect Authorization Server
    /// </summary>
    // ReSharper disable once UnusedMember.Global
    public class MyClientTokenAccess : IClientTokenAccess
    {
        private readonly ILogger<MyClientTokenAccess> _logger;


        private readonly IAuthenticationSettings _authenticationSettings;
        private readonly DiscoveryCache _discoveryCache;
        private const string OnBehalfOfGrant = "urn:ietf:params:oauth:grant-type:token-exchange";
        private const string TokenType = "urn:ietf:params:oauth:token-type:access_token";
        /// <summary>
        /// We use a static HttpClient that will be shared by all ClientTokenAccess instances.
        /// According to <see ref="https://aspnetmonsters.com/2016/08/2016-08-27-httpclientwrong/"/> it is best practice to reuse the same HttpClient for all calls.
        /// </summary>
        private static readonly HttpClient Client = new HttpClient();

        /// <summary>
        /// Constructor
        /// </summary>
        public MyClientTokenAccess(IAuthenticationSettings authenticationSettings, ILogger<MyClientTokenAccess> logger)
        {
            _authenticationSettings = authenticationSettings;
            _logger = logger;
            // TrustBuilder has Issuer/Discovery urls that don't match the IdentityServer expected Authority convention, disable Issuer validation!
            // Otherwise error like "Issuer name does not match authority" will occur when retrieving the discovery document.

            //var openIdConnectDiscoveryEndpoint = ""; //_authenticationSettings.OpenIdConnectDiscoveryDocument
            _discoveryCache = new DiscoveryCache(_authenticationSettings.OpenIdConnectAuthority,
                new DiscoveryPolicy
                {
                    ValidateIssuerName = false,
                    ValidateEndpoints = false // HACK allow other microsoft domains in here
                });
        }

        /// <inheritDoc/>
        /// <exception cref="TaskCanceledException">The task was canceled.</exception>
        /// <exception cref="OperationCanceledException">The token has had cancellation requested.</exception>
        /// <exception cref="ObjectDisposedException">The associated <see cref="CancellationTokenSource"></see> has been disposed.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="requestScope"/> is <see langword="null"/> or empty</exception>
        public TokenResponse GetClientAccessToken(string requestScope)
        {
            // get the well-known meta-data document (from cache if possible, otherwise via new request)
            return AsyncHelper.RunSync(() => GetClientAccessTokenAsync(requestScope));
        }

        // CancellationToken is mandatory here because we already have a method without CancellationToken and we need to keep that to remain compatible.
        /// <inheritDoc/>
        /// <exception cref="TaskCanceledException">The task was canceled.</exception>
        /// <exception cref="OperationCanceledException">The token has had cancellation requested.</exception>
        /// <exception cref="ObjectDisposedException">The associated <see cref="CancellationTokenSource"></see> has been disposed.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="requestScope"/> is <see langword="null"/> or empty</exception>
        public TokenResponse GetClientAccessToken(string requestScope, CancellationToken cancellationToken)
        {
            // get the well-known meta-data document (from cache if possible, otherwise via new request)
            return AsyncHelper.RunSync(() => GetClientAccessTokenAsync(requestScope, cancellationToken), cancellationToken);
        }

        /// <inheritDoc/>
        /// <exception cref="OperationCanceledException">The token has had cancellation requested.</exception>
        /// <exception cref="ObjectDisposedException">The associated <see cref="CancellationTokenSource"></see> has been disposed.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="requestScope"/> is <see langword="null"/> or empty</exception>
        public async Task<TokenResponse> GetClientAccessTokenAsync(string requestScope, CancellationToken cancellationToken = default)
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
                ClientId = _authenticationSettings.AppClientId,
                ClientSecret = _authenticationSettings.AppClientSecret,
                Scope = requestScope,
                Resource = new List<string>()
                {
                    "api://4945b1cf-b246-42f8-80e5-c4de9cd9eaf1",
                }
            }, cancellationToken).ConfigureAwait(false);
            return tokenResponse;
        }

        /// <inheritDoc/>
        /// <exception cref="TaskCanceledException">The task was canceled.</exception>
        /// <exception cref="ObjectDisposedException">The associated <see cref="CancellationTokenSource"></see> has been disposed.</exception>
        /// <exception cref="UnauthorizedAccessException">The principal on the thread is not a ClaimsPrincipal or it does not have a token or access_token claim.</exception>
        /// <exception cref="OperationCanceledException">The token has had cancellation requested.</exception>
        /// <exception cref="ArgumentNullException">requestScope is <see langword="null"/> or empty</exception>
        public TokenResponse GetOnBehalfOfAccessToken(string requestScope)
        {
            if (!(Thread.CurrentPrincipal is ClaimsPrincipal caller))
            {
                throw new UnauthorizedAccessException("OnBehalfOf requires a ClaimsPrincipal Thread user");
            }

            return AsyncHelper.RunSync(() => GetOnBehalfOfAccessTokenAsync(requestScope, caller));
        }

        // CancellationToken is mandatory here because we already have a method without CancellationToken and we need to keep that to remain compatible.
        /// <inheritDoc/>
        /// <exception cref="TaskCanceledException">The task was canceled.</exception>
        /// <exception cref="ObjectDisposedException">The associated <see cref="CancellationTokenSource"></see> has been disposed.</exception>
        /// <exception cref="UnauthorizedAccessException">The principal on the thread is not a ClaimsPrincipal or it does not have a token or access_token claim.</exception>
        /// <exception cref="OperationCanceledException">The token has had cancellation requested.</exception>
        /// <exception cref="ArgumentNullException">requestScope is <see langword="null"/> or empty</exception>
        public TokenResponse GetOnBehalfOfAccessToken(string requestScope, CancellationToken cancellationToken)
        {
            if (!(Thread.CurrentPrincipal is ClaimsPrincipal caller))
            {
                throw new UnauthorizedAccessException("OnBehalfOf requires a ClaimsPrincipal Thread user");
            }

            return AsyncHelper.RunSync(() => GetOnBehalfOfAccessTokenAsync(requestScope, caller, cancellationToken), cancellationToken);
        }

        /// <inheritDoc/>
        /// <exception cref="OperationCanceledException">The token has had cancellation requested.</exception>
        /// <exception cref="ObjectDisposedException">The associated <see cref="CancellationTokenSource"></see> has been disposed.</exception>
        /// <exception cref="UnauthorizedAccessException">The principal on the thread is not a ClaimsPrincipal or it does not have a token or access_token claim.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="requestScope"/> is <see langword="null"/> or empty</exception>
        public async Task<TokenResponse> GetOnBehalfOfAccessTokenAsync(string requestScope, ClaimsPrincipal caller, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(requestScope))
                throw new ArgumentNullException(nameof(requestScope));
            var tokenIssuerAddress = await GetTokenIssuerAddressAsync(cancellationToken).ConfigureAwait(false);

            // web api bearer token = "token" claim
            // using claims-principal directly = "access_token" claim
            var token = caller.FindFirst("token") ?? caller.FindFirst("access_token");
            if (token == null)
            {
                throw new UnauthorizedAccessException("OnBehalfOf requires a ClaimsPrincipal with access token claim");
            }

            var customParams = new Parameters
            {
                { "subject_token_type", TokenType},
                { "subject_token", token.Value},
                { "scope", requestScope }
            };

            //Check if cancellation is requested
            cancellationToken.ThrowIfCancellationRequested();
            var tokenResponse = await Client.RequestTokenAsync(new TokenRequest
            {
                Address = tokenIssuerAddress,
                ClientCredentialStyle = ClientCredentialStyle.AuthorizationHeader,
                ClientId = _authenticationSettings.AppClientId,
                ClientSecret = _authenticationSettings.AppClientSecret,
                GrantType = OnBehalfOfGrant,
                Parameters = customParams

            }, cancellationToken).ConfigureAwait(false);

            return tokenResponse;
        }

        /// <inheritDoc/>
        /// <exception cref="TaskCanceledException">The task was canceled.</exception>
        /// <exception cref="ObjectDisposedException">The provided <see cref="CancellationToken"></see> has already been disposed.</exception>
        /// <exception cref="OperationCanceledException">The token has had cancellation requested.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="refreshToken"/> is <see langword="null"/> or empty</exception>
        /// <exception cref="UnauthorizedAccessException">Failed to refresh the access token</exception>
        public TokenResponse RenewAccessToken(string refreshToken)
        {
            return AsyncHelper.RunSync(() => RenewAccessTokenAsync(refreshToken));
        }

        // CancellationToken is mandatory here because we already have a method without CancellationToken and we need to keep that to remain compatible.
        /// <inheritDoc/>
        /// <exception cref="TaskCanceledException">The task was canceled.</exception>
        /// <exception cref="ObjectDisposedException">The provided <see cref="CancellationToken"></see> has already been disposed.</exception>
        /// <exception cref="OperationCanceledException">The token has had cancellation requested.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="refreshToken"/> is <see langword="null"/> or empty</exception>
        /// <exception cref="UnauthorizedAccessException">Failed to refresh the access token</exception>
        public TokenResponse RenewAccessToken(string refreshToken, CancellationToken cancellationToken)
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
            //var tokenClient = new TokenClient(issuerTokenAddress,
            //    _authenticationSettings.AppClientId, _authenticationSettings.AppClientSecret);
            //var tokenResponse = await tokenClient.RequestRefreshTokenAsync(refreshToken, cancellationToken: cancellationToken).ConfigureAwait(false);
            var tokenResponse = await Client
                .RequestRefreshTokenAsync(
                    new RefreshTokenRequest
                    {
                        ClientCredentialStyle = ClientCredentialStyle.AuthorizationHeader,
                        Address = tokenIssuerAddress,
                        ClientId = _authenticationSettings.AppClientId,
                        ClientSecret = _authenticationSettings.AppClientSecret,
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
            discoveryResponse.VerifyDiscoveryResponse(_authenticationSettings.OpenIdConnectDiscoveryDocument);
            return discoveryResponse.TokenEndpoint;
        }


        /// <inheritDoc/>
        public TokenResponse GetResourceOwnerClientAccesstoken(string requestScope, string username, string password)
        {
            // get the well-known metadata document (from cache if possible, otherwise via new request)
            var discoveryResponse = _discoveryCache.GetAsync().Result;
            var issuerTokenAddress = discoveryResponse.TokenEndpoint;

            var tokenClient = new HttpClient();

            var tokenResponse = tokenClient.RequestPasswordTokenAsync(
                new PasswordTokenRequest
                {
                    UserName = username,
                    Password = password,
                    ClientId = _authenticationSettings.AppClientId,
                    ClientSecret = _authenticationSettings.AppClientSecret,
                    Scope = requestScope,
                    Address = issuerTokenAddress
                }).GetAwaiter().GetResult();

            _logger?.LogDebug($"tokenResponse - Error: {tokenResponse.Error}");
            _logger?.LogDebug($"tokenResponse - AccessToken: {tokenResponse.AccessToken}");

            if (tokenResponse.AccessToken == null)
                throw new Exception($"Could not retrieve accestoken for {username} and {password}");
            return tokenResponse;
        }
    }
}