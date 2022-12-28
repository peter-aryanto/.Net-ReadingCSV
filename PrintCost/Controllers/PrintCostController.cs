using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using PrintCost.BusinessLogics;
using PrintCost.DomainObjects;
using PrintCost.Helpers;

namespace PrintCost.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class PrintCostController : ControllerBase
  {
    public static readonly string PrintJobDetailsFileFormFieldName = "printJobDetails";
    public static readonly string ErrorPrintJobDetailsFile =
      "Please provide the CSV file containing the print job details.";
    public static readonly string ErrorCannotReadPrintJobDetailsFile =
      "Cannot read print job details from the provided file. " + ErrorPrintJobDetailsFile;

    public const string EndingLine = "=======================================";

    private readonly IPrintJobDetailsReader _printJobDetailsReader;
    private readonly IPrintCostCalculator _printCostCalculator;
    private readonly IOutputWriter _outputWriter;

    public PrintCostController(
      IPrintJobDetailsReader printJobDetailsReader,
      IPrintCostCalculator printCostCalculator,
      IOutputWriter outputWriter
    )
    {
      _printJobDetailsReader = printJobDetailsReader;
      _printCostCalculator = printCostCalculator;
      _outputWriter = outputWriter;
    }

    // curl -i -F printJobDetails=@sample.csv -F additionalInput1=Hello http://localhost:5000/api/PrintCost/PrintCostDetails
    [HttpPost("PrintCostDetails")]
    public IActionResult PrintCostDetails([FromForm]IFormCollection formData)
    {
      var printJobDetailsFile = formData.Files.GetFile(PrintJobDetailsFileFormFieldName);
      if (printJobDetailsFile == null)
      {
        return BadRequest(new { Error = ErrorPrintJobDetailsFile });
      }

      List<string> printJobDetailsCsvRows = new List<string>();
      try
      {
        using (var reader = new StreamReader(printJobDetailsFile.OpenReadStream()))
        {
          if (reader.Peek() >= 0)
          {
            // Skipping the 1st row, which is the header.
            reader.ReadLine();
          }

          while (reader.Peek() >= 0)
          {
            string row = reader.ReadLine();
            printJobDetailsCsvRows.Add(row);
          }
        }

        var printJobList = new List<PrintJobDetails>();
        foreach (var csvRow in printJobDetailsCsvRows)
        {
          var printJobDetails = _printJobDetailsReader.ReadPrintJobDetailsCsvRow(csvRow);
          if (printJobDetails != null)
          {
            printJobList.Add(printJobDetails);
          }
        }

        decimal totalCostInCents = 0;
        foreach (var printJobDetails in printJobList)
        {
          printJobDetails.CalculatedCostInCents = 0;
          foreach (var part in printJobDetails.PrintJobParts)
          {
            part.CalculatedCostInCents = _printCostCalculator.CalculateCostInCents(
              part.NumberOfPages,
              part.PrintPaper
            );
            printJobDetails.CalculatedCostInCents += part.CalculatedCostInCents;
          }

          totalCostInCents += printJobDetails.CalculatedCostInCents ?? 0;
          _outputWriter.ConsoleWriteLine(printJobDetails.ToString());
        }
        _outputWriter.ConsoleWriteLine($"Total Cost of All Jobs in Cents = {totalCostInCents}.");
        _outputWriter.ConsoleWriteLine(EndingLine);
      }
      catch (Exception e)
      {
        var errorObject =
          new
          {
            Error = e.Message + e.StackTrace,
          };
        return BadRequest(errorObject);
      }

      return Ok(
        new
        {
          CsvRows = printJobDetailsCsvRows,
          //AdditionalInput1 = string.Empty + formData["additionalInput1"],
        }
      );
    }
  }
}
