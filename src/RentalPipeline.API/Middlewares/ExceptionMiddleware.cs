using System.Net;
using System.Text.Json;
using RentalPipeline.Domain.Exceptions;

namespace RentalPipeline.API.Middlewares;

public class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exceção não tratada: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, message) = exception switch
        {
            // 404 — recurso não encontrado
            KeyNotFoundException => (HttpStatusCode.NotFound, exception.Message),

            // 409 — imóvel não disponível (conflito de estado)
            PropertyNotAvailableException => (HttpStatusCode.Conflict, exception.Message),

            // 422 — transição de estado inválida
            InvalidTransitionException => (HttpStatusCode.UnprocessableEntity, exception.Message),

            // 400 — payload inválido (enum desconhecido, etc)
            ArgumentException => (HttpStatusCode.BadRequest, exception.Message),

            // 500 — qualquer outro erro inesperado
            _ => (HttpStatusCode.InternalServerError, "Ocorreu um erro interno. Tente novamente.")
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var response = JsonSerializer.Serialize(new
        {
            status = (int)statusCode,
            error = message
        });

        await context.Response.WriteAsync(response);
    }
}