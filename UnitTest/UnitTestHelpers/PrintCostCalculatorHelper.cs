using Moq;
using Microsoft.Extensions.Options;
using PrintCost.DomainObjects;
using PrintCost.BusinessLogics;

namespace UnitTest.UnitTestHelpers
{
  public class PrintCostCalculatorHelper
  {
    public static PrintCostCalculator SetupTestObject(PrintOptions printOptions)
    {
      var printOptionsSetup = new Mock<IOptions<PrintOptions>>();
      printOptionsSetup.Setup(x => x.Value).Returns(printOptions);
      var testObject = new PrintCostCalculator(printOptionsSetup.Object);
      return testObject;
    }
  }
}