namespace Paycat.Api.Models;

public class CreateTransaction
{
    public Guid Id { get; set; }
    public decimal Amount { get; set; }
}