using System;
using System.Collections.Generic;
using Xunit;
using Moq;
using PrintCost.DomainObjects;

namespace UnitTest.DomainObjects
{
  public class PrintJobDetailsTests
  {
    [Fact]
    public void ToString_MustReturnCompleteDetails()
    {
      var printPaper = new Mock<IPrintPaper>();
      const string printPaperInfo = "Mock Paper.";
      printPaper.Setup(x => x.GetInfo()).Returns(printPaperInfo);
      var testObject = new PrintJobDetails
      {
        PrintJobParts = new List<PrintJobPart>
        {
          new PrintJobPart
          {
            NumberOfPages = 2,
            PrintPaper = printPaper.Object,
            CalculatedCostInCents = 20,
          },
          new PrintJobPart
          {
            NumberOfPages = 3,
            PrintPaper = printPaper.Object,
            CalculatedCostInCents = 30,
          },
        },
        CalculatedCostInCents = 50,
      };

      string completeDetails = $"2 x {printPaperInfo} -> Cost = 20."
        + Environment.NewLine + $"3 x {printPaperInfo} -> Cost = 30."
        + Environment.NewLine + "Subtotal = 50.";
      Assert.Equal(completeDetails, testObject.ToString());
    }
  }

  public class PrintJobPartTests
  {
    [Fact]
    public void ToString_MustReturnCompleteDetails()
    {
      var printPaper = new Mock<IPrintPaper>();
      const string printPaperInfo = "Print Paper.";
      printPaper.Setup(x => x.GetInfo()).Returns(printPaperInfo);

      var testObject = new PrintJobPart
      {
        NumberOfPages = 3,
        PrintPaper = printPaper.Object,
        CalculatedCostInCents = 30,
      };

      Assert.Equal(
        $"3 x {printPaperInfo} -> Cost = 30.",
        testObject.ToString()
      );
    }
  }
}
