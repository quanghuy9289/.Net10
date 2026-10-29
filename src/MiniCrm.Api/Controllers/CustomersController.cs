using Microsoft.AspNetCore.Mvc;
using MiniCrm.Api.Contracts;
using MiniCrm.Domain.Abstractions;
using MiniCrm.Domain.Entities;

namespace MiniCrm.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class CustomersController(ICustomerRepository repository) : ControllerBase
{
    // GET api/customers → 200
    [HttpGet]
    public async Task<ActionResult<List<CustomerResponse>>> GetAll(CancellationToken ct)
    {
        var customers = await repository.GetAllAsync(ct);
        return customers.Select(CustomerResponse.From).ToList();
    }

    // GET api/customers/5 → 200 | 404
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CustomerResponse>> GetById(Guid id, CancellationToken ct)
    {
        var customer = await repository.GetByIdAsync(id, ct);
        if (customer is null)
        {
            return NotFound();
        }

        return CustomerResponse.From(customer); // implicit -> 200 OK
    }

    // POST api/customers → 201 + Location | 400 (tự động)
    [HttpPost]
    public async Task<ActionResult<CustomerResponse>> Create([FromBody] CreateCustomerRequest request, CancellationToken ct)
    {
        var customer = new Customer
        {
            Name = request.Name,
            Email = request.Email,
        };

        await repository.AddAsync(customer, ct);

        return CreatedAtAction(nameof(GetById), new { id = customer.Id }, CustomerResponse.From(customer));
    }

    // PUT api/customers/5 → 204 | 404 | 400
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateCustomerRequest request, CancellationToken ct)
    {
        var existing = await repository.GetByIdAsync(id, ct);
        if (existing is null)
            return NotFound();

        // Nếu Customer là record: dùng `with`. Nếu là class có setter: gán trực tiếp.
        var updated = existing with { Name = request.Name, Email = request.Email, IsActive = request.IsActive };
        await repository.UpdateAsync(updated, ct);

        return NoContent();
    }

    // DELETE api/customers/5 → 204 | 404
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
        => await repository.DeleteAsync(id, ct) ? NoContent() : NotFound();
}