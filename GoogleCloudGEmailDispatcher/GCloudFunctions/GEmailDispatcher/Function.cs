using Google.Cloud.Functions.Framework;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace EmailDispatcher
{
    public class Function : IHttpFunction
    {
        private readonly ILogger _logger;

        public Function(ILogger<Function> logger)
        {
            _logger = logger;
        }

        public async Task HandleAsync(HttpContext context)
        {
            try
            {
                var credential = await TokenService.GetUserCredentialAsync();
                if (credential == null)
                {
                    context.Response.Redirect(EnvironmentVariables.GCLOUD_FUNCTION_OAUTHINIT_URL);
                    return;
                }

                var emailService = new EmailService(credential);

                var email = await DeserializeEmailAsync(context.Request.Body);
                if (email == null)
                {
                    await context.Response.WriteAsync("Could not get the email body!");
                    return;
                }

                var sent = await emailService.SendEmailAsync(email);
                if (!sent)
                {
                    await context.Response.WriteAsync("Failed to send the email!");
                    return;
                }

                await context.Response.WriteAsync("Email was sent successfully!");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "The exception occurred while sending an email!");
                await context.Response.WriteAsync("Failed to send the email due to an exception!");
            }
        }

        private async Task<Email> DeserializeEmailAsync(Stream requestBody)
        {
            using TextReader reader = new StreamReader(requestBody);
            string text = await reader.ReadToEndAsync();
            if (text.Length > 0)
            {
                try
                {
                    return JsonSerializer.Deserialize<Email>(text);
                }
                catch (JsonException parseException)
                {
                    _logger.LogError(parseException, "Error parsing JSON request");
                }
            }

            return null;
        }
    }
}