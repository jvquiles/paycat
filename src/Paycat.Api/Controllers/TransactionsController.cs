﻿using MediatR;
using Microsoft.AspNetCore.Mvc;
using Paycat.Api.Data;
using Paycat.Api.Models;
using Paycat.Messages;

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
            var createdTransaction = new CreatedTransaction()
            {
                Id = createTransaction.Id
            };
            var processedTransaction = await _mediator.Send(createdTransaction)
                .ConfigureAwait(false);

            if (processedTransaction.Value is not null 
                || processedTransaction.IsError)
            {
                return BadRequest(processedTransaction.ErrorMessage);
            }

            return Ok(processedTransaction.Value);
        }
    }
}