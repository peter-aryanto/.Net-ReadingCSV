using System;
using System.Collections.Generic;
using Xunit;
using PrintCost.DomainObjects;

namespace UnitTest.DomainObjects
{
  public class CopyPaperTests
  {
    [Fact]
    public void GetInfo_ReturnsInfo()
    {
      var testObject = new CopyPaper
      {
        Size = "A4",
        IsColor = false,
        IsDoubleSided = false,
      };

      Assert.Equal(
        "A4 copy paper black and white single sided.",
        testObject.GetInfo()
      );

      testObject.CostInCents = 15;
      Assert.Equal(
        "A4 copy paper black and white single sided [15 cent(s)].",
        testObject.GetInfo()
      );
    }

    [Fact]
    public void GetCostInCents_ReturnsCost()
    {
      var testObject = new CopyPaper
      {
        Size = "A4",
        IsColor = false,
        IsDoubleSided = false,
      };

      Assert.Equal(0, testObject.GetCostInCents());

      decimal cost = 15;
      testObject.CostInCents = cost;
      Assert.Equal(cost, testObject.GetCostInCents());
    }
  }
}
