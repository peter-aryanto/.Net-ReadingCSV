using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrintCost.DomainObjects
{
  public class PrintOptions
  {
    public PrintOptions()
    {
      CopyPapers = new List<CopyPaper>();
    }

    public List<CopyPaper> CopyPapers { get; set; }
  }

  public interface IPrintPaper
  {
    public string GetInfo();
    public decimal GetCostInCents();
  }

  public class CopyPaper : IPrintPaper
  {
    public string Size { get; set; }
    public bool IsColor { get; set; }
    public bool IsDoubleSided { get; set; }
    public decimal CostInCents { get; set; }

    public string GetInfo()
    {
      string colourInfo =
        IsColor ? "colour" : "black and white";
      string doubleSidedInfo =
        IsDoubleSided ? "double sided" : "single sided";
      string costInCentsInfo = string.Empty;
      if (CostInCents > 0)
      {
        costInCentsInfo = $" [{CostInCents} cent(s)]";
      }
      return $"{Size} copy paper {colourInfo} {doubleSidedInfo}{costInCentsInfo}.";
    }

    public decimal GetCostInCents()
    {
      return CostInCents;
    }
  }
}
