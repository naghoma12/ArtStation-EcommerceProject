using ArtStation.Resources;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Localization;
using System.Net;
using System.Text.Json;

namespace ArtStation.Middlewares
{
    public class ValidationAuthorization : IAuthorizationMiddlewareResultHandler
    {
        private readonly AuthorizationMiddlewareResultHandler authorizationMiddleware = new AuthorizationMiddlewareResultHandler();
        public async Task HandleAsync(RequestDelegate next, HttpContext context, AuthorizationPolicy policy, PolicyAuthorizationResult authorizeResult)
        {
            if (authorizeResult.Challenged)
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                await context.Response.WriteAsJsonAsync(new { Message = ControllerMessages.Unauthorized });
                return;
            }
            if (authorizeResult.Forbidden)
            {
                context.Response.StatusCode = (int)HttpStatusCode.OK;
                await context.Response.WriteAsJsonAsync(new { Message = ControllerMessages.Unauthorized });
                return;
            }
            await authorizationMiddleware.HandleAsync(next, context, policy, authorizeResult);
        }
    }
}
