using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Moq;
using PrintCost.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using PrintCost.BusinessLogics;
using PrintCost.DomainObjects;
using PrintCost.Helpers;

namespace UnitTest.Controllers
{
  public class PrintCostControllerTests
  {
    private readonly PrintCostController _testObject;

    private readonly Mock<IPrintJobDetailsReader> _printJobDetailsReader;
    private readonly Mock<IPrintCostCalculator> _printCostCalculator;
    private readonly Mock<IOutputWriter> _outputWriter;

    private readonly PrintJobDetails _dummyPrintJobDetails;
    private readonly string _dummyPrintJobDetailsString;

    public PrintCostControllerTests()
    {
      _printJobDetailsReader = new Mock<IPrintJobDetailsReader>();
      const decimal dummyJobPartCostInCents = 100;
      var dummyPrintJobParts =
        new List<PrintJobPart>
        {
          new PrintJobPart
          {
            NumberOfPages = 1,
            PrintPaper = new CopyPaper
            {
              IsColor = false,
            },
            CalculatedCostInCents = dummyJobPartCostInCents,
          },
          new PrintJobPart
          {
            NumberOfPages = 2,
            PrintPaper = new CopyPaper
            {
              IsColor = true,
            },
            CalculatedCostInCents = dummyJobPartCostInCents,
          },
        };
      _dummyPrintJobDetails = new PrintJobDetails
      {
        PrintJobParts = dummyPrintJobParts,
      };
      _dummyPrintJobDetails.CalculatedCostInCents =
        dummyPrintJobParts.Count * dummyJobPartCostInCents;
      _dummyPrintJobDetailsString = _dummyPrintJobDetails.ToString();
      _printJobDetailsReader
        .Setup(x => x.ReadPrintJobDetailsCsvRow(It.Is<string>(s => !string.IsNullOrWhiteSpace(s))))
        .Returns(_dummyPrintJobDetails)
        .Verifiable();
      _printJobDetailsReader
        .Setup(x => x.ReadPrintJobDetailsCsvRow(It.Is<string>(s => string.IsNullOrWhiteSpace(s))))
        .Returns<PrintJobDetails>(null)
        .Verifiable();

      _printCostCalculator = new Mock<IPrintCostCalculator>();
      _printCostCalculator
        .Setup(x => x.CalculateCostInCents(It.IsAny<int>(), It.IsAny<IPrintPaper>()))
        .Returns(dummyJobPartCostInCents)
        .Verifiable();

      _outputWriter = new Mock<IOutputWriter>();

      _testObject = new PrintCostController(
        _printJobDetailsReader.Object,
        _printCostCalculator.Object,
        _outputWriter.Object
      );
    }

    private IFormFile SetupPrintJobDetailsFile(List<string> csvRows)
    {
      var fileContent = "Headers";
      foreach (var csvRow in csvRows)
      {
        fileContent += Environment.NewLine + csvRow;
      }
      var fileName = "sample.csv";
      var memoryStream = new MemoryStream();
      var writer = new StreamWriter(memoryStream);
      writer.Write(fileContent);
      writer.Flush();
      memoryStream.Position = 0;
      IFormFile printJobDetailsFile = new FormFile(
        memoryStream,
        0,
        memoryStream.Length,
        PrintCostController.PrintJobDetailsFileFormFieldName,
        fileName
      );
      return printJobDetailsFile;
    }

    [Fact]
    public void PrintCostDetails_WhenNoFileProvided_ThenReturnsBadRequest()
    {
      var files = new Mock<IFormFileCollection>();
      files.Setup(x => x.GetFile(PrintCostController.PrintJobDetailsFileFormFieldName))
        .Returns<IFormFile>(null);

      var formData = new Mock<IFormCollection>();
      formData.Setup(x => x.Files).Returns(files.Object);

      var output = _testObject.PrintCostDetails(formData.Object);
      Assert.IsType<BadRequestObjectResult>(output);
      Assert.Contains(
        PrintCostController.ErrorPrintJobDetailsFile,
        ((BadRequestObjectResult)output).Value.ToString()
      );
    }

    [Fact]
    public void PrintCostDetails_WhenFileIsProvided_ThenCallsPrintJobDetailsReader()
    {
      var contentRow1 = "25, 10, false";
      var contentRow2 = "55, 13, true";
      var printJobDetailsFile = SetupPrintJobDetailsFile(
        new List<string>
        {
          contentRow1,
          contentRow2,
        }
      );
      var files = new Mock<IFormFileCollection>();
      files.Setup(x => x.GetFile(PrintCostController.PrintJobDetailsFileFormFieldName))
        .Returns(printJobDetailsFile);

      var formData = new Mock<IFormCollection>();
      formData.Setup(x => x.Files).Returns(files.Object);

      var output = _testObject.PrintCostDetails(formData.Object);

      _printJobDetailsReader.Verify(
        x => x.ReadPrintJobDetailsCsvRow(It.IsAny<string>()),
        Times.Exactly(2)
      );
      _printJobDetailsReader.Verify(x => x.ReadPrintJobDetailsCsvRow(contentRow1), Times.Once);
      _printJobDetailsReader.Verify(x => x.ReadPrintJobDetailsCsvRow(contentRow2), Times.Once);
    }

