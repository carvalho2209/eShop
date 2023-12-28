namespace Application.Customers.GetCustomer;

public record CustomerQueryResponse(Guid Id, string Name, string Email);