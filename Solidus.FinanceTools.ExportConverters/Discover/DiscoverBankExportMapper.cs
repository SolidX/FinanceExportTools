using CsvHelper.Configuration;

namespace Solidus.FinanceTools.ExportConverters.Discover
{
    public class DiscoverBankExportMapper : ClassMap<DiscoverBankTransaction>
    {
        public DiscoverBankExportMapper()
        {
            Map(m => m.TransactionDate).Name("Transaction Date");
            Map(m => m.Description).Name("Transaction Description");
            Map(m => m.TransactionType).Name("Transaction Type");
            Map(m => m.RawDebit).Name("Debit");
            Map(m => m.RawCredit).Name("Credit");
            Map(m => m.RawBalance).Name("Balance");
        }
    }
}
