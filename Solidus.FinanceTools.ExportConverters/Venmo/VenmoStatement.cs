using System;
using System.Collections.Generic;
using System.Linq;

namespace Solidus.FinanceTools.ExportConverters.Venmo
{
    public class VenmoStatement
    {
        public decimal? StartingBalance { get; set; }
        public decimal? EndingBalance { get; set; }
        public DateTime? StatementDate { get; set; }
        public string Account { get; set; }

        public List<VenmoTransaction> Transactions { get; set; }

        public VenmoStatement(string rawHeader, IEnumerable<VenmoTransaction> rawTxns)
        {
            //Account Statement - (@<username>) - <starting date> to <ending date>
            var acctStartPos = rawHeader.IndexOf('(') + 1;
            Account = rawHeader.Substring(acctStartPos, rawHeader.IndexOf(')', acctStartPos) - acctStartPos);
            //TODO: Parse Statement Date eventually

            StartingBalance = rawTxns.FirstOrDefault(txn => txn.BeginningBalance.HasValue)?.BeginningBalance;
            EndingBalance = rawTxns.FirstOrDefault(txn => txn.EndingBalance.HasValue)?.EndingBalance;

            Transactions = rawTxns.ToList();
            Transactions.RemoveAll(txn => txn.BeginningBalance.HasValue || txn.EndingBalance.HasValue);
        }
    }
}
