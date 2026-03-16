using BSSLTaskManagement.ServicesInterfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using static BSSLTaskManagement.ViewModels.SystemViewModels;

#nullable disable
namespace BSSLTaskManagement.Pages.SystemPage
{
    public class SystemModel(ISystemSerivces system, IWebHostEnvironment environment) : PageModel
    {
        private readonly ISystemSerivces system = system;
        private readonly IWebHostEnvironment environment = environment;

        [BindProperty]
        public List<SystemTypeVM> SystemTypes { get; set; } = [];
        public ResponseVM ResponseMessage { get; set; } = new ResponseVM();
        public async Task<IActionResult> OnGetAsync()
        {
            await OnPageLoadAsync();
            return Page();
        }
        public async Task OnPageLoadAsync()
        {
            SystemTypes = await system.GetSystemTypesAsync();
            if (SystemTypes.Count == 0)
            {
                for (int i = 0; i < 5; i++)
                {
                    SystemTypes.Add(new SystemTypeVM
                    {
                        HelperFile = null,
                        HelperFileName = "",
                        SystemCode = "",
                        Code = "",
                        SystemDescription = "",
                        SystemId = null
                    });
                }
            }
            else
                await CreateTextFile(SystemTypes);
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var systemTypes = await ReadTextFile("SystemTypes.txt", 0);
                    if (systemTypes.Count > 0)
                        ResponseMessage = await system.SaveSystemTypesAsync(systemTypes);
                }
                catch (Exception ex)
                {
                    _ = ex.Message.ToString();
                }
            }
            await OnPageLoadAsync();
            return Page();
        }
        public async Task<IActionResult> OnPostUploadHelperFileAsync(IFormFile helperFile, string uniqno)
        {
            try
            {
                // Don't persist the uploaded file to disk yet. Only update the in-memory text file
                if (helperFile == null || helperFile.Length == 0)
                    return new JsonResult(new { status = "Error", statusDescription = "NoFile" });

                if (helperFile != null && helperFile.Length > 0)
                {
                    string folderPath = Path.Combine(environment.WebRootPath, "HelpFile");
                    // Ensure folder exists
                    if (!Directory.Exists(folderPath))
                        Directory.CreateDirectory(folderPath);

                    var extension = Path.GetExtension(helperFile.FileName).ToLower();

                    var allowedExtensions = new[] { ".pdf", ".doc", ".docx" };

                    if (!allowedExtensions.Contains(extension))
                    {
                        //new JsonResult(new { status = "Success", fileName = fileNames });
                        return new JsonResult(new
                        {
                            status = "Error",
                            statusDescription = $"Invalid file type for {uniqno}"
                        });
                    }

                    string fileName = $"{uniqno}_{DateTime.Now:yyyyMMddHHmmss}{extension}";
                    string newFilePath = Path.Combine(folderPath, fileName);

                    using var stream = new FileStream(newFilePath, FileMode.Create);
                    await helperFile.CopyToAsync(stream);


                    var systemTypes = await ReadTextFile("SystemTypes.txt", 0);
                    systemTypes.ForEach(x =>
                    {
                        if (x.Code == uniqno)
                        {
                            x.HelperFile = null;
                            // Update only the text record with the uploaded file name (do not save file contents)
                            fileName = helperFile.FileName ?? string.Empty;
                            x.HelperFileName = !string.IsNullOrWhiteSpace(fileName) ? fileName : string.Empty;
                        }
                    });

                    await CreateTextFile(systemTypes);

                    return new JsonResult(new { status = "Success", statusDescription = fileName });
                }
                else
                {
                    return new JsonResult(new
                    {
                        status = "Error",
                        statusDescription = $"File could not be uploaded",
                    });
                }
            }
            catch (Exception ex)
            {
                return new JsonResult(new { status = "Error", statusDescription = ex.Message });
            }
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
            var systemCode = request.UniqueUpdated;
            var todo = request.Todo;
            var value = request.Value;

            var result = new JsonResult(new { status = "failed", statusDescription = "An error has occurred, please try again." });

            try
            {
                var systemTypes = await ReadTextFile("SystemTypes.txt", 0);

                if (systemTypes.Count > 0)
                {
                    var systemType = systemTypes.FirstOrDefault(x => x.Code == uniqno);

                    if (todo == 1) // Update / Add Row
                    {
                        var column = value[0];      // 1 or 2
                        var columnValue = value[1]; // actual value

                        if (systemType != null) // Updating existing row
                        {

                            systemTypes.ForEach(x =>
                            {
                                if (x.Code == uniqno)
                                {
                                    if (column == "1")
                                        x.SystemCode = columnValue;

                                    if (column == "2")
                                        x.SystemDescription = columnValue;

                                    // if (column == "3")
                                    //     x.HelperFile = columnValue;
                                }
                            });                              
                        }
                        else
                        {
                            systemTypes.Add(new SystemTypeVM
                            {
                                HelperFile = null,
                                HelperFileName = "",
                                SystemCode = systemCode,
                                Code = string.IsNullOrWhiteSpace(uniqno) ? systemCode : uniqno,
                                SystemDescription = "",
                                SystemId = null
                            });
                        }
                    }
                    else // Remove Row
                    {
                        if (systemType != null)
                        {
                            systemTypes.Remove(systemType);

                            if (!string.IsNullOrEmpty(systemType.HelperFileName))
                            {
                                string folderPath = Path.Combine(environment.WebRootPath, "HelpFile");
                                // Ensure folder exists
                                if (!Directory.Exists(folderPath))
                                    Directory.CreateDirectory(folderPath);

                                var oldFile = Path.Combine(folderPath, systemType.HelperFileName ?? "");

                                if (System.IO.File.Exists(oldFile))
                                    System.IO.File.Delete(oldFile);
                            }
                        }
                    }

                    await CreateTextFile(systemTypes);

                    result = new JsonResult(new { status = "Success", statusDescription = "Updated successfully" });
                }
            }
            catch (Exception ex)
            {
                return new JsonResult(new { status = "Error", statusDescription = ex.Message });
            }

            return result;
        }
        public Task CreateTextFile(List<SystemTypeVM> systemTypes)
        {
            var detail = JsonConvert.SerializeObject(systemTypes, Formatting.Indented);
            var folderPath = Path.Combine(environment.ContentRootPath, "TextFiles");
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);
            var filePath = Path.Combine(folderPath, $"SystemTypes.txt");

            if (System.IO.File.Exists(filePath))
                System.IO.File.Delete(filePath);
            System.IO.File.WriteAllText(filePath, detail + Environment.NewLine); // Use System.IO.File explicitly to avoid ambiguity  
            return Task.CompletedTask;
        }
        public Task<List<SystemTypeVM>> ReadTextFile(string textFile, int delete)
        {
            var systemTypes = new List<SystemTypeVM>();
            var folderPath = Path.Combine(environment.ContentRootPath, "TextFiles");
            var filePath = Path.Combine(folderPath, textFile);

            if (System.IO.File.Exists(filePath))
            {
                var detail = System.IO.File.ReadAllText(filePath);
                systemTypes = JsonConvert.DeserializeObject<List<SystemTypeVM>>(detail);

                if (delete == 1)
                    System.IO.File.Delete(filePath);
            }
            return Task.FromResult(systemTypes);
        }
    }
}
