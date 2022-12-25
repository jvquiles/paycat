using MediatR;

namespace Paycat.Messages;

public class CreatedTransaction : IRequest<Result<ProcessedTransaction>>
{
    public Guid Id { get; set; }
    public DateTimeOffset Date { get; set; }
    public decimal Amount { get; set; }
}