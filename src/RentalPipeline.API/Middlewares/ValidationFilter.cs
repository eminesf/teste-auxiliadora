using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace RentalPipeline.API.Middlewares;

public class ValidationFilter<T>(IValidator<T> validator) : IAsyncActionFilter
    where T : class
{
  public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
  {
    var argument = context.ActionArguments.Values
        .OfType<T>()
        .FirstOrDefault();

    if (argument is null)
    {
      await next();
      return;
    }

    var result = await validator.ValidateAsync(argument);

    if (!result.IsValid)
    {
      var errors = result.Errors
          .GroupBy(e => e.PropertyName)
          .ToDictionary(
              g => g.Key,
              g => g.Select(e => e.ErrorMessage).ToArray()
          );

      context.Result = new BadRequestObjectResult(new
      {
        status = 400,
        errors
      });

      return;
    }

    await next();
  }
}