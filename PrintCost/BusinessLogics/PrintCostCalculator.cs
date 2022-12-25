using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using PrintCost.DomainObjects;

namespace PrintCost.BusinessLogics
{
  public interface IPrintCostCalculator
  {
    public decimal? FindCostInCentsPerCopyPaperSheet(
      string size,
      bool isColor,
      bool isDoubleSided
    );
  }

  public class PrintCostCalculator : IPrintCostCalculator
  {
    private readonly PrintOptions _printOptions;

    public PrintCostCalculator(IOptions<PrintOptions> printOptions)
    {
      _printOptions = printOptions.Value;
    }

    public decimal? FindCostInCentsPerCopyPaperSheet(
      string size,
      bool isColor,
      bool isDoubleSided
    )
    {
      decimal? output = null;

      foreach (var paper in _printOptions.CopyPapers)
      {
        if (
          paper.Size == size
          && paper.IsColor == isColor
          && paper.IsDoubleSided == isDoubleSided
        )
        {
          output = paper.CostInCents;
          break;
        }
      }

      return output;
    }
  }
}
