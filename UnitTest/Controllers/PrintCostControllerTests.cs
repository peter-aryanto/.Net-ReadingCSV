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

namespace UnitTest.Controllers
{
  public class PrintCostControllerTests
  {
    [Fact]
    public void PrintCostDetails_WhenNoFileProvided_ThenReturnsBadRequest()
    {
      var testObject = new PrintCostController();

      var files = new Mock<IFormFileCollection>();
      files.Setup(x => x.GetFile(PrintCostController.PrintJobDetailsFileFormFieldName))
        .Returns<IFormFile>(null);

      var formData = new Mock<IFormCollection>();
      formData.Setup(x => x.Files).Returns(files.Object);

      var output = testObject.PrintCostDetails(formData.Object);
      Assert.IsType<BadRequestObjectResult>(output);
      Assert.Contains(
        PrintCostController.ErrorPrintJobDetailsFile,
        ((BadRequestObjectResult)output).Value.ToString()
      );
    }

    [Fact]
    public void PrintCostDetails_WhenFileIsProvided_ThenReturnsOk()
    {
      var testObject = new PrintCostController();

      var contentRow1 = "25, 10, false";
      var contentRow2 = "55, 13, true";
      var fileContent = "Headers"
        + Environment.NewLine + contentRow1
        + Environment.NewLine + contentRow2;
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
      var files = new Mock<IFormFileCollection>();
      files.Setup(x => x.GetFile(PrintCostController.PrintJobDetailsFileFormFieldName))
        .Returns(printJobDetailsFile);

      var formData = new Mock<IFormCollection>();
      formData.Setup(x => x.Files).Returns(files.Object);

      var output = testObject.PrintCostDetails(formData.Object);
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
