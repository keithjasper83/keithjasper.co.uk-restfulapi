using Microsoft.AspNetCore.Mvc;

namespace keithjasper.co.uk_restfulapi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DocumentationController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetApiDocumentation()
        {
            var assembly = typeof(DocumentationController).Assembly;
            var xmlFile = $"{assembly.GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

            if (System.IO.File.Exists(xmlPath))
            {
                // Return the XML file as a FileStreamResult
                return File(System.IO.File.OpenRead(xmlPath), "application/xml");
            }
            else
            {
                return NotFound("XML documentation file not found.");
            }
        }
    }
}
