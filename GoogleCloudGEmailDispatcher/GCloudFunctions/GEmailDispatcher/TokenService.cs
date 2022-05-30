using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Requests;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Gmail.v1;
using Google.Apis.Util;
using Google.Cloud.Datastore.V1;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace EmailDispatcher
{
    public static class TokenService
    {
        private static readonly string[] Scopes = {
            GmailService.Scope.GmailSend,
            GmailService.Scope.GmailCompose,
            GmailService.Scope.GmailReadonly
        };

        public static async Task<UserCredential> GetUserCredentialAsync()
        {
            var authDb = DatastoreDb.Create(EnvironmentVariables.GCLOUD_FUNCTION_PROJECT_ID, "EmailDispatcher");
            var keyFactory = authDb.CreateKeyFactory("Auth");
            var tokenKey = keyFactory.CreateKey("OAuthToken");

            var result = await authDb.LookupAsync(tokenKey);
            if (result == null) return null;

            var token = JsonSerializer.Deserialize<TokenResponse>(result["token"].StringValue);

            var credential = RebuildUserCredential(token);

            await RefreshTokenIfExpiredAsync(credential, tokenKey, authDb);

            return credential;
        }

        private static UserCredential RebuildUserCredential(TokenResponse token) => new UserCredential(GetAuthorizationFlow(), "me", token);

        private static async Task RefreshTokenIfExpiredAsync(UserCredential credential, Key tokenKey, DatastoreDb authDb)
        {
            if (credential.Token.IsExpired(SystemClock.Default))
            {
                await credential.RefreshTokenAsync(CancellationToken.None);
                await SaveTokenInDatastoreAsync(credential, tokenKey, authDb);
            }
        }

        private static async Task SaveTokenInDatastoreAsync(UserCredential credential, Key tokenKey, DatastoreDb authDb)
        {
            var authToken = new Entity
            {
                Key = tokenKey,
                ["token"] = JsonSerializer.Serialize(credential.Token)
            };

            using (var transaction = await authDb.BeginTransactionAsync())
            {
                transaction.Upsert(authToken);
                transaction.Commit();
            }
        }

        /// <summary>
        /// Requests a refresh token that the client (the cloud function GEmailDispatcher)
        /// will use to send emails without the user having to input their consent every time to do that action.
        /// </summary>
        //From https://stackoverflow.com/a/27686104
        private class OfflineAccessGoogleAuthorizationCodeFlow : GoogleAuthorizationCodeFlow
        {
            public OfflineAccessGoogleAuthorizationCodeFlow(Initializer initializer) : base(initializer)
            {
            }

            public override AuthorizationCodeRequestUrl CreateAuthorizationCodeRequest(string redirectUri)
            {
                return new GoogleAuthorizationCodeRequestUrl(new Uri(AuthorizationServerUrl))
                {
                    ClientId = ClientSecrets.ClientId,
                    Scope = string.Join(" ", Scopes),
                    RedirectUri = redirectUri,
                    AccessType = "offline",
                    Prompt = "consent"
                };
            }
        }

        private static OfflineAccessGoogleAuthorizationCodeFlow GetAuthorizationFlow()
        {
            return new OfflineAccessGoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
            {
                ClientSecrets = new ClientSecrets
                {
                    ClientId = EnvironmentVariables.GMAIL_CLIENT_ID,
                    ClientSecret = EnvironmentVariables.GMAIL_CLIENT_SECRET
                },
                Scopes = Scopes,
                ProjectId = EnvironmentVariables.GCLOUD_FUNCTION_PROJECT_ID,
                Prompt = "consent"
            });
        }
    }
}