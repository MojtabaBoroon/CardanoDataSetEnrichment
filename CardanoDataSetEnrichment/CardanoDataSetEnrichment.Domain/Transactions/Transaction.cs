using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardanoDataSetEnrichment.Domain.Transactions
{
    public class Transaction
    {
        public string TransactionUTI { get; set; }
        public string ISIN { get; set; }
        public double Notional { get; set; }
        public string NotionalCurrency { get; set; }
        public string TransactionType { get; set; }
        public DateTime TransactionDateTime { get; set; }
        public double Rate { get; set; }
        public string LEI { get; set; }
        public LegalName LegalName { get; set; } = new();
        public ICollection<string> Bic { get; set; }
        public double TransactionCosts { get; set; }

    }
}
