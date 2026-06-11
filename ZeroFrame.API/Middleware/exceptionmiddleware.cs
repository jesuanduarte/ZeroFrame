using System.Text.Json;
using ZeroFrame.API.Errors;
using ZeroFrame.Application.Exceptions;

namespace ZeroFrame.API.Middleware
{
    public class ExceptionMiddleware
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IHostEnvironment _env;

        public ExceptionMiddleware(
            RequestDelegate next,
            ILogger<ExceptionMiddleware> logger,
            IHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        // O método captura o erro, registra-o e retorna uma resposta JSON apropriada com base no tipo de exceção.
        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);

                if (httpContext.Response.HasStarted)
                {
                    throw;
                }

                var response = CreateResponse(httpContext, ex);

                httpContext.Response.Clear();
                httpContext.Response.StatusCode = response.StatusCode;
                httpContext.Response.ContentType = "application/json";

                await httpContext.Response.WriteAsJsonAsync(response, JsonOptions);
            }
        }

        // é responsável por mapear diferentes tipos de exceções para respostas HTTP apropriadas.
        // Ele verifica o tipo da exceção e retorna ApiException correspondente, contendo o status code
        private ApiException CreateResponse(HttpContext httpContext, Exception exception)
        {
            var details = _env.IsDevelopment() ? exception.StackTrace : null;

            return exception switch
            {
                NotFoundException => new ApiNotFound(exception.Message, details),
                BadRequestException => new ApiBadRequest(exception.Message, details),
                KeyNotFoundException => new ApiNotFound(exception.Message, details),
                BadHttpRequestException => new ApiBadRequest(exception.Message, details),
                ArgumentException => new ApiBadRequest(exception.Message, details),
                InvalidOperationException => new ApiBadRequest(exception.Message, details),
                _ when _env.IsDevelopment() => new ApiException(
                    StatusCodes.Status500InternalServerError,
                    exception.Message,
                    details),
                _ => new ApiException(
                    StatusCodes.Status500InternalServerError,
                    "Ocorreu um erro interno no servidor.",
                    null)
            };
        }
    }

    
    public static class ExceptionMiddlewareExtensions
    {
        public static IApplicationBuilder UseExceptionMiddleware(this IApplicationBuilder app)
        {
            return app.UseMiddleware<ExceptionMiddleware>();
        }
    }
}
