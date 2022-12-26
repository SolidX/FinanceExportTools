using System;

namespace Solidus.FinanceTools.ExportConverters.Discover
{
    public class DiscoverCardTransaction
    {
        public DateTime TransactionDate { get; set; }
        public DateTime PostDate { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public string Category { get; set; }
    }
}
