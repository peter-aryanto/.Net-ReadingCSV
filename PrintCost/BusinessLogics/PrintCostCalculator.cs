using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PrintCost.DomainObjects;

namespace PrintCost.BusinessLogics
{
  public interface IPrintCostCalculator
  {
    public decimal? FindCostInCentsPerCopyPaperSheet(
      List<CopyPaper> copyPapers,
      string size,
      bool isColor,
      bool isDoubleSided
    );
  }

  public class PrintCostCalculator : IPrintCostCalculator
  {
    public decimal? FindCostInCentsPerCopyPaperSheet(
      List<CopyPaper> copyPapers,
      string size,
      bool isColor,
      bool isDoubleSided
    )
    {
      decimal? output = null;

      foreach (var paper in copyPapers)
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
