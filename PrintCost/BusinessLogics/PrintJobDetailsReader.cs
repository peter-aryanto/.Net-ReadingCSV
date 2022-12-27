using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PrintCost.DomainObjects;

namespace PrintCost.BusinessLogics
{
  public interface IPrintJobDetailsReader
  {
    public PrintJobDetails ReadPrintJobDetailsCsvRow(string csvRow);
  }

  public class PrintJobDetailsReader : IPrintJobDetailsReader
  {
    public PrintJobDetails ReadPrintJobDetailsCsvRow(string csvRow)
    {
      if (string.IsNullOrWhiteSpace(csvRow))
      {
        return null;
      }

      string[] inputCsvColumns = csvRow.Split(',');
      int expectedColumnCount = PrintJobDetails.CsvColumns.Length;

      if (inputCsvColumns.Length != PrintJobDetails.CsvColumns.Length)
      {
        string commaDelimitedCsvColumns = string.Join(", ", PrintJobDetails.CsvColumns);
        throw new Exception($"Found {inputCsvColumns.Length} columns(s) "
          + $"while expecting {expectedColumnCount} columns: "
          + $"{commaDelimitedCsvColumns}.");
      }

      var output = new PrintJobDetails();
      for (int index = 0; index < expectedColumnCount; index++)
      {
        string csvColumnValue = inputCsvColumns[index].Trim();
        switch (index)
        {
          case 0:
            output.TotalNumberOfPages = int.Parse(csvColumnValue);
            if (output.TotalNumberOfPages <= 0)
            {
              throw new Exception($"Invalid total number of pages ({output.TotalNumberOfPages}).");
            }

            break;
          case 1:
            output.NumberOfColourPages = int.Parse(csvColumnValue);
            if (output.NumberOfColourPages < 0)
            {
              throw new Exception($"Invalid number of colour pages ({output.NumberOfColourPages}).");
            }
            if (output.NumberOfColourPages > output.TotalNumberOfPages)
            {
              throw new Exception($"Number of colour pages ({output.NumberOfColourPages}) "
                + $"should not be more than total number of pages ({output.TotalNumberOfPages}).");
            }

            output.NumberOfBlackAndWhitePages =
              output.TotalNumberOfPages - output.NumberOfColourPages;
            break;
          case 2:
            output.IsDoubleSided = bool.Parse(csvColumnValue);
            break;
          default:
            throw new Exception($"Invalid column[{index}] from CSV row: {csvRow}.");
        }
      }
      output.PaperType = PrintJobDetails.defaultPaperType;
      output.PaperSize = PrintJobDetails.defaultPaperSize;
      return output;
    }
  }
}
