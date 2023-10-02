using CsvHelper.Configuration;

namespace Solidus.FinanceTools.ExportConverters.Venmo
{
    public class VenmoExportMapper : ClassMap<VenmoTransaction>
    {
        public VenmoExportMapper()
        {
            Map().Index(0).Ignore(); //Blank
            Map(m => m.TransactionID).Name("ID");
            Map(m => m.RawDate).Name("Datetime");
            Map(m => m.TransactionType).Name("Type");
            Map(m => m.Status).Name("Status");
            Map(m => m.Note).Name("Note");
            Map(m => m.From).Name("From");
            Map(m => m.To).Name("To");
            Map(m => m.RawTotal).Name("Amount (total)");
            Map(m => m.RawTip).Name("Amount (tip)");
            Map(m => m.RawTax).Name("Amount (tax)");
            Map(m => m.RawFee).Name("Amount (fee)");
            Map(m => m.TaxRate).Name("Tax Rate");
            Map(m => m.TaxExempt).Name("Tax Exempt");
            Map(m => m.FundingSource).Name("Funding Source");
            Map(m => m.Destination).Name("Destination");
            Map(m => m.RawBeginningBalance).Name("Beginning Balance");
            Map(m => m.RawEndingBalance).Name("Ending Balance");
            Map(m => m.RawStatementFees).Name("Statement Period Venmo Fees");
            Map(m => m.TerminalLocation).Name("Terminal Location");
            Map(m => m.RawYTDVenmoFees).Name("Year to Date Venmo Fees");
            Map().Index(21).Ignore(); //Disclaimer
        }
    }
}
