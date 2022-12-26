using CsvHelper.Configuration;

namespace Solidus.FinanceTools.ExportConverters.Discover
{
    public class DiscoverCardExportMapper : ClassMap<DiscoverCardTransaction>
    {
        public DiscoverCardExportMapper()
        {
            Map(m => m.TransactionDate).Name("Trans. Date");
            Map(m => m.PostDate).Name("Post Date");
            Map(m => m.Description).Name("Description");
            Map(m => m.Amount).Name("Amount");
            Map(m => m.Category).Name("Category");
        }
    }
}
