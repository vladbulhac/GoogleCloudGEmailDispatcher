using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Requests;
using System;

namespace OAuthCallback
{
    /// <summary>
    /// Requests a refresh token that the client (the cloud function GEmailDispatcher)
    /// will use to send emails without the user having to input their consent every time to do that action.
    /// </summary>
    //From https://stackoverflow.com/a/27686104
    public class OfflineAccessGoogleAuthorizationCodeFlow : GoogleAuthorizationCodeFlow
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
}