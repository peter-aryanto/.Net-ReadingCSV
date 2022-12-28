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
      int totalNumberOfPages = 0;
      int numberOfBlackAndWhitePages = 0;
      int numberOfColourPages = 0;
      bool isDoubleSided = false;
      for (int index = 0; index < expectedColumnCount; index++)
      {
        string csvColumnValue = inputCsvColumns[index].Trim();
        switch (index)
        {
          case 0:
            totalNumberOfPages = int.Parse(csvColumnValue);
            if (totalNumberOfPages <= 0)
            {
              throw new Exception($"Invalid total number of pages ({totalNumberOfPages}).");
            }

            break;
          case 1:
            numberOfColourPages = int.Parse(csvColumnValue);
            if (numberOfColourPages < 0)
            {
              throw new Exception($"Invalid number of colour pages ({numberOfColourPages}).");
            }
            if (numberOfColourPages > totalNumberOfPages)
            {
              throw new Exception($"Number of colour pages ({numberOfColourPages}) "
                + $"should not be more than total number of pages ({totalNumberOfPages}).");
            }

            numberOfBlackAndWhitePages = totalNumberOfPages - numberOfColourPages;

            break;
          case 2:
            isDoubleSided = bool.Parse(csvColumnValue);

            break;
          default:
            throw new Exception($"Invalid column[{index}] from CSV row: {csvRow}.");
        }
      }

      if (numberOfBlackAndWhitePages > 0)
      {
        output.PrintJobParts.Add(
          new PrintJobPart
          {
            NumberOfPages = numberOfBlackAndWhitePages,
            PrintPaper =
              new CopyPaper
              {
                Size = PrintJobDetails.defaultPaperSize,
                IsColor = false,
                IsDoubleSided = isDoubleSided,
              },
          }
        );
      }

      if (numberOfColourPages > 0)
      {
        output.PrintJobParts.Add(
          new PrintJobPart
          {
            NumberOfPages = numberOfColourPages,
            PrintPaper =
              new CopyPaper
              {
                Size = PrintJobDetails.defaultPaperSize,
                IsColor = true,
                IsDoubleSided = isDoubleSided,
              },
          }
        );
      }
      //
      return output;
    }
  }
}
