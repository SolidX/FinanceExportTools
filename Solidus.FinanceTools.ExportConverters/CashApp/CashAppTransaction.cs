using System;
using System.Globalization;

namespace Solidus.FinanceTools.ExportConverters.CashApp
{
    public class CashAppTransaction
    {
        public string TransactionID { get; set; }
        public string RawDate { get; set; }
        public DateTime Date
        { 
            get {
                var enUS = new CultureInfo("en-US");

                string dt;
                var tz = RawDate.Substring(RawDate.LastIndexOf(" ") + 1);

                if (ExportExtensions.TIMEZONE_OFFSET_MAP.ContainsKey(tz))
                    dt = RawDate.Replace(tz, ExportExtensions.TIMEZONE_OFFSET_MAP[tz]);
                else
                    dt = RawDate;

                DateTime.TryParseExact(dt, "yyyy-MM-dd HH:mm:ss zzz", enUS, DateTimeStyles.None, out DateTime parsed);

                //TODO: Error handling?
                return parsed;
            } 
        }
        public string TransactionType { get; set; }
        public string Currency { get; set; }
        public string RawAmount { get; set; }
        public decimal Amount { 
            get {
                CurrencyTools.TryGetCurrencySymbol(Currency, out string currSymbol);
                return Decimal.Parse(RawAmount.Replace(currSymbol, ""));
            }
        }
        public string RawFee { get; set; }
        public decimal Fee
        {
            get
            {
                CurrencyTools.TryGetCurrencySymbol(Currency, out string currSymbol);
                return Decimal.Parse(RawFee.Replace(currSymbol, ""));
            }
        }
        public string RawNetAmount { get; set; }
        public decimal NetAmount
        {
            get
            {
                CurrencyTools.TryGetCurrencySymbol(Currency, out string currSymbol);
                return Decimal.Parse(RawNetAmount.Replace(currSymbol, ""));
            }
        }
        public string AssetType { get; set; }
        public decimal? AssetPrice { get; set; }
        public decimal? AssetAmount { get; set; }
        public string Status { get; set; }
        public string Notes { get; set; }
        public string SenderReceiverName { get; set; }
        public string Account { get; set; }
    }
}