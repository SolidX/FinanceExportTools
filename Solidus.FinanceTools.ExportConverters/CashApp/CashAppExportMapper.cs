using CsvHelper.Configuration;

namespace Solidus.FinanceTools.ExportConverters.CashApp
{
    public class CashAppExportMapper : ClassMap<CashAppTransaction>
    {
        public CashAppExportMapper()
        {
            Map(m => m.TransactionID).Name("Transaction ID");
            Map(m => m.RawDate).Name("Date");
            Map(m => m.TransactionType).Name("Transaction Type");
            Map(m => m.Currency).Name("Currency");
            Map(m => m.RawAmount).Name("Amount");
            Map(m => m.RawFee).Name("Fee");
            Map(m => m.RawNetAmount).Name("Net Amount");
            Map(m => m.AssetType).Name("Asset Type");
            Map(m => m.AssetPrice).Name("Asset Price");
            Map(m => m.AssetAmount).Name("Asset Amount");
            Map(m => m.Status).Name("Status");
            Map(m => m.Notes).Name("Notes");
            Map(m => m.SenderReceiverName).Name("Name of sender/receiver");
            Map(m => m.Account).Name("Account");
        }
    }
}
