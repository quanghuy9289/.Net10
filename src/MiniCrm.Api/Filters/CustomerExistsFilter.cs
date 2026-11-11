using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MiniCrm.Domain.Abstractions;
using MiniCrm.Domain.Exceptions;

public sealed class CustomerExistsFilter(ICustomerRepository repository) : IAsyncActionFilter
{
    private readonly ICustomerRepository _repository = repository;

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (context.ActionArguments.TryGetValue("id", out var idObj) && idObj is Guid id)
        {
            var customer = await _repository.GetByIdAsync(id, context.HttpContext.RequestAborted);
            if (customer is null)
            {
                throw new CustomerNotFoundException(id);
            }
        }

        await next();
    }
}