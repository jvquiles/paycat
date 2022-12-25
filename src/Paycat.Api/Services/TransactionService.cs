using MediatR;
using Paycat.Api.Data;
using Paycat.Infrastructure;
using Paycat.Messages;
using Paycat.Messages.Enums;

namespace Paycat.Api.Services;

internal class TransactionService : IRequestHandler<CreatedTransaction, Result<ProcessedTransaction>>
{
    private readonly IMessenger _messenger;

    public TransactionService(IMessenger messenger)
    {
        this._messenger = messenger;
    }

    public async Task<Result<ProcessedTransaction>> Handle(CreatedTransaction request, CancellationToken cancellationToken)
    {
        var transaction = new Transaction()
        {
            Id = request.Id,
            Amount = request.Amount,
            Date = request.Date,
            Status = TransactionStatus.New
        };

        var processedTransaction = await _messenger.SendAsync<CreatedTransaction, Result<ProcessedTransaction>>(request, TimeSpan.FromSeconds(30))
            .ConfigureAwait(false);

        if (processedTransaction is not null && !processedTransaction.IsError)
        {
            transaction.Status = processedTransaction.Value.Status;
            transaction.Date = processedTransaction.Value.Date;
        }

        // TODO: Save transaction

        return processedTransaction;
    }
}