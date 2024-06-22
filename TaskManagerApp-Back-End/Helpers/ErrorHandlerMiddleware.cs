using System;
using System.Net;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using TaskManagerApp.Services.Exceptions;

namespace TaskManagerApp.Helpers
{
	public class ErrorHandlerMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly ILogger<ErrorHandlerMiddleware> _logger;

		public ErrorHandlerMiddleware(RequestDelegate next, ILogger<ErrorHandlerMiddleware> logger)
		{
			_next = next;
			_logger = logger;
		}

		public async Task Invoke(HttpContext context)
		{
			try
			{
				_logger.LogInformation("Invoking next middleware");

				// Log authentication and role information
				if (context.User.Identity!.IsAuthenticated)
				{
					_logger.LogInformation("User is authenticated.");
				}
				else
				{
					_logger.LogInformation("User is not authenticated.");
				}
				var roles = context.User.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToList();
				if (roles.Any())
				{
					_logger.LogInformation($"User roles: {string.Join(", ", roles)}");
				}
				else
				{
					_logger.LogInformation("User has no roles.");
				}

				if (context.User.Identity.IsAuthenticated && context.User.IsInRole("User"))
				{
					_logger.LogInformation("User is in the 'User' role.");
				}
				else
				{
					_logger.LogInformation("User is not in the 'User' role.");
				}

				await _next(context);
			}
			catch (Exception exception)
			{
				_logger.LogError(exception, "An unhandled exception has occurred");
				var response = context.Response;
				response.ContentType = "application/json";

				response.StatusCode = exception switch
				{
					InvalidRegistrationException or
					UserAlreadyExistsException => (int)HttpStatusCode.BadRequest,
					UnauthorizedAccessException => (int)HttpStatusCode.Unauthorized,
					ForbiddenException => (int)HttpStatusCode.Forbidden,
					UserNotFoundException => (int)HttpStatusCode.NotFound,
					_ => (int)HttpStatusCode.InternalServerError
				};

				var result = JsonSerializer.Serialize(new { message = exception?.Message });
				await response.WriteAsync(result);
			}
		}
	}
}
