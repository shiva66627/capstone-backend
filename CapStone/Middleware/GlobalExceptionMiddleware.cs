using CapStone.DTO.Common;
using CapStone.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace CapStone.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred: {Message}", ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var statusCode = HttpStatusCode.InternalServerError;
            var errorType = "UnexpectedInternalServerError";
            var message = "An unexpected error occurred.";

            switch (exception)
            {
                case ResourceNotFoundException:
                    statusCode = HttpStatusCode.NotFound;
                    errorType = "ResourceNotFound";
                    message = exception.Message;
                    break;

                case DuplicateEmailException:
                    statusCode = HttpStatusCode.Conflict;
                    errorType = "DuplicateEmail";
                    message = exception.Message;
                    break;

                case DuplicateProductNameException:
                    statusCode = HttpStatusCode.Conflict;
                    errorType = "DuplicateProductName";
                    message = exception.Message;
                    break;

                case DuplicateTransactionException:
                    statusCode = HttpStatusCode.Conflict;
                    errorType = "DuplicateTransactionReference";
                    message = exception.Message;
                    break;

                case ForbiddenAccessException:
                    statusCode = HttpStatusCode.Forbidden;
                    errorType = "ForbiddenRoleAccess"; // Or AttemptToAccessAnotherCustomersData
                    message = exception.Message;
                    break;

                case InvalidCredentialsException:
                    statusCode = HttpStatusCode.Unauthorized;
                    errorType = "InvalidLoginCredentials";
                    message = exception.Message;
                    break;

                case InactiveUserException:
                    statusCode = HttpStatusCode.Unauthorized;
                    errorType = "InactiveUserLoginAttempt";
                    message = exception.Message;
                    break;

                case ClaimAmountExceedsCoverageException:
                    statusCode = HttpStatusCode.BadRequest;
                    errorType = "ClaimAmountExceedingCoverage";
                    message = exception.Message;
                    break;

                case ClaimModificationException:
                    statusCode = HttpStatusCode.BadRequest;
                    errorType = "AttemptToModifyApprovedOrRejectedClaim";
                    message = exception.Message;
                    break;

                case BusinessRuleViolationException:
                    statusCode = HttpStatusCode.BadRequest;
                    errorType = "BusinessRuleViolation";
                    message = exception.Message;
                    break;

                case KeyNotFoundException:
                    statusCode = HttpStatusCode.NotFound;
                    errorType = "ResourceNotFound";
                    message = exception.Message;
                    break;

                case ArgumentException:
                case InvalidOperationException:
                    statusCode = HttpStatusCode.BadRequest;
                    errorType = "ValidationFailure";
                    message = exception.Message;
                    break;
            }

            context.Response.StatusCode = (int)statusCode;

            var response = new ErrorResponseDTO
            {
                Timestamp = DateTime.UtcNow,
                StatusCode = (int)statusCode,
                ErrorType = errorType,
                Message = message,
                Path = context.Request.Path
            };

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var json = JsonSerializer.Serialize(response, options);
            return context.Response.WriteAsync(json);
        }
    }
}
