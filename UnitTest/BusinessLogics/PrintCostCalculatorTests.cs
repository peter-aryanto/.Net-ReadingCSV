using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Moq;
using PrintCost.DomainObjects;
using PrintCost.BusinessLogics;
using Microsoft.Extensions.Options;

namespace UnitTest.BusinessLogics
{
  public class PrintCostCalculatorTests
  {
    [Fact]
    public void FindCostInCentsPerCopyPaperSheet_WhenExists_ThenCanFindCost()
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
      var printOptions = new PrintOptions
      {
        CopyPapers = copyPapers,
      };

      //IOptions<PrintOptions> printOptionsSetup = Options.Create<PrintOptions>(printOptions);
      //var testObject = new PrintCostCalculator(printOptionsSetup);
      var printOptionsSetup = new Mock<IOptions<PrintOptions>>();
      printOptionsSetup.Setup(x => x.Value).Returns(printOptions);
      var testObject = new PrintCostCalculator(printOptionsSetup.Object);

      decimal? output;

      output = testObject.FindCostInCentsPerCopyPaperSheet("A4", false, false);
      Assert.Equal(a4Basic.CostInCents, output);

      output = testObject.FindCostInCentsPerCopyPaperSheet("A4", false, true);
      Assert.Equal(a4DoubleSided.CostInCents, output);

      output = testObject.FindCostInCentsPerCopyPaperSheet("A3", false, true);
      Assert.Null(output);
    }
  }
}
