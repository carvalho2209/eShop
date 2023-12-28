using Application.Behaviors.Messaging;
using MediatR;

namespace Application.Customers.Create;

public record CreateCustomerCommand(string Email, string Name) : ICommand;

public record CreateCustomerRequest(string Email, string Name);