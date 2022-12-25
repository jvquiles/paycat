using MediatR;
using Paycat.Messages;
using Paycat.Messages.Enums;

namespace Paycat.Processor.Services;

internal class TransactionHandler : IRequestHandler<CreatedTransaction, Result<ProcessedTransaction>>
{
    public Task<Result<ProcessedTransaction>> Handle(CreatedTransaction request, CancellationToken cancellationToken)
    {
        var processedTransaction = new ProcessedTransaction
        {
            Id = request.Id,
            Date = DateTimeOffset.Now,
            Status = TransactionStatus.Paid
        };

        return Task.FromResult(Result<ProcessedTransaction>.Ok(processedTransaction));
    }
}