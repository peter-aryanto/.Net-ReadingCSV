using System;
using Xunit;
using PrintCost.DomainObjects;
using PrintCost.BusinessLogics;

namespace UnitTest.BusinessLogics
{
  public class PrintJobDetailsReaderTests
  {
    [Fact]
    public void ReadPrintJobDetailsCsvRow_WhenRowIsEmpty_ThenReturnsNull()
    {
      var testObject = new PrintJobDetailsReader();

      PrintJobDetails output;

      output = testObject.ReadPrintJobDetailsCsvRow(null);
      Assert.Null(output);

      output = testObject.ReadPrintJobDetailsCsvRow(" ");
      Assert.Null(output);
    }

    [Fact]
    public void ReadPrintJobDetailsCsvRow_WhenExpectedColumnsDoNotMatch_ThenThrowsException()
    {
      var testObject = new PrintJobDetailsReader();

      PrintJobDetails output;

      string commaDelimitedCsvColumns = string.Join(", ", PrintJobDetails.CsvColumns);

      try
      {
        string csvRowWithMissingColumn = "55, 13";
        output = testObject.ReadPrintJobDetailsCsvRow(csvRowWithMissingColumn);
      }
      catch (Exception e)
      {
        Assert.Contains(commaDelimitedCsvColumns, e.Message);
      }

      try
      {
        string csvRowWithOneTooManyColumn = "55, 13, true, true";
        output = testObject.ReadPrintJobDetailsCsvRow(csvRowWithOneTooManyColumn);
      }
      catch (Exception e)
      {
        Assert.Contains(commaDelimitedCsvColumns, e.Message);
      }
    }

    [Fact]
    public void ReadPrintJobDetailsCsvRow_WhenColumnValueTypeInvalid_ThenThrowsException()
    {
      var testObject = new PrintJobDetailsReader();

      string csvRowWithInvalidLastColumnValueType = "25, 10, yes";
      Assert.ThrowsAny<Exception>(() =>
        testObject.ReadPrintJobDetailsCsvRow(csvRowWithInvalidLastColumnValueType));
    }

    [Fact]
    public void ReadPrintJobDetailsCsvRow_WhenNumberOfPagesInvalid_ThenThrowsException()
    {
      var testObject = new PrintJobDetailsReader();

      PrintJobDetails output;

      string csvRowWithNoColourPage = "1, 0, false";
      output = testObject.ReadPrintJobDetailsCsvRow(csvRowWithNoColourPage);
      // No exception/error this far.

      string csvRowWithAllColourPages = "1, 1, false";
      output = testObject.ReadPrintJobDetailsCsvRow(csvRowWithAllColourPages);
      // No exception/error this far.

      try
      {
        string csvRowWithColourPagesMoreThanTotalOfAllPages = "1, 2, false";
        output = testObject.ReadPrintJobDetailsCsvRow(csvRowWithColourPagesMoreThanTotalOfAllPages);
      }
      catch (Exception e)
      {
        string expectedMessage = "Number of colour pages (2) "
          + "should not be more than total number of pages (1).";
        Assert.Equal(expectedMessage, e.Message);
      }

      try
      {
        string csvRowWithColourPagesMoreThanTotalOfAllPages = "0, 0, false";
        output = testObject.ReadPrintJobDetailsCsvRow(csvRowWithColourPagesMoreThanTotalOfAllPages);
      }
      catch (Exception e)
      {
        string expectedMessage = "Invalid total number of pages (0).";
        Assert.Equal(expectedMessage, e.Message);
      }

      try
      {
        string csvRowWithColourPagesMoreThanTotalOfAllPages = "1, -1, false";
        output = testObject.ReadPrintJobDetailsCsvRow(csvRowWithColourPagesMoreThanTotalOfAllPages);
      }
      catch (Exception e)
      {
        string expectedMessage = "Invalid number of colour pages (-1).";
        Assert.Equal(expectedMessage, e.Message);
      }
    }

    [Fact]
    public void ReadPrintJobDetailsCsvRow_WhenAllColumnsValid_ThenReturnsPrintJobDetails()
    {
      var testObject = new PrintJobDetailsReader();

      PrintJobDetails output;
      const string commaAndSpaceDelimiter = ", ";
      const string commaOnlyDelimiter = ",";
      string validCsvRow = $"55{commaAndSpaceDelimiter}13{commaOnlyDelimiter}tRUe";
      output = testObject.ReadPrintJobDetailsCsvRow(validCsvRow);

      Assert.Equal(2, output.PrintJobParts.Count);

      var jobPart = output.PrintJobParts[0];
      var numberOfPages = jobPart.NumberOfPages;
      Assert.Equal(55 - 13, numberOfPages);
      Assert.Equal(typeof(CopyPaper).Name, jobPart.PrintPaper.GetType().Name);
      var copyPaper = (CopyPaper)jobPart.PrintPaper;
      Assert.Equal("A4", copyPaper.Size);
      Assert.False(copyPaper.IsColor);
      Assert.True(copyPaper.IsDoubleSided);

      jobPart = output.PrintJobParts[1];
      numberOfPages = jobPart.NumberOfPages;
      Assert.Equal(13, numberOfPages);
      Assert.Equal(typeof(CopyPaper).Name, jobPart.PrintPaper.GetType().Name);
      copyPaper = (CopyPaper)jobPart.PrintPaper;
      Assert.Equal("A4", copyPaper.Size);
      Assert.True(copyPaper.IsColor);
      Assert.True(copyPaper.IsDoubleSided);
    }
  }
}