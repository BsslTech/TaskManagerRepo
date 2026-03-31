using BSSLTaskManagement.ServicesInterfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Reflection;
using static BSSLTaskManagement.ViewModels.SystemViewModels;


#nullable disable
namespace BSSLTaskManagement.Pages.SystemPage
{
    public class ModuleSetupModel(ISystemSerivces system, IWebHostEnvironment environment) : PageModel
    {
        private readonly ISystemSerivces system = system;
        private readonly IWebHostEnvironment environment = environment;


        
        public SystemTypeVM SystemTypes { get; set; } = new();
        public List<SystemTypeVM> SystemTypesList { get; set; } = [];
        [BindProperty]
        public ModuleSetupVM ModuleSetup { get; set; } = new();
        public ResponseVM ResponseMessage { get; set; } = new ResponseVM();
        public async Task<IActionResult> OnGetAsync()
        {
            await LoadModulesAsync();
            return Page();
        }

        private async Task LoadModulesAsync()
        {
            SystemTypesList = await system.GetSystemTypesAsync();
             for (int i = 0; i <= 5; i++)
            {
                ModuleSetup.ModuleDetails
                .Add(new ModuleDetailsVM
                {
                    ModuleId = null,
                    ModuleCode = "",
                    ModuleDescription = ""
                });
            }
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var modules = await ReadTextFile("Modules.txt", 1);
                    if (modules.Count > 0)
                    {
                        var payload = new ModuleSetupVM
                        {
                            SystemId = ModuleSetup.SystemId,
                            ModuleDetails = modules
                        };
                        ResponseMessage = await system.SaveModuleSetupAsync(payload);
                        if (ResponseMessage.Status == "Success")
                        {
                            ModelState.Clear();
                            ModuleSetup = new ModuleSetupVM
                            {
                                SystemId = null,
                                ModuleDetails = [],
                            };
                        }
                    }
                }
                catch (Exception ex)
                {
                    _ = ex.Message.ToString();
                }
            }
            await LoadModulesAsync();
            return Page();
        }
        public class UpdateTextFileRequest
        {
            public string Uniqno { get; set; }
            public string UniqueUpdated { get; set; }
            public int? SystemId { get; set; }
            public int Todo { get; set; }
            public List<string> Value { get; set; }
        }
        public async Task<IActionResult> OnGetSystemModulesAsync(string systemId)
        {
            string status = "Failed";
            var result = new JsonResult(new { status, statusDescription = "No module found for the selected system type, please try again." });
            var modules = await system.GetModulesTypeSystemAsync(Convert.ToInt32(systemId));
            if (modules.Count > 0)
            {
                await CreateTextFile(modules); status = "Success";
            }
            else
                for (int i = 0; i <= 5; i++)
                {
                    modules
                    .Add(new ModuleDetailsVM
                    {
                        ModuleId = null,
                        ModuleCode = "",
                        ModuleDescription = ""
                    });
                }

            result = new JsonResult(new { status, statusDescription = modules });

            return result;
        }
        public async Task<IActionResult> OnPostUpdateTextFileAsync([FromBody] UpdateTextFileRequest request)
        {
            var uniqno = request.Uniqno;
            var moduleCode = request.UniqueUpdated;
            var todo = request.Todo;
            var value = request.Value;
            int? systemId = request.SystemId;

            var result = new JsonResult(new { status = "failed", statusDescription = "An error has occurred, please try again." });
            var systems = await system.GetSystemTypesAsync();
            try
            {
                var systemModules = await ReadTextFile("Modules.txt", 0);
                if (systemModules.Count > 0)
                {
                    var module = systemModules.FirstOrDefault(x => x.ModuleCode == uniqno);

                    if (todo == 1) // Update / Add Row
                    {
                        var column = value[0];      // 1 or 2
                        var columnValue = value[1]; // actual value

                        if (module != null) // Updating existing row
                        {

                            systemModules.ForEach(x =>
                            {
                                if (x.ModuleCode == uniqno)
                                {
                                    if (column == "1")
                                        x.ModuleCode = columnValue;

                                    if (column == "2")
                                        x.ModuleDescription = columnValue;
                                }
                            });
                        }
                        else
                        {
                            var system = systems.FirstOrDefault(k=>k.SystemId == systemId);
                            systemModules.Add(new ModuleDetailsVM
                            {
                                HelperFile = null,
                                HelperFileName = "",
                                ModuleCode = moduleCode,
                                ModuleDescription = "",
                                Code = string.IsNullOrWhiteSpace(uniqno) ? moduleCode : uniqno,
                                SystemDescription = system.SystemDescription,
                                SystemCode = system.SystemCode,
                                SystemId = system.SystemId,
                            });
                        }
                    }
                    else // Remove Row
                    {
                        if (module != null)
                            systemModules.Remove(module);
                    }
                    await CreateTextFile(systemModules);

                    result = new JsonResult(new { status = "Success", statusDescription = "Updated successfully" });
                }
            }
            catch (Exception ex)
            {
                return new JsonResult(new { status = "Error", statusDescription = ex.Message });
            }

            return result;
        }
        public Task CreateTextFile(List<ModuleDetailsVM> modules)
        {
            var detail = JsonConvert.SerializeObject(modules, Formatting.Indented);
            var folderPath = Path.Combine(environment.ContentRootPath, "TextFiles");
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);
            var filePath = Path.Combine(folderPath, $"Modules.txt");

            if (System.IO.File.Exists(filePath))
                System.IO.File.Delete(filePath);
            System.IO.File.WriteAllText(filePath, detail + Environment.NewLine); // Use System.IO.File explicitly to avoid ambiguity  
            return Task.CompletedTask;
        }
        public Task<List<ModuleDetailsVM>> ReadTextFile(string textFile, int delete)
        {
            var modules = new List<ModuleDetailsVM>();
            var folderPath = Path.Combine(environment.ContentRootPath, "TextFiles");
            var filePath = Path.Combine(folderPath, textFile);

            if (System.IO.File.Exists(filePath))
            {
                var detail = System.IO.File.ReadAllText(filePath);
                modules = JsonConvert.DeserializeObject<List<ModuleDetailsVM>>(detail);

                if (delete == 1)
                    System.IO.File.Delete(filePath);
            }
            return Task.FromResult(modules);
        }
    }
}
