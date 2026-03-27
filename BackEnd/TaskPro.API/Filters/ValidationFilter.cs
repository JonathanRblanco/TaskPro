using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace TaskPro.API.Filters
{
    public class ValidationFilter(IServiceProvider serviceProvider) : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(
            ActionExecutingContext context,
            ActionExecutionDelegate next)
        {
            var dto = context.ActionArguments.Values
                .FirstOrDefault(v => v is not null && !v.GetType().IsPrimitive
                    && v.GetType() != typeof(string)
                    && v.GetType() != typeof(Guid));

            if (dto is not null)
            {
                var validatorType = typeof(IValidator<>).MakeGenericType(dto.GetType());
                var validator = serviceProvider.GetService(validatorType) as IValidator;

                if (validator is not null)
                {
                    var validationContext = new ValidationContext<object>(dto);
                    var result = await validator.ValidateAsync(validationContext);

                    if (!result.IsValid)
                    {
                        var errors = result.Errors
                            .GroupBy(e => e.PropertyName)
                            .ToDictionary(
                                g => g.Key,
                                g => g.Select(e => e.ErrorMessage).ToArray());

                        context.Result = new BadRequestObjectResult(new { errors });
                        return;
                    }
                }
            }

            await next();
        }
    }
}
