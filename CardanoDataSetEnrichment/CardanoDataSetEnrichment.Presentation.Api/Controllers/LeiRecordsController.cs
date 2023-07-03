using CardanoDataSetEnrichment.Application.Abstractios;
using CardanoDataSetEnrichment.Domain.Transactions;
using Microsoft.AspNetCore.Mvc;

namespace CardanoDataSetEnrichment.Presentation.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeiRecordsController : Controller
    {
        [HttpGet]
        public async Task<ActionResult<ICollection<Transaction>>>GetEnrichedTransactions(
            [FromServices] ITransactionEnrichment transactionEnrichment)
        {
            ICollection<Transaction> transactions = await transactionEnrichment.GetEntichedTransactiosAsync();

            return Ok(transactions);
        }
    }
}
