using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

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
      }
      catch (Exception e)
      {
        return StatusCode(StatusCodes.Status500InternalServerError, new { Exception = e.StackTrace });
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
