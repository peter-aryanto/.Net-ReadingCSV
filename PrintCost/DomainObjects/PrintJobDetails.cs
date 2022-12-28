using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrintCost.DomainObjects
{
  public class PrintJobDetails
  {
    public static readonly Type defaultPaperType = typeof(CopyPaper);

    public const string defaultPaperSize = "A4";

    public static readonly string[] CsvColumns =
    {
      "Total Number of Pages",
      "Number of Colour Pages",
      "Is Double Sided",
    };

    public PrintJobDetails()
    {
      PrintJobParts = new List<PrintJobPart>();
    }

    public List<PrintJobPart> PrintJobParts { get; set; }
    public decimal? CalculatedCostInCents { get; set; }

    public override string ToString()
    {
      string output = string.Empty;
      foreach (var part in PrintJobParts)
      {
        if (!string.IsNullOrEmpty(output))
        {
          output += Environment.NewLine;
        }
        output += part.ToString();
      }
      output += Environment.NewLine + $"Subtotal = {CalculatedCostInCents}.";
      return output;
    }
  }

  public class PrintJobPart
  {
    public int NumberOfPages { get; set; }
    public IPrintPaper PrintPaper { get; set; }
    public decimal? CalculatedCostInCents { get; set; }

    //public decimal GetCostInCents()

    public override string ToString()
    {
      return $"{NumberOfPages} x {PrintPaper.GetInfo()} -> Cost = {CalculatedCostInCents}.";
    }
  }
}
