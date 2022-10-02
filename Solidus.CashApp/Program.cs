using CsvHelper;
using Solidus.CashApp.ExportConverter;
using System.CommandLine;
using System.Globalization;

namespace Solidus.CashAppExport
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            var inputOption = new Option<FileInfo?>("--input", "Path to a CashApp CSV export file to convert in to QIF format.") { IsRequired = true };
            var outputOption = new Option<FileInfo?>("--output", "Path to a CashApp CSV export file to convert in to QIF format.") { IsRequired = false };

            var rootCommand = new RootCommand("Converts CashApp CSV export files in to QIF format.")
            {
                inputOption,
                outputOption
            };

            rootCommand.SetHandler((input, output) =>
            {
                ConvertFile(input!, output);
            }, inputOption, outputOption);

            return await rootCommand.InvokeAsync(args);
        }


        static void ConvertFile(FileInfo inputFile, FileInfo? outputFile = null)
        {
            var outputPath = outputFile == null ? Path.Combine(inputFile.DirectoryName, Path.GetFileNameWithoutExtension(inputFile.Name) + ".qif") : outputFile.FullName;

            using (var reader = new StreamReader(inputFile.FullName))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            using (var writer = File.CreateText(outputPath))
            {
                csv.Context.RegisterClassMap<CashAppExportMapper>();

                var rows = csv.GetRecords<CashAppTransaction>().ToList();
                rows.ExportAsQIFFile(writer);
            }
        }
    }
}