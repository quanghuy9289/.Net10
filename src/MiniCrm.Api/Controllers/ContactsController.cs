using Microsoft.AspNetCore.Mvc;
using MiniCrm.Api.Contracts;
using MiniCrm.Domain.Abstractions;

[ApiController]
[Route("api/customers/{customerId:guid}/contacts")]
public sealed class ContactsController(ICustomerRepository repository) : ControllerBase
{
    // GET api/customers/5/contacts
    [HttpGet]
    public async Task<ActionResult<List<ContactResponse>>> GetAll(Guid customerId, CancellationToken ct)
    {
        var customer = await repository.GetByIdAsync(customerId, ct);
        if (customer is null)
        {
            return NotFound();
        }

        return customer.Contacts.Select(ContactResponse.From).ToList();
    }

    // GET api/customers/5/contacts/12
    [HttpGet("{contactId:guid}")]
    public async Task<ActionResult<ContactResponse>> GetById(Guid customerId, Guid contactId, CancellationToken ct)
    {
        var customer = await repository.GetByIdAsync(customerId, ct);
        if (customer is null)
        {
            return NotFound();
        }

        var contact = customer.Contacts.FirstOrDefault(c => c.Id == contactId);
        if (contact is null)
        {
            return NotFound();
        }

        return ContactResponse.From(contact);
    }
}