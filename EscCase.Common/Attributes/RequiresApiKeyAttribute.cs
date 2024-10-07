using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;

namespace EscCase.Common.Attributes
{
    [AttributeUsage(validOn: AttributeTargets.Class | AttributeTargets.Method)]
    public class RequiresApiKeyAttribute : Attribute, IAsyncActionFilter
    {
        private const string ApiKeySettingsName = "apikey";
        private const string ApiKeyHeaderName = "X-Api-Key";

        private readonly ContentResult InvalidApiKeyResult = new()
        {
            StatusCode = 401,
            Content = "Invalid API Key"
        };

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var hasAllowAnonymous = context.ActionDescriptor.EndpointMetadata
                                    .Any(em => em.GetType() == typeof(AllowAnonymousAttribute));

            if (hasAllowAnonymous)
            {
                await next();
                return;
            }

            if (!context.HttpContext.Request.Headers.TryGetValue(ApiKeyHeaderName, out var extractedApiKey))
            {
                if (context.HttpContext.Request.Query.TryGetValue(ApiKeySettingsName, out var apiKeyFromQueryString))
                {
                    extractedApiKey = apiKeyFromQueryString;
                }
            }

            if (string.IsNullOrWhiteSpace(extractedApiKey))
            {
                context.Result = InvalidApiKeyResult;
                return;
            }

            var apiKey = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("API-KEY").Value ?? string.Empty;

            if (!apiKey.Equals(extractedApiKey))
            {
                context.Result = InvalidApiKeyResult;
                return;
            }

            await next();
        }
    }
}
