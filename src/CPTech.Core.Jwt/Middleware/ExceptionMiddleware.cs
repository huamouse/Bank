using System;
using System.Text.Json;
using System.Threading.Tasks;
using CPTech.Core;
using CPTech.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace CPTech.Middleware
{
    #region ExceptionMiddleware
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ILogger<ExceptionMiddleware> logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            this.next = next;
            this.logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await next(context);

                await CheckRspCodeAsync(context);
            }
            catch (NetException ex)
            {
                if (!context.Response.HasStarted) context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(SerializerErrorMessage(ex.Code, ex.Message));
            }
            catch (Exception ex)
            {
                if (!context.Response.HasStarted) context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(SerializerErrorMessage(500, ex.InnerException?.Message ?? ex.Message));
            }
        }

        private async Task CheckRspCodeAsync(HttpContext context)
        {
            switch (context.Response.StatusCode)
            {
                case 200:
                case 204:
                case 404:
                    return;
                case 401:
                    if (!context.Response.HasStarted) context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync(SerializerErrorMessage(401, "token已过期，请重新登录！"));
                    break;
                case 415:
                    logger.LogError("415:不支持的媒体类型！");
                    break;
                default:
                    logger.LogInformation($"Http response code: {context.Response.StatusCode}");
                    break;
            }
        }

        private string SerializerErrorMessage(int code, string message)
        {
            return JsonSerializer.Serialize(ResultModel.Error(code, message), Constants.JsonSerializerOption);
        }
    }
    #endregion

    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseException(this IApplicationBuilder builder) => builder.UseMiddleware<ExceptionMiddleware>();
    }
}
