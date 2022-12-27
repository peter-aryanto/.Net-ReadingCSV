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
      Assert.Equal(1, output.NumberOfBlackAndWhitePages);

      string csvRowWithAllColourPages = "1, 1, false";
      output = testObject.ReadPrintJobDetailsCsvRow(csvRowWithAllColourPages);
      Assert.Equal(0, output.NumberOfBlackAndWhitePages);

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

      string validCsvRow = "55,13,tRUe";
      output = testObject.ReadPrintJobDetailsCsvRow(validCsvRow);
      Assert.Equal(typeof(CopyPaper).Name, output.PaperType.Name);
      Assert.Equal("A4", output.PaperSize);
      Assert.Equal(55, output.TotalNumberOfPages);
      Assert.Equal(55 - 13, output.NumberOfBlackAndWhitePages);
      Assert.Equal(13, output.NumberOfColourPages);
      Assert.True(output.IsDoubleSided);

      validCsvRow = validCsvRow.Replace(",", ", ");
      output = testObject.ReadPrintJobDetailsCsvRow(validCsvRow);
      Assert.Equal(typeof(CopyPaper).Name, output.PaperType.Name);
      Assert.Equal("A4", output.PaperSize);
      Assert.Equal(55, output.TotalNumberOfPages);
      Assert.Equal(55 - 13, output.NumberOfBlackAndWhitePages);
      Assert.Equal(13, output.NumberOfColourPages);
      Assert.True(output.IsDoubleSided);
    }
  }
}