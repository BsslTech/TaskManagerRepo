using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Configuration;
using System.Text.Json;

namespace BSSLTaskManagement.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class TooltipsController : ControllerBase
    {
        private readonly IWebHostEnvironment _environment;
        private readonly IConfiguration _configuration;

        public TooltipsController(IWebHostEnvironment environment, IConfiguration configuration)
        {
            _environment = environment;
            _configuration = configuration;
        }

        // Syncs the tooltip schema from the ERP to the File in TaskManager.
        [HttpPost("sync")]
        public async Task<IActionResult> Sync([FromBody] TooltipSchema schema)
        {

            var authResult = ValidateApiKey();
            if (authResult != null) return authResult;


            if (schema == null || string.IsNullOrWhiteSpace(schema.FormCode))
                return BadRequest("formCode is required.");

            var directory = Path.Combine(
                _environment.WebRootPath,
                "tooltips"
            );

            Directory.CreateDirectory(directory);

            var filePath = Path.Combine(
                directory,
                $"{schema.FormCode}.schema.json"
            );

            schema.UpdatedUtc = DateTime.UtcNow;

            var json = JsonSerializer.Serialize(
                schema,
                new JsonSerializerOptions
                {
                    WriteIndented = true
                }
            );

            await System.IO.File.WriteAllTextAsync(filePath, json);

            return Ok(new
            {
                success = true,
                formCode = schema.FormCode
            });
        }

        [HttpGet("schema/{formCode}")]
        public IActionResult GetSchema(string formCode)
        {
            var authResult = ValidateApiKey();
            if (authResult != null) return authResult;


            var filePath = Path.Combine(
                _environment.WebRootPath,
                "tooltips",
                $"{formCode}.schema.json"
            );

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }

            var json = System.IO.File.ReadAllText(filePath);

            return Content(json, "application/json");
        }

        [HttpPost("content/{formCode}")]
        public IActionResult SaveContent(string formCode, [FromBody] TooltipContentField field)
        {
            //var authResult = ValidateApiKey();
            //if (authResult != null) return authResult;


            var filePath = Path.Combine(
                _environment.WebRootPath,
                "tooltips",
                $"{formCode}.content.json");

            TooltipContent content;

            if (System.IO.File.Exists(filePath))
            {
                var existingJson = System.IO.File.ReadAllText(filePath);

                content = JsonSerializer.Deserialize<TooltipContent>(
                    existingJson,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    })
                    ?? new TooltipContent();
            }
            else
            {
                content = new TooltipContent
                {
                    FormCode = formCode
                };
            }

            var existingField = content.Fields.FirstOrDefault(x =>
                x.Code.Equals(field.Code, StringComparison.OrdinalIgnoreCase));

            if (existingField != null)
            {
                existingField.Label = field.Label;
                //existingField.Title = field.Title;
                existingField.Message = field.Message;
                existingField.IsActive = field.IsActive;
            }
            else
            {
                content.Fields.Add(field);
            }

            content.FormCode = formCode;
            content.UpdatedUtc = DateTime.UtcNow;

            var json = JsonSerializer.Serialize(
                content,
                new JsonSerializerOptions
                {
                    WriteIndented = true
                });

            System.IO.File.WriteAllText(filePath, json);

            return Ok(content);
        }

        public IActionResult? ValidateApiKey()
        {
            var configuredApiKey = _configuration["TooltipSync:ApiKey"];
            var providedApiKey = Request.Headers["X-Tooltip-Api-Key"].FirstOrDefault();

            if (string.IsNullOrEmpty(configuredApiKey) ||
                providedApiKey != configuredApiKey)
            {
                return Unauthorized();
            }
            return null;
        }
    }

    public class TooltipSchema
    {
        public string FormCode { get; set; } = "";
        public string FormName { get; set; } = "";
        public DateTime UpdatedUtc { get; set; }
        public List<TooltipField> Fields { get; set; } = new();
    }

    public class TooltipField
    {
        public string Code { get; set; } = "";
        public string Label { get; set; } = "";
        public int Order { get; set; }
    }

    public class TooltipContent
    {
        public string FormCode { get; set; } = "";
        public string FormName { get; set; } = "";
        public DateTime UpdatedUtc { get; set; }
        public List<TooltipContentField> Fields { get; set; } = new();
    }

    public class TooltipContentField
    {
        public string Code { get; set; } = "";
        public string Label { get; set; } = "";
        //public string Title { get; set; } = "";
        public string Message { get; set; } = "";
        public bool IsActive { get; set; }
    }

}