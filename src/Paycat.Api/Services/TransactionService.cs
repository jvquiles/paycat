using MediatR;
using Paycat.Api.Data;
using Paycat.Infrastructure;
using Paycat.Messages;
using Paycat.Messages.Enums;

namespace Paycat.Api.Services;

internal class TransactionService : IRequestHandler<CreatedTransaction, Result<ProcessedTransaction>>
{
    private readonly IMessenger _messenger;
    private readonly IRepository<Transaction> _transactionRepository;

    public TransactionService(IMessenger messenger, IRepository<Transaction> transactionRepository)
    {
        _messenger = messenger;
        _transactionRepository = transactionRepository;
    }

    public async Task<Result<ProcessedTransaction>> Handle(CreatedTransaction request, CancellationToken cancellationToken)
    {
        var transaction = await _transactionRepository.Get(request.Id);
        if (transaction is not null)
        {
            return Result<ProcessedTransaction>.Error("Can't duplicate transactions.");
        }

        transaction = new Transaction()
        {
            Id = request.Id,
            Amount = request.Amount,
            Date = request.Date,
            Status = TransactionStatus.New
        };
        await _transactionRepository.AddOrUpdate(transaction.Id, transaction);

        var processedTransaction = await _messenger.SendAsync<CreatedTransaction, Result<ProcessedTransaction>>(request, TimeSpan.FromSeconds(30))
            .ConfigureAwait(false);

        if (processedTransaction is not null && !processedTransaction.IsError)
        {
            transaction.Status = processedTransaction.Value.Status;
            transaction.Date = processedTransaction.Value.Date;
        }

        await _transactionRepository.AddOrUpdate(transaction.Id, transaction);

        return processedTransaction;
    }
}