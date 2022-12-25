using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using PrintCost.DomainObjects;
using PrintCost.BusinessLogics;

namespace UnitTest.BusinessLogics
{
  public class PrintCostCalculatorTests
  {
    [Fact]
    public void FindCostInCentsPerCopyPaperSheet_WhenItemExists_ThenCanFindCost()
    {
      var a4Basic = new CopyPaper
      {
        Size = "A4",
        IsColor = false,
        IsDoubleSided = false,
        CostInCents = 15,
      };
      var a4DoubleSided = new CopyPaper
      {
        Size = "A4",
        IsColor = false,
        IsDoubleSided = true,
        CostInCents = 10,
      };
      var copyPapers = new List<CopyPaper>
      {
        a4Basic,
        a4DoubleSided,
      };

      var testObject = new PrintCostCalculator();
      decimal? output;

      output = testObject.FindCostInCentsPerCopyPaperSheet(
        copyPapers,
        "A4", false, false
      );
      Assert.Equal(a4Basic.CostInCents, output);

      output = testObject.FindCostInCentsPerCopyPaperSheet(
        copyPapers,
        "A4", false, true
      );
      Assert.Equal(a4DoubleSided.CostInCents, output);

      output = testObject.FindCostInCentsPerCopyPaperSheet(
        copyPapers,
        "A3", false, true
      );
      Assert.Null(output);
    }
  }
}