    [Fact]
    public void PrintCostDetails_WhenFileIsProvided_ThenCallsPrintCostCalculator()
    {
      var contentRow1 = "25, 10, false";
      var emptyRow2 = string.Empty;
      var contentRow3 = "55, 13, true";
      var csvRows =
        new List<string>
        {
          contentRow1,
          emptyRow2,
          contentRow3,
        };
      var printJobDetailsFileWith2ContentRowsAnd1EmptyRow = SetupPrintJobDetailsFile(csvRows);
      var files = new Mock<IFormFileCollection>();
      files.Setup(x => x.GetFile(PrintCostController.PrintJobDetailsFileFormFieldName))
        .Returns(printJobDetailsFileWith2ContentRowsAnd1EmptyRow);

      var formData = new Mock<IFormCollection>();
      formData.Setup(x => x.Files).Returns(files.Object);

      var output = _testObject.PrintCostDetails(formData.Object);

      _printCostCalculator.Verify(
        x => x.CalculateCostInCents(_dummyPrintJobDetails.PrintJobParts[0].NumberOfPages, It.Is<IPrintPaper>(p => !((CopyPaper)p).IsColor)),
        Times.Exactly(2)
      );
      _printCostCalculator.Verify(
        x => x.CalculateCostInCents(_dummyPrintJobDetails.PrintJobParts[1].NumberOfPages, It.Is<IPrintPaper>(p => ((CopyPaper)p).IsColor)),
        Times.Exactly(2)
      );
    }

    [Fact]
    public void PrintCostDetails_WhenFileIsProvided_ThenCallsOutputWriter()
    {
      var contentRow1 = "25, 10, false";
      var emptyRow2 = string.Empty;
      var contentRow3 = "55, 13, true";
      var printJobDetailsFileWith2ContentRowsAnd1EmptyRow = SetupPrintJobDetailsFile(
        new List<string>
        {
          contentRow1,
          emptyRow2,
          contentRow3,
        }
      );
      var files = new Mock<IFormFileCollection>();
      files.Setup(x => x.GetFile(PrintCostController.PrintJobDetailsFileFormFieldName))
        .Returns(printJobDetailsFileWith2ContentRowsAnd1EmptyRow);

      var formData = new Mock<IFormCollection>();
      formData.Setup(x => x.Files).Returns(files.Object);

      var output = _testObject.PrintCostDetails(formData.Object);

      _outputWriter.Verify(x => x.ConsoleWriteLine(_dummyPrintJobDetailsString), Times.Exactly(2));
      const string expectedTotalCostInCentsInfo = "Total Cost of All Jobs in Cents = 400.";
      _outputWriter.Verify(x => x.ConsoleWriteLine(expectedTotalCostInCentsInfo), Times.Once);
      _outputWriter.Verify(x => x.ConsoleWriteLine(PrintCostController.EndingLine), Times.Once);
    }

    [Fact]
    public void PrintCostDetails_WhenFileIsProvided_ThenReturnsOk()
    {
      var contentRow1 = "25, 10, false";
      var contentRow2 = "55, 13, true";
      var printJobDetailsFile = SetupPrintJobDetailsFile(
        new List<string>
        {
          contentRow1,
          contentRow2,
        }
      );
      var files = new Mock<IFormFileCollection>();
      files.Setup(x => x.GetFile(PrintCostController.PrintJobDetailsFileFormFieldName))
        .Returns(printJobDetailsFile);

      var formData = new Mock<IFormCollection>();
      formData.Setup(x => x.Files).Returns(files.Object);

      var output = _testObject.PrintCostDetails(formData.Object);
      Assert.IsType<OkObjectResult>(output);
      var outputJsonString = JsonConvert.SerializeObject(((OkObjectResult)output).Value);
      JObject outputJObject = JObject.Parse(outputJsonString);
      JArray csvRowJArray = (JArray)outputJObject["CsvRows"];
      List<string> csvRows = csvRowJArray.ToObject<List<string>>();
      Assert.Equal(2, csvRows.Count);
      Assert.Equal(contentRow1, csvRows[0]);
      Assert.Equal(contentRow2, csvRows[1]);
    }
  }
}
