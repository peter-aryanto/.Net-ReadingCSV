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

    public Type PaperType;
    public string PaperSize { get; set; }
    public int TotalNumberOfPages { get; set; }
    public int NumberOfBlackAndWhitePages { get; set; }
    public int NumberOfColourPages { get; set; }
    public bool IsDoubleSided { get; set; }
  }
}
