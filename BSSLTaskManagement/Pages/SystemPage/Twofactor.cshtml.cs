using BSSLTaskManagement.ServicesInterfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using static BSSLTaskManagement.ViewModels.SystemViewModels;

namespace BSSLTaskManagement.Pages.SystemPage
{
    public class TwoFactorModel(ISystemSerivces _system, IWebHostEnvironment _environment) : PageModel
    {
        [BindProperty]
        public TwoFactorVM TwoFactor { get; set; } = new();
        public ResponseVM ResponseMessage { get; set; } = new ResponseVM();
        public async Task<IActionResult> OnGetAsync()
        {
            await LoadTwoFactorAsync();
            return Page();
        }

        private async Task LoadTwoFactorAsync()
        {
            TwoFactor = await _system.GetTwoFactorAuthenticationAsync();
            if (TwoFactor == null)
            {
                TwoFactor = new TwoFactorVM
                {
                    UseType = null,
                    MaxNumber = null,
                    Seconds = null,
                    TwoFactorDetails = [],
                };
                for (int i = 0; i <= 3; i++)
                {
                    TwoFactor.TwoFactorDetails
                    .Add(new TwoFactorSetupVM
                    {
                        Code = "",
                        Description = ""
                    });
                }
            }

        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var twoFactors = await ReadTextFile("TwoFactors.txt", 1);
                    if (twoFactors.Count > 0)
                    {
                        var payload = new TwoFactorVM
                        {
                            UseType = TwoFactor.UseType,
                            MaxNumber = TwoFactor.MaxNumber,
                            Seconds = TwoFactor.Seconds,
                            TwoFactorDetails = twoFactors
                        };

                        ResponseMessage = await _system.SaveTwoFactorAuthenticationAsync(payload);
                        if (ResponseMessage.Status == "Success")
                        {
                            ModelState.Clear();
                            TwoFactor = new TwoFactorVM
                            {
                                UseType = null,
                                MaxNumber = null,
                                Seconds = null,
                                TwoFactorDetails = [],
                            };
                            for (int i = 0; i <= 3; i++)
                            {
                                TwoFactor.TwoFactorDetails
                                .Add(new TwoFactorSetupVM
                                {
                                    Code = "",
                                    Description = ""
                                });
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _ = ex.Message.ToString();
                }
            }
            await LoadTwoFactorAsync();
            return Page();
        }
        public class UpdateTextFileRequest
        {
            public string Uniqno { get; set; }
            public string UniqueUpdated { get; set; }
            public int Todo { get; set; }
            public List<string> Value { get; set; }
        }
        public async Task<IActionResult> OnPostUpdateTextFileAsync([FromBody] UpdateTextFileRequest request)
        {
            var uniqno = request.Uniqno;
            var code = request.UniqueUpdated;
            var todo = request.Todo;
            var value = request.Value;

            var result = new JsonResult(new { status = "failed", statusDescription = "An error has occurred, please try again." });
            var factors = await _system.GetTwoFactorAuthenticationAsync();
            try
            {
                var twoFactors = await ReadTextFile("TwoFactors.txt", 0);
                if (twoFactors.Count > 0)
                {
                    var twoFactor = twoFactors.FirstOrDefault(x => x.Code == uniqno);

                    if (todo == 1) // Update / Add Row
                    {
                        var column = value[0];      // 1 or 2
                        var columnValue = value[1]; // actual value

                        if (twoFactor != null) // Updating existing row
                        {

                            twoFactors.ForEach(x =>
                            {
                                if (x.Code == uniqno)
                                {
                                    if (column == "1")
                                        x.Code = columnValue;

                                    if (column == "2")
                                        x.Description = columnValue;
                                }
                            });
                        }
                        else
                        {
                            twoFactors.Add(new TwoFactorSetupVM
                            {
                                Code = string.IsNullOrWhiteSpace(uniqno) ? code : uniqno,
                                Description = "",
                            });
                        }
                    }
                    else // Remove Row
                    {
                        if (twoFactor != null)
                            twoFactors.Remove(twoFactor);
                    }
                    await CreateTextFile(twoFactors);

                    result = new JsonResult(new { status = "Success", statusDescription = "Updated successfully" });
                }
            }
            catch (Exception ex)
            {
                return new JsonResult(new { status = "Error", statusDescription = ex.Message });
            }

            return result;
        }
        public Task CreateTextFile(List<TwoFactorSetupVM> twoFactors)
        {
            var detail = JsonConvert.SerializeObject(twoFactors, Formatting.Indented);
            var folderPath = Path.Combine(_environment.ContentRootPath, "TextFiles");
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);
            var filePath = Path.Combine(folderPath, $"TwoFactors.txt");

            if (System.IO.File.Exists(filePath))
                System.IO.File.Delete(filePath);
            System.IO.File.WriteAllText(filePath, detail + Environment.NewLine); // Use System.IO.File explicitly to avoid ambiguity  
            return Task.CompletedTask;
        }
        public Task<List<TwoFactorSetupVM>> ReadTextFile(string textFile, int delete)
        {
            var twoFactors = new List<TwoFactorSetupVM>();
            var folderPath = Path.Combine(_environment.ContentRootPath, "TextFiles");
            var filePath = Path.Combine(folderPath, textFile);

            if (System.IO.File.Exists(filePath))
            {
                var detail = System.IO.File.ReadAllText(filePath);
                twoFactors = JsonConvert.DeserializeObject<List<TwoFactorSetupVM>>(detail);

                if (delete == 1)
                    System.IO.File.Delete(filePath);
            }
            return Task.FromResult(twoFactors);
        }
    }
}