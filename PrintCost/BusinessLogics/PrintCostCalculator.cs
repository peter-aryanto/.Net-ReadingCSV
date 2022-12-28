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
    public decimal FindCostInCentsPerCopyPaperPage(
      string size,
      bool isColor,
      bool isDoubleSided
    );
    public decimal? CalculateCostInCents(int numberOfPages, IPrintPaper printPaper);
  }

  public class PrintCostCalculator : IPrintCostCalculator
  {
    private readonly PrintOptions _printOptions;

    public PrintCostCalculator(IOptions<PrintOptions> printOptions)
    {
      _printOptions = printOptions.Value;
    }

    public decimal FindCostInCentsPerCopyPaperPage(
      string size,
      bool isColor,
      bool isDoubleSided
    )
    {
      foreach (var paper in _printOptions.CopyPapers)
      {
        if (
          paper.Size == size
          && paper.IsColor == isColor
          && paper.IsDoubleSided == isDoubleSided
        )
        {
          return paper.CostInCents;
        }
      }

      throw new Exception(GenerateErrorMessageFromCalculation(
        new CopyPaper
        {
          Size = size,
          IsColor = isColor,
          IsDoubleSided = isDoubleSided,
        }
      ));
    }

    private string GenerateErrorMessageFromCalculation(IPrintPaper printPaper)
    {
      return $"Cannot calculate cost for: {printPaper.GetInfo()}";
    }

    public decimal? CalculateCostInCents(int numberOfPages, IPrintPaper printPaper)
    {
      if (printPaper.GetType().Name == typeof(CopyPaper).Name)
      {
        var copyPaper = (CopyPaper)printPaper;
        var costInCents = FindCostInCentsPerCopyPaperPage(
          copyPaper.Size,
          copyPaper.IsColor,
          copyPaper.IsDoubleSided
        );
        copyPaper.CostInCents = costInCents;
        return numberOfPages * costInCents;
      }
      else
      {
        throw new Exception(GenerateErrorMessageFromCalculation(printPaper));
      }
    }
  }
}
