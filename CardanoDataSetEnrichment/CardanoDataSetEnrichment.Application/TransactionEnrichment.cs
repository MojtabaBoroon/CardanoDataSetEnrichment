using CardanoDataSetEnrichment.Application.Abstractios;
using CardanoDataSetEnrichment.Domain.LeiRecordDtos;
using CardanoDataSetEnrichment.Domain.Transactions;
using CardanoDataSetEnrichment.Infrastructure.ExternalServices.Abstractions;
using CardanoDataSetEnrichment.Infrastructure.FileReader.Abstractios;

namespace CardanoDataSetEnrichment.Application
{
    public class TransactionEnrichment : ITransactionEnrichment
    {
        private readonly IGLeifApiClient _gLeifApiClient;
        private readonly ITransactionParser _transactionParser;

        private ICollection<Transaction> _transactions;


        public TransactionEnrichment(IGLeifApiClient gLeifApiClient, ITransactionParser transactionParser)
        {
            _gLeifApiClient = gLeifApiClient;
            _transactionParser = transactionParser;
        }

        public async Task<ICollection<Transaction>> GetEntichedTransactiosAsync()
        {
            _transactions = _transactionParser.Parse();

            List<LeiRecordDto> leiRecords = await GetLeiRecord();

            foreach (var pair in _transactions.Zip(leiRecords, (item1, item2) => new { transaction = item1, leiRecord = item2 }))
            {
                EnrichTransaction(pair.transaction, pair.leiRecord);
            }

            return _transactions;
        }

        private async Task<List<LeiRecordDto>> GetLeiRecord()
        {
            List<Task<LeiRecordDto>> tasks = new();

            foreach (var transaction in _transactions)
            {
                tasks.Add(_gLeifApiClient.GetTransactioByLeiAsync(transaction.LEI));
            }
            await Task.WhenAll(tasks);
            var leiRecords = tasks.Select(x => x.Result).ToList();
            return leiRecords;
        }

        private void EnrichTransaction(Transaction transaction, LeiRecordDto leiRecord)
        {
            transaction.Bic = leiRecord.Data.FirstOrDefault().Attributes.Bic;
            transaction.LegalName.Name = leiRecord.Data.FirstOrDefault().Attributes.Entity.LegalName.Name;
            transaction.LegalName.Language = leiRecord.Data.FirstOrDefault().Attributes.Entity.LegalName.Language;
            transaction.TransactionCosts = CalculateTransactionCosts(transaction, leiRecord);
        }

        private double CalculateTransactionCosts(Transaction transaction, LeiRecordDto leiRecord)
        {
            double transactionCosts = default;

            if (leiRecord.Data.FirstOrDefault().Attributes.Entity.LegalAddress.Country.ToLower() == "gb")
            {
                transactionCosts = transaction.Notional * transaction.Rate - transaction.Notional;
            }
            if (leiRecord.Data.FirstOrDefault().Attributes.Entity.LegalAddress.Country.ToLower() == "nl")
            {
                var a = (transaction.Notional * (1 / transaction.Rate)) - transaction.Notional;
                transactionCosts = Math.Abs((transaction.Notional * (1 / transaction.Rate)) - transaction.Notional); ;
            }
            return transactionCosts;
        }
    }
}
