using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Moq;
using PrintCost.DomainObjects;
using PrintCost.BusinessLogics;
using UnitTest.UnitTestHelpers;

namespace UnitTest.BusinessLogics
{
  public class PrintCostCalculatorTests
  {
    [Fact]
    public void FindCostInCentsPerCopyPaperPage_WhenNotFound_ThenThrowsException()
    {
      var a4Basic = new CopyPaper
      {
        Size = "A4",
        IsColor = false,
        IsDoubleSided = false,
        CostInCents = 15,
      };
      var copyPapers = new List<CopyPaper>
      {
        a4Basic,
      };
      var printOptions = new PrintOptions
      {
        CopyPapers = copyPapers,
      };

      var testObject = PrintCostCalculatorHelper.SetupTestObject(printOptions);

      decimal output;

      try
      {
        output = testObject.FindCostInCentsPerCopyPaperPage(
          a4Basic.Size,
          a4Basic.IsColor,
          !a4Basic.IsDoubleSided
        );
      }
      catch (Exception e)
      {
        Assert.Equal(
          "Cannot calculate cost for: A4 copy paper black and white double sided.",
          e.Message
        );
      }
    }

    [Fact]
    public void FindCostInCentsPerCopyPaperPage_WhenFound_ThenReturnsCost()
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
      //var printOptionsSetup = new Mock<IOptions<PrintOptions>>();
      //printOptionsSetup.Setup(x => x.Value).Returns(printOptions);
      //var testObject = new PrintCostCalculator(printOptionsSetup.Object);
      var testObject = PrintCostCalculatorHelper.SetupTestObject(printOptions);

      decimal output;

      output = testObject.FindCostInCentsPerCopyPaperPage("A4", false, false);
      Assert.Equal(a4Basic.CostInCents, output);

      output = testObject.FindCostInCentsPerCopyPaperPage("A4", false, true);
      Assert.Equal(a4DoubleSided.CostInCents, output);
    }

    private class DummyPaper : IPrintPaper
    {
      public decimal GetCostInCents()
      {
        return 0;
      }

      public string GetInfo()
      {
        return "Dummy paper.";
      }
    }

    [Fact]
    public void CalculateCostInCents_WhenForPaperOtherThanCopyPaper_ThenThrowsException()
    {
      var testObject = PrintCostCalculatorHelper.SetupTestObject(new PrintOptions());

      decimal? output;

      try
      {
        output = testObject.CalculateCostInCents(1, new DummyPaper());
      }
      catch (Exception e)
      {
        Assert.Equal(
          "Cannot calculate cost for: Dummy paper.",
          e.Message
        );
      }
    }

    [Fact]
    public void CalculateCostInCents_WhenForAvailableCopyPaperOption_ThenReturnsCost()
    {
      var a4Basic = new CopyPaper
      {
        Size = "A4",
        IsColor = false,
        IsDoubleSided = false,
        CostInCents = 15,
      };
      var copyPapers = new List<CopyPaper>
      {
        a4Basic,
      };
      var printOptions = new PrintOptions
      {
        CopyPapers = copyPapers,
      };

      var testObject = PrintCostCalculatorHelper.SetupTestObject(printOptions);

      CopyPaper input =
        new CopyPaper
        {
          Size = a4Basic.Size,
          IsColor = a4Basic.IsColor,
          IsDoubleSided = a4Basic.IsDoubleSided,
        };
      decimal? output;

      output = testObject.CalculateCostInCents(2, input);
      Assert.Equal(2 * a4Basic.CostInCents, output);
      Assert.Equal(a4Basic.CostInCents, input.CostInCents);
    }
  }
}
