using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Gmail.v1;
using Google.Cloud.Functions.Framework;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace OAuthInit
{
    public class Function : IHttpFunction
    {
        private readonly ILogger _logger;

        private static readonly string[] Scopes = {
            GmailService.Scope.GmailSend,
            GmailService.Scope.GmailCompose,
            GmailService.Scope.GmailReadonly
        };

        public Function(ILogger<Function> logger)
        {
            _logger = logger;
        }

        public async Task HandleAsync(HttpContext context)
        {
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

                var redirectUri = flow.CreateAuthorizationCodeRequest(EnvironmentVariables.GCLOUD_FUNCTION_OAUTHCALLBACK_URL).Build();
                context.Response.Redirect(redirectUri.ToString());
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                await context.Response.WriteAsync("The Google Cloud Function OAuthInit failed to redirect to Google's Authorization Server due to an exception!");
            }

            await context.Response.WriteAsync("OAuthInit finished.");
        }
    }
}