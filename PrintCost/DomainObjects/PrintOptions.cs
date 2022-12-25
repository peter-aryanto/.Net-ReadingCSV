using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrintCost.DomainObjects
{
  public class PrintOptions
  {
    public List<CopyPaper> CopyPapers { get; set; }
  }

  public class CopyPaper
  {
    public string Size { get; set; }
    public bool IsColor { get; set; }
    public bool IsDoubleSided { get; set; }
    public decimal CostInCents { get; set; }
  }
}
