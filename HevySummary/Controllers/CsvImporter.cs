using Microsoft.AspNetCore.Mvc;

namespace HevySummary.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class CsvImporter
{
    [HttpPost]
    public IActionResult ImportHevyCsv([FromForm] IFormFile file)
    {
        /* 
         * This will be the path through which users can upload the Hevy CSV format
         * How will they upload the file? Posting a binary
         */
        if (file.Length == 0)
        {
            // TODO: be more specific about why it's a bad request
            return new BadRequestResult();
        }

        if (Path.GetExtension(file.FileName).ToLower() != ".csv")
        {
            // TODO: be more specific
            return new BadRequestResult();
        }
        
        // TODO: call CsvService.ImportHevyCsv which will do the work
        
        throw new NotImplementedException();
    }
}