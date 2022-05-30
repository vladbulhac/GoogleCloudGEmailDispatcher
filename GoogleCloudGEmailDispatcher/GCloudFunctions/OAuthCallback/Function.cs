using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Gmail.v1;
using Google.Cloud.Datastore.V1;
using Google.Cloud.Functions.Framework;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace OAuthCallback
{
    public class Function : IHttpFunction
    {
        private static readonly string[] Scopes = {
            GmailService.Scope.GmailSend,
            GmailService.Scope.GmailCompose,
            GmailService.Scope.GmailReadonly
        };

        private readonly DatastoreDb authDb;
        private readonly Key tokenKey;
        private readonly ILogger _logger;

        public Function(ILogger<Function> logger)
        {
            _logger = logger;

            authDb = DatastoreDb.Create(EnvironmentVariables.GCLOUD_FUNCTION_PROJECT_ID, "EmailDispatcher");
            var keyFactory = authDb.CreateKeyFactory("Auth");
            tokenKey = keyFactory.CreateKey(EnvironmentVariables.GCLOUD_DATASTORE_TOKENKEY);
        }

        public async Task HandleAsync(HttpContext context)
        {
            var code = context.Request.Query["code"].ToString();
            if (string.IsNullOrEmpty(code))
            {
                await context.Response.WriteAsync("The Google Cloud Function OAuthCallback didn't receive an authorization code!");
                return;
            }

            try
            {
                var flow = new OfflineAccessGoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
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

                var token = await flow.ExchangeCodeForTokenAsync("me", code, EnvironmentVariables.GCLOUD_FUNCTION_OAUTHCALLBACK_URL, CancellationToken.None);

                var credential = new UserCredential(flow, "me", token);
                await SaveTokenInDatastoreAsync(credential);

                await context.Response.WriteAsync("Saved the access and refresh tokens in Google Cloud Datastore!");
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                await context.Response.WriteAsync("The Google Cloud Function OAuthCallback couldn't save the access and refresh tokens in Google Cloud Datastore!");
            }
        }

        private async Task SaveTokenInDatastoreAsync(UserCredential credential)
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
    }
}