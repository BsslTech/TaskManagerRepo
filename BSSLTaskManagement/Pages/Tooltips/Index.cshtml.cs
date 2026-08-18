using BSSLTaskManagement.Api;
using BSSLTaskManagement.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace BSSLTaskManagement.Pages.Tooltips
{
    public class IndexModel : PageModel
    {
        private readonly IWebHostEnvironment _environment;

        public IndexModel(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public List<TooltipSchema> Forms { get; set; } = new();

        public TooltipSchema? SelectedForm { get; set; }

        public TooltipContent? SelectedContent { get; set; }

        public async Task OnGetAsync(string? formCode)
        {
            Forms = LoadSchemas();

            if (!string.IsNullOrWhiteSpace(formCode))
            {
                SelectedForm = Forms.FirstOrDefault(
                    x => x.FormCode.Equals(
                        formCode,
                        StringComparison.OrdinalIgnoreCase));

                if (SelectedForm != null)
                {
                    SelectedContent = LoadContent(
                        SelectedForm.FormCode);
                }
            }

            await Task.CompletedTask;
        }

        private List<TooltipSchema> LoadSchemas()
        {
            var directory = Path.Combine(
                _environment.WebRootPath,
                "tooltips");

            if (!Directory.Exists(directory))
            {
                return new List<TooltipSchema>();
            }

            var files = Directory.GetFiles(
                directory,
                "*.schema.json");

            var result = new List<TooltipSchema>();

            foreach (var file in files)
            {
                try
                {
                    var json = System.IO.File.ReadAllText(file);

                    var schema =
                        JsonSerializer.Deserialize<TooltipSchema>(
                            json,
                            new JsonSerializerOptions
                            {
                                PropertyNameCaseInsensitive = true
                            });

                    if (schema != null)
                    {
                        result.Add(schema);
                    }
                }
                catch
                {
                    // Ignore invalid schema files for now
                }
            }

            return result
                .OrderBy(x => x.FormName)
                .ToList();
        }

        private TooltipContent? LoadContent(string formCode)
        {
            var filePath = Path.Combine(
                _environment.WebRootPath,
                "tooltips",
                $"{formCode}.content.json");

            if (!System.IO.File.Exists(filePath))
            {
                return null;
            }

            var json = System.IO.File.ReadAllText(filePath);

            return JsonSerializer.Deserialize<TooltipContent>(
                json,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
        }
    }
}