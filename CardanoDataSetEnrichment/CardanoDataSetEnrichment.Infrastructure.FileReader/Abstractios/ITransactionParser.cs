using CardanoDataSetEnrichment.Domain.Transactions;

namespace CardanoDataSetEnrichment.Infrastructure.FileReader.Abstractios
{
    public interface ITransactionParser
    {
        public ICollection<Transaction> Parse();

    }
}
