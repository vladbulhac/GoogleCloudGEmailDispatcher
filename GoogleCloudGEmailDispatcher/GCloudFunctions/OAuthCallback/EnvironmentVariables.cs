using System;

namespace OAuthCallback
{
    public static class EnvironmentVariables
    {
        public static string GCLOUD_FUNCTION_REGION { get; }
        public static string GCLOUD_FUNCTION_PROJECT_ID { get; }
        public static string NAMEOF_OAUTHCALLBACK_GCLOUD_FUNCTION { get; }

        public static string GCLOUD_FUNCTION_OAUTHCALLBACK_URL { get; }

        public static string GCLOUD_DATASTORE_TOKENKEY { get; }

        public static string GMAIL_CLIENT_ID { get; }
        public static string GMAIL_CLIENT_SECRET { get; }

        static EnvironmentVariables()
        {
            GCLOUD_FUNCTION_PROJECT_ID = Environment.GetEnvironmentVariable(nameof(GCLOUD_FUNCTION_PROJECT_ID)) ?? throw new ArgumentNullException(nameof(GCLOUD_FUNCTION_PROJECT_ID));
            GCLOUD_FUNCTION_REGION = Environment.GetEnvironmentVariable(nameof(GCLOUD_FUNCTION_REGION)) ?? throw new ArgumentNullException(nameof(GCLOUD_FUNCTION_REGION));
            GCLOUD_DATASTORE_TOKENKEY = Environment.GetEnvironmentVariable(nameof(GCLOUD_DATASTORE_TOKENKEY)) ?? throw new ArgumentNullException(nameof(GCLOUD_DATASTORE_TOKENKEY));
            NAMEOF_OAUTHCALLBACK_GCLOUD_FUNCTION = Environment.GetEnvironmentVariable(nameof(NAMEOF_OAUTHCALLBACK_GCLOUD_FUNCTION)) ?? throw new ArgumentNullException(nameof(NAMEOF_OAUTHCALLBACK_GCLOUD_FUNCTION));

            GCLOUD_FUNCTION_OAUTHCALLBACK_URL = $"https://{GCLOUD_FUNCTION_REGION}-{GCLOUD_FUNCTION_PROJECT_ID}.cloudfunctions.net/{NAMEOF_OAUTHCALLBACK_GCLOUD_FUNCTION}";

            GMAIL_CLIENT_ID = Environment.GetEnvironmentVariable(nameof(GMAIL_CLIENT_ID)) ?? throw new ArgumentNullException(nameof(GMAIL_CLIENT_ID));
            GMAIL_CLIENT_SECRET = Environment.GetEnvironmentVariable(nameof(GMAIL_CLIENT_SECRET)) ?? throw new ArgumentNullException(nameof(GMAIL_CLIENT_SECRET));
        }
    }
}