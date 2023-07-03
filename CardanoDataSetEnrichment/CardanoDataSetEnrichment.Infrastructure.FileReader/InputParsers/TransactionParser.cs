using CardanoDataSetEnrichment.Domain.Transactions;
using CardanoDataSetEnrichment.Infrastructure.FileReader.Abstractios;
using System.Globalization;

namespace CardanoDataSetEnrichment.Infrastructure.FileReader.InputParsers
{
    public class TransactionParser : ITransactionParser
    {
        public ICollection<Transaction> Parse()
        {
            string filePath = "input_dataset.csv";
            string data = ReadEmbeddedResource(GetType(), filePath);

            List<Transaction> transactions = ParseTransactions(data);
            return transactions;
        }

        public string ReadEmbeddedResource(Type type, string resourceName)
        {
            string data;
            using Stream stream = type.Assembly.GetManifestResourceStream(type, resourceName);

            using (StreamReader reader = new StreamReader(stream))
            {
                data = reader.ReadToEnd();
            }
            return data;
        }

        public List<Transaction> ParseTransactions(string data)
        {
            string[] lines = data.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            List<Transaction> transactions = new();

            for (int i = 1; i < lines.Length; i++)
            {
                string[] fields = lines[i].Split(',');

                Transaction transaction = new Transaction();
                transaction.TransactionUTI = fields[0];
                transaction.ISIN = fields[1];
                transaction.Notional = double.Parse(fields[2], CultureInfo.InvariantCulture);
                transaction.NotionalCurrency = fields[3];
                transaction.TransactionType = fields[4];
                transaction.TransactionDateTime = DateTime.Parse(fields[5]);
                transaction.Rate = double.Parse(fields[6], CultureInfo.InvariantCulture);
                transaction.LEI = fields[7];

                transactions.Add(transaction);
            }

            return transactions;
        }
    }
}