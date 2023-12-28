using MediatR;

namespace Application.Customers.GetCustomer;

public record GetCustomerQuery : IRequest<CustomerQueryResponse[]>;