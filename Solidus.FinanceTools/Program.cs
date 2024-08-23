using CsvHelper;
using CsvHelper.Configuration;
using Solidus.FinanceTools.ExportConverters.CashApp;
using Solidus.FinanceTools.ExportConverters.Discover;
using Solidus.FinanceTools.ExportConverters.Venmo;
using System.CommandLine;
using System.Globalization;

namespace Solidus.FinanceTools
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            var returnCode = 0;
            
            var typeArgument = new Argument<string>("Input Type", "The type of the input file to convert. Only 'CashApp', 'DiscoverBank', 'DiscoverCard', and 'Venmo' are supported at the moment.");

            var inputOption = new Option<FileInfo?>("--input", "Path to a CashApp CSV export file.") { IsRequired = true };
            inputOption.AddAlias("-i");
            var outputOption = new Option<FileInfo?>("--output", "Path to output QIF file to.") { IsRequired = false };
            outputOption.AddAlias("-o");

            var rootCommand = new RootCommand("Converts the provided transaction export file into QIF format.")
            {
                typeArgument,
                inputOption,
                outputOption
            };

            rootCommand.SetHandler((type, input, output) =>
            {
                try
                {
                    if (type.Equals("Venmo", StringComparison.InvariantCultureIgnoreCase))
                        ConvertVenmoFile(input!, output);
                    else
                        ConvertFile(type, input!, output);
                }
                catch (ArgumentException ex)
                {
                    returnCode = 1;
                }
            }, typeArgument, inputOption, outputOption);

            await rootCommand.InvokeAsync(args);

            return returnCode;
        }

        /// <summary>
        /// Reads in a provided <paramref name="inputFile"/> containing financial transactions and converts it into QIF format.
        /// </summary>
        /// <param name="conversionType">A supported converstion type. Only 'CashApp', 'DiscoverBank', 'DiscoverCard' are supported at the moment.</param>
        /// <param name="inputFile">The CSV file to convert</param>
        /// <param name="outputFile">The QIF file to produce</param>
        /// <exception cref="ArgumentException">If the provided <paramref name="conversionType"/> is not supported.</exception>
        static void ConvertFile(string conversionType, FileInfo inputFile, FileInfo? outputFile = null)
        {
            var outputPath = outputFile == null ? Path.Combine(inputFile.DirectoryName, Path.GetFileNameWithoutExtension(inputFile.Name) + ".qif") : outputFile.FullName;

            using (var reader = new StreamReader(inputFile.FullName))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            using (var writer = File.CreateText(outputPath))
            {
                csv.Context.RegisterClassMap<CashAppExportMapper>();
                csv.Context.RegisterClassMap<DiscoverBankExportMapper>();
                csv.Context.RegisterClassMap<DiscoverCardExportMapper>();

                switch (conversionType)
                {
                    case "CashApp":
                        csv.GetRecords<CashAppTransaction>().ToList().ExportAsQIFFile(writer);
                        break;
                    case "DiscoverBank":
                        csv.GetRecords<DiscoverBankTransaction>().ToList().ExportAsQIFFile(writer);
                        break;
                    case "DiscoverCard":
                        csv.GetRecords<DiscoverCardTransaction>().ToList().ExportAsQIFFile(writer);
                        break;
                    default:
                        throw new ArgumentException("Invalid conversion type requested.", nameof(conversionType));
                }
            }
        }

        /// <summary>
        /// Reads in a provided <paramref name="inputFile"/> containing Venmo financial transactions and converts it into QIF format.
        /// </summary>
        /// <param name="inputFile">The CSV file to convert</param>
        /// <param name="outputFile">The QIF file to produce</param>
        static void ConvertVenmoFile(FileInfo inputFile, FileInfo? outputFile = null)
        {
            var outputPath = outputFile == null ? Path.Combine(inputFile.DirectoryName, Path.GetFileNameWithoutExtension(inputFile.Name) + ".qif") : outputFile.FullName;

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                MissingFieldFound = null,
                ShouldSkipRecord = (records) =>
                {
                    return records.Row.Parser.Record.All(String.IsNullOrWhiteSpace) || !String.IsNullOrWhiteSpace(records.Row[0]);
                }
            };

            var rawHeader = "";

            using (var reader = new StreamReader(inputFile.FullName))
            using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)))
            {
                csv.Read();
                rawHeader = csv.GetField(0);
            }

            using (var reader = new StreamReader(inputFile.FullName))
            using (var csv = new CsvReader(reader, config))
            using (var writer = File.CreateText(outputPath))
            {
                csv.Context.RegisterClassMap<VenmoExportMapper>();
                
                var rawTxns = csv.GetRecords<VenmoTransaction>().ToList();
                var statement = new VenmoStatement(rawHeader, rawTxns);
                statement.ExportAsQIFFile(writer);
            }
        }
    }
}