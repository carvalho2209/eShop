using Application.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Customers.GetCustomer;

public class GetCustomerQueryHandler : IRequestHandler<GetCustomerQuery, CustomerQueryResponse[]>
{
    private readonly IApplicationDbContext _context;

    public GetCustomerQueryHandler(IApplicationDbContext context) => _context = context;

    public Task<CustomerQueryResponse[]> Handle(GetCustomerQuery request, CancellationToken cancellationToken)
        => _context
            .Customers
            .Select(x => new CustomerQueryResponse(x.Id.Value, x.Name, x.Email))
            .ToArrayAsync(cancellationToken);
}