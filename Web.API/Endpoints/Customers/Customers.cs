using Application.Customers.Create;
using Application.Customers.GetCustomer;
using Carter;
using MediatR;

namespace Web.API.Endpoints.Customers;

public sealed class Customers : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("customers", async (
            CreateCustomerRequest request,
            ISender sender) =>
        {
            var command = new CreateCustomerCommand(request.Email, request.Name);

            await sender.Send(command);

            return Results.Ok();
        });

        app.MapGet("customers", async (
            ISender sender) =>
        {
            var query = new GetCustomerQuery();

            var customers = await sender.Send(query);

            return Results.Ok(customers);
        });
    }
}