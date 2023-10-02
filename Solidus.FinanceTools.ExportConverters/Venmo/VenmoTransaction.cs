using System;
using System.Globalization;

namespace Solidus.FinanceTools.ExportConverters.Venmo
{
    public class VenmoTransaction
    {
        public string TransactionID { get; set; }
        public string RawDate { get; set; }
        public DateTime? Date
        { 
            get {
                if (String.IsNullOrWhiteSpace(RawDate))
                    return null;
                
                var enUS = new CultureInfo("en-US");
                DateTime.TryParseExact(RawDate, enUS.DateTimeFormat.SortableDateTimePattern, enUS, DateTimeStyles.AssumeUniversal, out DateTime parsed);

                //TODO: Error handling?
                return parsed;
            } 
        }
        public string TransactionType { get; set; }
        public string Status { get; set; }
        public string Note { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string RawTotal { get; set; }
        public decimal? Total {
            get {
                return ParseCurrencyString(RawTotal);
            }
        }
        public string RawTip { get; set; }
        public decimal? Tip
        {
            get
            {
                return ParseCurrencyString(RawTip);
            }
        }
        public string RawTax { get; set; }
        public decimal? Tax
        {
            get
            {
                return ParseCurrencyString(RawTax);
            }
        }
        public string RawFee { get; set; }
        public decimal? Fee
        {
            get
            {
                return ParseCurrencyString(RawFee);
            }
        }
        public decimal? TaxRate { get; set; }
        public string TaxExempt { get; set; }
        public string FundingSource { get; set; }
        public string Destination { get; set; }
        public string RawBeginningBalance { get; set; }
        public decimal? BeginningBalance
        {
            get
            {
                if (String.IsNullOrWhiteSpace(RawBeginningBalance))
                    return null;

                //TODO: Stop assuming this is only USD even though Venmo never specifies the currency anywhere
                CurrencyTools.TryGetCurrencySymbol("USD", out string currSymbol);
                return Decimal.Parse(RawBeginningBalance.Replace(currSymbol, ""));
            }
        }
        public string RawEndingBalance { get; set; }
        public decimal? EndingBalance
        {
            get
            {
                if (String.IsNullOrWhiteSpace(RawEndingBalance))
                    return null;

                //TODO: Stop assuming this is only USD even though Venmo never specifies the currency anywhere
                CurrencyTools.TryGetCurrencySymbol("USD", out string currSymbol);
                return Decimal.Parse(RawEndingBalance.Replace(currSymbol, ""));
            }
        }
        public string RawStatementFees { get; set; }
        public decimal? StatementFees
        {
            get
            {
                return ParseCurrencyString(RawStatementFees);
            }
        }
        public string TerminalLocation { get; set; }
        public string RawYTDVenmoFees { get; set; }
        public decimal? YTDVenmoFees
        {
            get
            {
                return ParseCurrencyString(RawYTDVenmoFees);
            }
        }

        /// <summary>
        /// Parses a Venmo monetary string in to a decimal value.
        /// </summary>
        /// <param name="str">A string representation of a monetary value</param>
        /// <exception cref="FormatException">If the provided string <paramref name="str"/> could not be parsed into a decimal.</exception>
        /// <remarks>For some reason Venmo presents monetary values as a sign (+/-), followed by a space, and then a currency symbol</remarks>
        private static decimal? ParseCurrencyString(string str)
        {
            if (String.IsNullOrWhiteSpace(str))
                return null;

            //TODO: Stop assuming this is only USD even though Venmo never specifies the currency anywhere
            CurrencyTools.TryGetCurrencySymbol("USD", out string currSymbol);
            var isNegative = str[0] == '-';
            var cleaned = str.Substring(str.IndexOf(currSymbol) + 1).Trim();
            if (isNegative) cleaned = '-' + cleaned;

            return Decimal.Parse(cleaned);
        }
    }
}