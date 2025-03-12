using System;
using System.Net;
using System.Security.Authentication;
using LIMSApi.Helpers;
using LIMSApi.Models;
using LIMSApi.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace LIMSApi.Middleware
{
    public class GeneralizedExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GeneralizedExceptionHandlingMiddleware> _logger;

        public GeneralizedExceptionHandlingMiddleware(RequestDelegate next, ILogger<GeneralizedExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext httpContext, LoggedInUserProvider userProvider, ISiteActivityService siteActivityService, ISiteErrorService siteErrorService)
        {
            // initialize the current user
            try
            {
                userProvider.Initialize();
                await _next(httpContext);

                if (httpContext.Items["ActivityLog"] is List<SiteActivity> siteActivities)
                {
                    await siteActivityService.CreateMultipleSiteActivities(siteActivities);
                }
            }
            catch (Exception ex)
            {
                // Capture controller and action names dynamically
                var controllerName = httpContext.Request.RouteValues["controller"]?.ToString();
                var actionName = httpContext.Request.RouteValues["action"]?.ToString();
                var userIp = httpContext.Connection.RemoteIpAddress?.ToString();
                var userAgent = httpContext.Request.Headers["User-Agent"].ToString();
                var userId = LoggedInUserProvider.CurrentUser?.UserId;

                string message = $"Error in controller: {controllerName}, action: {actionName}";

                var errorResponse = new ExceptionResponse
                {
                    Message = "An unexpected error occurred.",
                    Controller = controllerName,
                    Action = actionName,
                    ExceptionType = ex.GetType().Name,
                    Timestamp = DateTime.UtcNow
                };

                if (ex is ArgumentNullException || ex is InvalidOperationException)
                {
                    httpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    errorResponse.Message = ex.Message;

                }
                else if (ex is UnauthorizedAccessException)
                {
                    httpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    errorResponse.Message = $"You are not authorized to access this resource ({actionName}).";
                }
                else if (ex is KeyNotFoundException)
                {
                    httpContext.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    errorResponse.Message = $"The requested resource was not found ({actionName}).";
                }
                else if (ex is InvalidCredentialException)
                {
                    message = $"Error in controller: {controllerName}, action: {actionName} Invalid Credential.";
                    httpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    errorResponse.Message = ex.Message;
                }
                else if (ex is SqlException)
                {
                    message = $"Error in controller: {controllerName}, action: {actionName} from: database";
                    httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    errorResponse.Message = "An error occurred while processing your request.";
                }
                else
                {
                    httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                }

                var siteError = new SiteError
                {
                    ErrorCode = controllerName,
                    ExceptionMessage = ex.Message,
                    ExceptionStackTrace = ex.StackTrace,
                    Source = ex.Source,
                    Ipaddress = userIp,
                    Browser = userAgent,
                    Description = ex.InnerException != null ? ex.InnerException.InnerException?.HResult.ToString() + " : " + ex.InnerException.InnerException?.Message : $"Error in {controllerName}/{actionName}: {ex.Message}",
                    WebUrl = $"{controllerName}/{actionName}",
                    ModifiedBy = "" + userId,
                    ModifiedOn = DateTime.UtcNow,
                    CompanyCode = "LIMS"
                };

                _logger.LogError(ex, message);
                await httpContext.Response.WriteAsJsonAsync(errorResponse);
            }
            finally
            {
                // Clear the User Data
                LoggedInUserProvider.ClearUser();
            }
        }


    }

    public class ExceptionResponse
    {
        public string? Message { get; set; }
        public string? Controller { get; set; }
        public string? Action { get; set; }
        public string? ExceptionType { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
