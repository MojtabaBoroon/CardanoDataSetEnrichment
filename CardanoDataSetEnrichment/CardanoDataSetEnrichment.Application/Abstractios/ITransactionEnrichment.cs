
using CardanoDataSetEnrichment.Domain.Transactions;

namespace CardanoDataSetEnrichment.Application.Abstractios
{
    public interface ITransactionEnrichment
    {
        Task<ICollection<Transaction>> GetEntichedTransactiosAsync();
    }
}
