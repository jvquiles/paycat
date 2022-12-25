using Paycat.Messages.Enums;

namespace Paycat.Messages;

public class ProcessedTransaction
{
    public Guid Id { get; set; }
    public TransactionStatus Status { get; set; }
    public DateTimeOffset Date { get; set; }
}