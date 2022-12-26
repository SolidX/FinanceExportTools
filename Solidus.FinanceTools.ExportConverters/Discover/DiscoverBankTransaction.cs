using System;

namespace Solidus.FinanceTools.ExportConverters.Discover
{
    public class DiscoverBankTransaction
    {
        public DateTime TransactionDate { get; set; }
        public string Description { get; set; }
        public string TransactionType { get; set; }
        public string RawDebit { get; set; }
        public string RawCredit { get; set; }
        public string RawBalance { get; set; }
        public decimal Debit
        {
            get
            {
                CurrencyTools.TryGetCurrencySymbol("USD", out string currSymbol);
                return Decimal.Parse(RawDebit.Replace(currSymbol, ""));
            }
        }
        public decimal Credit
        {
            get
            {
                CurrencyTools.TryGetCurrencySymbol("USD", out string currSymbol);
                return Decimal.Parse(RawCredit.Replace(currSymbol, ""));
            }
        }
        public decimal Balance
        {
            get
            {
                CurrencyTools.TryGetCurrencySymbol("USD", out string currSymbol);
                return Decimal.Parse(RawBalance.Replace(currSymbol, ""));
            }
        }
    }
}
