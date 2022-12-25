using MediatR;
using Microsoft.AspNetCore.Mvc;
using Paycat.Api.Data;
using Paycat.Api.Models;
using Paycat.Infrastructure;
using Paycat.Messages;
using Paycat.Messages.Enums;

namespace Paycat.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TransactionsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [ProducesResponseType(typeof(Transaction), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Transaction), StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPut()]
        public async Task<IActionResult> Create([FromBody] CreateTransaction createTransaction)
        {
            var transaction = new Transaction()
            {
                Id = createTransaction.Id,
                Amount = createTransaction.Amount,
                Status = TransactionStatus.New,
                Date = DateTimeOffset.Now
            };
            var createdTransaction = new CreatedTransaction()
            {
                Id = createTransaction.Id
            };
            var processedTransaction = await _mediator.Send(createdTransaction)
                .ConfigureAwait(false);

            if (processedTransaction.IsError)
            {
                return BadRequest(processedTransaction.ErrorMessage);
            }

            return Ok(transaction);
        }
    }
}