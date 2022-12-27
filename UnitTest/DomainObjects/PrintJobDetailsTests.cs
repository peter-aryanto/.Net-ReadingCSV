using Xunit;
using PrintCost.DomainObjects;

namespace UnitTest.DomainObjects
{
  public class PrintJobDetailsTests
  {
    [Fact]
    public void ToString_MustPresentCorrectDetails()
    {
      var testObject = new PrintJobDetails
      {
        PaperType = typeof(CopyPaper),
        PaperSize = "A4",
        NumberOfBlackAndWhitePages = 1,
        NumberOfColourPages = 2,
        IsDoubleSided = true
      };

      Assert.Equal(
        "Print job: A4 copy paper double sided; 1 black and white page(s); 2 colour page(s).",
        testObject.ToString()
      );

      testObject.IsDoubleSided = false;
      Assert.Equal(
        "Print job: A4 copy paper single sided; 1 black and white page(s); 2 colour page(s).",
        testObject.ToString()
      );
    }
  }
}