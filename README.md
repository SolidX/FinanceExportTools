# Finance Export Conversion Tools
This tool converts a variety of financial transaction export CSV files into QIF format.
It was originally created to simplify importing CashApp transactions in to [HomeBank](http://homebank.free.fr/en/index.php).

## Currently Supported CSV to QIF Conversions
- CashApp Transactions
- Discover Bank Statement
- Discover Card Statement

## CLI Usage
```
Usage:
  Solidus.FinanceTools <Input Type> [options]

Arguments:
  <Input Type>  The type of the input file to convert. Only 'CashApp', 'DiscoverBank', 'DiscoverCard' are supported at
                the moment.

Options:
  -i, --input <input> (REQUIRED)  Path to a CashApp CSV export file.
  -o, --output <output>           Path to output QIF file to.
  --version                       Show version information
  -?, -h, --help                  Show help and usage information
```