using System.Net;
using Microsoft.AspNetCore.Mvc;
using MiniCrm.Api.Contracts;

public sealed class CustomersApiTests(MiniCrmApiFactory factory) : IClassFixture<MiniCrmApiFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task Get_unknown_id_returns_404()
    {
        var ct = TestContext.Current.CancellationToken; // xUnit v3
        var response = await _client.GetAsync("/api/customers/00000000-0000-0000-0000-000000000000", ct);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Get_all_returns_200()
    {
        var ct = TestContext.Current.CancellationToken; // xUnit v3
        var response = await _client.GetAsync("/api/customers", ct);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Get_invalid_route_returns_404()
    {
        var ct = TestContext.Current.CancellationToken; // xUnit v3
        var response = await _client.GetAsync("/api/customers/invalid-route", ct);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Post_valid_customer_returns_201_and_location_header()
    {
        var ct = TestContext.Current.CancellationToken; // xUnit v3
        var request = new CreateCustomerRequest
        {
            Name = "John Doe",
            Email = $"{Guid.NewGuid():N}@test.local"
        };

        var response = await _client.PostAsJsonAsync("/api/customers", request, ct);
        
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);

        // Optionally, you can also verify that the Location header points to the correct resource
        var location = response.Headers.Location!.ToString();
        var getResponse = await _client.GetAsync(location, ct);
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
    }

    [Fact]
    public async Task Post_invalid_customer_returns_400()
    {
        var ct = TestContext.Current.CancellationToken; // xUnit v3
        var request = new CreateCustomerRequest
        {
            Name = "", // Invalid: Name is required
            Email = "invalid-email" // Invalid: Not a valid email format
        };

        var response = await _client.PostAsJsonAsync("/api/customers", request, ct);
        
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Post_missing_email_returns_400_and_validation_errors()
    {
        var ct = TestContext.Current.CancellationToken; // xUnit v3
        var request = new CreateCustomerRequest
        {
            Name = "John Doe",
            Email = "" // Invalid: Email is required
        };

        var response = await _client.PostAsJsonAsync("/api/customers", request, ct);
        
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var validationProblemDetails = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>(cancellationToken: ct);
        Assert.NotNull(validationProblemDetails);
        Assert.Contains("email", validationProblemDetails!.Errors.Keys);
    }

    [Fact]
    public async Task Put_valid_update_returns_204()
    {
        var ct = TestContext.Current.CancellationToken; // xUnit v3
        var createRequest = new CreateCustomerRequest
        {
            Name = "Jane Doe",
            Email = $"{Guid.NewGuid():N}@test.local"
        };

        var createResponse = await _client.PostAsJsonAsync("/api/customers", createRequest, ct);
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        var location = createResponse.Headers.Location!.ToString();

        var updateRequest = new UpdateCustomerRequest
        {
            Name = "Jane Smith",
            Email = $"{Guid.NewGuid():N}@test.local"
        };

        var updateResponse = await _client.PutAsJsonAsync(location, updateRequest, ct);
        Assert.Equal(HttpStatusCode.NoContent, updateResponse.StatusCode);
    }

    [Fact]
    public async Task Put_unknown_id_returns_404()
    {
        var ct = TestContext.Current.CancellationToken; // xUnit v3
        var updateRequest = new UpdateCustomerRequest
        {
            Name = "Nonexistent Customer",
            Email = $"{Guid.NewGuid():N}@test.local"
        };

        var response = await _client.PutAsJsonAsync("/api/customers/00000000-0000-0000-0000-000000000000", updateRequest, ct);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_existing_customer_returns_204()
    {
        var ct = TestContext.Current.CancellationToken; // xUnit v3
        var createRequest = new CreateCustomerRequest
        {
            Name = "Mark Doe",
            Email = $"{Guid.NewGuid():N}@test.local"
        };

        var createResponse = await _client.PostAsJsonAsync("/api/customers", createRequest, ct);
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        var location = createResponse.Headers.Location!.ToString();

        var deleteResponse = await _client.DeleteAsync(location, ct);
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
    }

    [Fact]
    public async Task Delete_unknown_id_returns_404()
    {
        var ct = TestContext.Current.CancellationToken; // xUnit v3
        var response = await _client.DeleteAsync("/api/customers/00000000-0000-0000-0000-000000000000", ct);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}