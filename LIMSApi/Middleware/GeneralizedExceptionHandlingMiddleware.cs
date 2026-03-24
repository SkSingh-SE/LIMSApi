using System;
using System.Net;
using System.Security.Authentication;
using LIMSApi.Helpers;
using LIMSApi.Models;
using LIMSApi.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace LIMSApi.Middleware
{
    public class GeneralizedExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GeneralizedExceptionHandlingMiddleware> _logger;
        private readonly bool _isDevelopment;

        public GeneralizedExceptionHandlingMiddleware(RequestDelegate next, ILogger<GeneralizedExceptionHandlingMiddleware> logger, IWebHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _isDevelopment = env.IsDevelopment();
        }

        public async Task Invoke(
     HttpContext context,
     LoggedInUserProvider userProvider,
     ISiteActivityService siteActivityService,
     ISiteErrorService siteErrorService)
        {
            try
            {
                userProvider.Initialize();
                await _next(context);

                if (context.Items["ActivityLog"] is List<SiteActivity> activities)
                {
                    await siteActivityService.CreateMultipleSiteActivities(activities);
                }
            }
            catch (Exception ex)
            {
                var controller = context.Request.RouteValues["controller"]?.ToString();
                var action = context.Request.RouteValues["action"]?.ToString();

                var user = LoggedInUserProvider.CurrentUser;

                // Show user-friendly validation messages in production too
                var isBusinessException = ex is InvalidOperationException or ArgumentException or ArgumentNullException;
                var isDbException = ex is SqlException or DbUpdateException;
                var userMessage = isBusinessException
                    ? ex.Message
                    : isDbException
                        ? "A database error occurred. Please check your data and try again."
                        : _isDevelopment ? ex.Message : "Something went wrong. Please try again or contact your administrator.";

                var errorResponse = new ExceptionResponse
                {
                    Message = userMessage,
                    Controller = controller,
                    Action = action,
                    ExceptionType = _isDevelopment ? ex.GetType().Name : null,
                    Timestamp = DateTime.UtcNow
                };

                context.Response.StatusCode = ex switch
                {
                    ArgumentNullException or ArgumentException or InvalidOperationException => StatusCodes.Status400BadRequest,
                    UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
                    KeyNotFoundException => StatusCodes.Status404NotFound,
                    InvalidCredentialException => StatusCodes.Status400BadRequest,
                    SqlException or DbUpdateException => StatusCodes.Status400BadRequest,
                    _ => StatusCodes.Status500InternalServerError
                };

                var siteError = new SiteError
                {
                    ErrorCode = controller,
                    ExceptionMessage = ex.Message,
                    ExceptionStackTrace = ex.StackTrace,
                    Source = ex.Source,
                    Ipaddress = context.Connection.RemoteIpAddress?.ToString(),
                    Browser = context.Request.Headers["User-Agent"].ToString(),
                    Description = $"Error in {controller}/{action}: {ex.Message}",
                    WebUrl = $"{controller}/{action}",
                    ModifiedBy = user?.UserId.ToString() ?? "Anonymous",
                    ModifiedOn = DateTime.UtcNow,
                    CompanyCode = user?.CompanyCode ?? "LIMS"
                };

                await siteErrorService.CreateSiteError(siteError);
                _logger.LogError(ex, "Unhandled exception");

                await context.Response.WriteAsJsonAsync(errorResponse);
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
