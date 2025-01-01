﻿using IonTechnologies.Qif;
using IonTechnologies.Qif.Transactions;
using Solidus.FinanceTools.ExportConverters.CashApp;
using Solidus.FinanceTools.ExportConverters.Discover;
using Solidus.FinanceTools.ExportConverters.Venmo;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Solidus.FinanceTools
{
    public static class ExportExtensions
    {
        public static readonly Dictionary<string, string> TIMEZONE_OFFSET_MAP = new Dictionary<string, string>() {
            {"ACDT", "+1030"},
            {"ACST", "+0930"},
            {"ADT", "-0300"},
            {"AEDT", "+1100"},
            {"AEST", "+1000"},
            {"AHDT", "-0900"},
            {"AHST", "-1000"},
            {"AST", "-0400"},
            {"AT", "-0200"},
            {"AWDT", "+0900"},
            {"AWST", "+0800"},
            {"BAT", "+0300"},
            {"BDST", "+0200"},
            {"BET", "-1100"},
            {"BST", "-0300"},
            {"BT", "+0300"},
            {"BZT2", "-0300"},
            {"CADT", "+1030"},
            {"CAST", "+0930"},
            {"CAT", "-1000"},
            {"CCT", "+0800"},
            {"CDT", "-0500"},
            {"CED", "+0200"},
            {"CET", "+0100"},
            {"CEST", "+0200"},
            {"CST", "-0600"},
            {"EAST", "+1000"},
            {"EDT", "-0400"},
            {"EED", "+0300"},
            {"EET", "+0200"},
            {"EEST", "+0300"},
            {"EST", "-0500"},
            {"FST", "+0200"},
            {"FWT", "+0100"},
            {"GMT", "GMT"},
            {"GST", "+1000"},
            {"HDT", "-0900"},
            {"HST", "-1000"},
            {"IDLE", "+1200"},
            {"IDLW", "-1200"},
            {"IST", "+0530"},
            {"IT", "+0330"},
            {"JST", "+0900"},
            {"JT", "+0700"},
            {"MDT", "-0600"},
            {"MED", "+0200"},
            {"MET", "+0100"},
            {"MEST", "+0200"},
            {"MEWT", "+0100"},
            {"MST", "-0700"},
            {"MT", "+0800"},
            {"NDT", "-0230"},
            {"NFT", "-0330"},
            {"NT", "-1100"},
            {"NST", "+0630"},
            {"NZ", "+1100"},
            {"NZST", "+1200"},
            {"NZDT", "+1300"},
            {"NZT", "+1200"},
            {"PDT", "-0700"},
            {"PST", "-0800"},
            {"ROK", "+0900"},
            {"SAD", "+1000"},
            {"SAST", "+0900"},
            {"SAT", "+0900"},
            {"SDT", "+1000"},
            {"SST", "+0200"},
            {"SWT", "+0100"},
            {"USZ3", "+0400"},
            {"USZ4", "+0500"},
            {"USZ5", "+0600"},
            {"USZ6", "+0700"},
            {"UT", "-0000"},
            {"UTC", "-0000"},
            {"UZ10", "+1100"},
            {"WAT", "-0100"},
            {"WET", "-0000"},
            {"WST", "+0800"},
            {"YDT", "-0800"},
            {"YST", "-0900"},
            {"ZP4", "+0400"},
            {"ZP5", "+0500"},
            {"ZP6", "+0600"}
        };

        #region CashApp
        /// <summary>
        /// Converts an IEnumerable of CashAppTransactions List of QIF Transactions
        /// </summary>
        /// <param name="transactions">The CashApp transactions to convert</param>
        /// <returns>A list of QIF transactions</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="transactions"/> is null</exception>
        public static IEnumerable<BasicTransaction> ToQifTransactionList(this IEnumerable<CashAppTransaction> transactions)
        {
            if (transactions == null) throw new ArgumentNullException(nameof(transactions));
            if (!transactions.Any()) return Enumerable.Empty<BasicTransaction>();

            var output = new List<BasicTransaction>();
            foreach (var txn in transactions)
            {
                var transaction = new BasicTransaction
                {
                    Date = txn.Date.ToUniversalTime(),
                    Number = txn.TransactionID,
                    ClearedStatus = txn.Status.Equals("Complete", StringComparison.InvariantCultureIgnoreCase) ? "c" : "",
                    Payee = txn.SenderReceiverName,
                    Memo = txn.Notes,
                    Amount = txn.Amount
                    //TODO: Split transaction for anything with a Fee
                };

                output.Add(transaction);
            }

            return output;
        }

        /// <summary>
        /// Generates and outputs a QIF file from the given <paramref name="transactions"/>
        /// </summary>
        /// <param name="transactions">A List of CashAppTransactions</param>
        /// <param name="output">Stream to write QIF file to</param>
        public static void ExportAsQIFFile(this List<CashAppTransaction> transactions, Stream output)
        {
            QifDocument doc = new QifDocument();
            var qif = transactions.ToQifTransactionList();

            foreach (var t in qif)
                doc.CashTransactions.Add(t);

            doc.Save(output);
        }

        /// <summary>
        /// Generates and outputs a QIF file from the given <paramref name="transactions"/>
        /// </summary>
        /// <param name="transactions">A List of CashAppTransactions</param>
        /// <param name="output">TextWriter to write QIF file to</param>
        public static void ExportAsQIFFile(this List<CashAppTransaction> transactions, TextWriter output)
        {
            QifDocument doc = new QifDocument();
            var qif = transactions.Where(t => t.TransactionType != "Account Notifications").ToQifTransactionList();

            foreach (var t in qif)
                doc.CashTransactions.Add(t);

            doc.Save(output);
        }
        #endregion
        #region Discover Bank
        /// <summary>
        /// Converts an IEnumerable of DiscoverBankTransaction List of QIF Transactions
        /// </summary>
        /// <param name="transactions">The Discover Bank transactions to convert</param>
        /// <returns>A list of QIF transactions</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="transactions"/> is null</exception>
        public static IEnumerable<BasicTransaction> ToQifTransactionList(this List<DiscoverBankTransaction> transactions)
        {
            if (transactions == null) throw new ArgumentNullException(nameof(transactions));
            if (!transactions.Any()) return Enumerable.Empty<BasicTransaction>();

            var output = new List<BasicTransaction>();
            foreach (var txn in transactions)
            {
                var transaction = new BasicTransaction
                {
                    Date = txn.TransactionDate.ToUniversalTime(),
                    ClearedStatus = "c",
                    Memo = txn.Description,
                    Amount = txn.Credit != 0 ? txn.Credit : txn.Debit * -1
                };

                output.Add(transaction);
            }

            return output;
        }

        /// <summary>
        /// Generates and outputs a QIF file from the given <paramref name="transactions"/>
        /// </summary>
        /// <param name="transactions">A List of DiscoverBankTransactions</param>
        /// <param name="output">Stream to write QIF file to</param>
        public static void ExportAsQIFFile(this List<DiscoverBankTransaction> transactions, Stream output)
        {
            QifDocument doc = new QifDocument();
            var qif = transactions.ToQifTransactionList();

            var acct = new AccountListTransaction
            {
                Name = "Discover",
                Type = "Bank"
            };
            doc.AccountListTransactions.Add(acct);

            foreach (var t in qif)
                doc.BankTransactions.Add(t);

            doc.Save(output);
        }

        /// <summary>
        /// Generates and outputs a QIF file from the given <paramref name="transactions"/>
        /// </summary>
        /// <param name="transactions">A List of DiscoverBankTransactions</param>
        /// <param name="output">TextWriter to write QIF file to</param>
        public static void ExportAsQIFFile(this List<DiscoverBankTransaction> transactions, TextWriter output)
        {
            QifDocument doc = new QifDocument();
            var qif = transactions.ToQifTransactionList();

            var acct = new AccountListTransaction
            {
                Name = "Discover",
                Type = "Bank"
            };
            doc.AccountListTransactions.Add(acct);

            foreach (var t in qif)
                doc.BankTransactions.Add(t);

            doc.Save(output);
        }
        #endregion
        #region Discover Card
        /// <summary>
        /// Converts an IEnumerable of DiscoverCardTransaction List of QIF Transactions
        /// </summary>
        /// <param name="transactions">The Discover Credit Card transactions to convert</param>
        /// <returns>A list of QIF transactions</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="transactions"/> is null</exception>
        public static IEnumerable<BasicTransaction> ToQifTransactionList(this List<DiscoverCardTransaction> transactions)
        {
            if (transactions == null) throw new ArgumentNullException(nameof(transactions));
            if (!transactions.Any()) return Enumerable.Empty<BasicTransaction>();

            var output = new List<BasicTransaction>();
            foreach (var txn in transactions)
            {
                var transaction = new BasicTransaction
                {
                    Date = txn.TransactionDate.ToUniversalTime(),
                    ClearedStatus = "c",
                    Memo = txn.Description,
                    Amount = txn.Amount * -1,
                    Category = txn.Category
                };

                output.Add(transaction);
            }

            return output;
        }

        /// <summary>
        /// Generates and outputs a QIF file from the given <paramref name="transactions"/>
        /// </summary>
        /// <param name="transactions">A List of DiscoverCardTransaction</param>
        /// <param name="output">Stream to write QIF file to</param>
        public static void ExportAsQIFFile(this List<DiscoverCardTransaction> transactions, Stream output)
        {
            QifDocument doc = new QifDocument();
            var qif = transactions.ToQifTransactionList();

            var acct = new AccountListTransaction
            {
                Name = "Discover",
                Type = "CCard"
            };
            doc.AccountListTransactions.Add(acct);

            foreach (var t in qif)
                doc.CreditCardTransactions.Add(t);

            doc.Save(output);
        }

        /// <summary>
        /// Generates and outputs a QIF file from the given <paramref name="transactions"/>
        /// </summary>
        /// <param name="transactions">A List of DiscoverCardTransaction</param>
        /// <param name="output">TextWriter to write QIF file to</param>
        public static void ExportAsQIFFile(this List<DiscoverCardTransaction> transactions, TextWriter output)
        {
            QifDocument doc = new QifDocument();
            var qif = transactions.ToQifTransactionList();

            var acct = new AccountListTransaction
            {
                Name = "Discover",
                Type = "CCard"
            };
            doc.AccountListTransactions.Add(acct);

            foreach (var t in qif)
                doc.CreditCardTransactions.Add(t);

            doc.Save(output);
        }
        #endregion
        #region Venmo
        /// <summary>
        /// Converts an IEnumerable of VenmoTransactions List of QIF Transactions
        /// </summary>
        /// <param name="transactions">The Venmo transactions to convert</param>
        /// <returns>A list of QIF transactions</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="transactions"/> is null</exception>
        public static IEnumerable<BasicTransaction> ToQifTransactionList(this List<VenmoTransaction> transactions)
        {
            if (transactions == null) throw new ArgumentNullException(nameof(transactions));

            var output = new List<BasicTransaction>();
            foreach (var txn in transactions)
            {
                if (txn.BeginningBalance.HasValue || txn.EndingBalance.HasValue)
                    continue;

                var transaction = new BasicTransaction
                {
                    Date = txn.Date.HasValue ? txn.Date.Value.ToLocalTime() : DateTime.Today,
                    Number = txn.TransactionID,
                    ClearedStatus = txn.Status.Equals("Complete", StringComparison.InvariantCultureIgnoreCase) ? "c" : "",
                    Payee = txn.Total.Value >= 0 ? txn.From : txn.To,
                    Memo = txn.Note,
                    Amount = txn.Total.Value
                    //TODO: Split transaction for anything with a Fee
                };

                output.Add(transaction);
            }

            return output;
        }

        /// <summary>
        /// Generates and outputs a QIF file from the given <paramref name="statement"/>
        /// </summary>
        /// <param name="statement">A Venmo Statement</param>
        /// <param name="output">Stream to write QIF file to</param>
        public static void ExportAsQIFFile(this VenmoStatement statement, Stream output)
        {
            QifDocument doc = new QifDocument();

            var acct = new AccountListTransaction
            {
                Name = $"Venmo - {statement.Account}"
            };

            if (statement.EndingBalance.HasValue)
                acct.StatementBalance = statement.EndingBalance.Value;

            doc.AccountListTransactions.Add(acct);

            var qif = statement.Transactions.ToQifTransactionList();

            foreach (var t in qif)
                doc.CashTransactions.Add(t);

            doc.Save(output);
        }

        /// <summary>
        /// Generates and outputs a QIF file from the given <paramref name="statement"/>
        /// </summary>
        /// <param name="statement">A Venmo Statement</param>
        /// <param name="output">TextWriter to write QIF file to</param>
        public static void ExportAsQIFFile(this VenmoStatement statement, TextWriter output)
        {
            QifDocument doc = new QifDocument();

            var acct = new AccountListTransaction
            {
                Name = $"Venmo - {statement.Account}"
            };

            if (statement.EndingBalance.HasValue)
                acct.StatementBalance = statement.EndingBalance.Value;

            doc.AccountListTransactions.Add(acct);

            var qif = statement.Transactions.ToQifTransactionList();

            foreach (var t in qif)
                doc.CashTransactions.Add(t);

            doc.Save(output);
        }
        #endregion
    }
}
