using Paycat.Messages.Enums;

namespace Paycat.Api.Data;

public class Transaction
{
    public Guid Id { get; set; }
    public TransactionStatus Status { get; set; }
    public DateTimeOffset Date { get; set; }
    public decimal Amount { get; set; }
}