using BSSLTaskManagement.ServicesInterfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text;
using System.IO;
using static BSSLTaskManagement.ViewModels.SystemViewModels;

namespace BSSLTaskManagement.Pages.Menu
{
    public class SystemMenuSetupModel : PageModel 
    {
        private readonly ISystemSerivces system;
        private readonly IWebHostEnvironment environment;

        [BindProperty]
        public GeneralCodesVM SystemMenu { get; set; } = new();
        public List<GeneralCodesVM> SystemMenuList { get; set; } = [];
        public ResponseVM ResponseMessage { get; set; } = new ResponseVM();

 
        public SystemMenuSetupModel(ISystemSerivces system, IWebHostEnvironment environment)
        {
            this.system = system;
            this.environment = environment;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            await LoadSystemMenusAsync();
            return Page();
        }

        private async Task LoadSystemMenusAsync()
        {
            SystemMenuList = await system.GetSystemMenusAsync();
            if (SystemMenuList.Count == 0)
            {
                // Initialize with 6 empty menu entries for user 
                for (int i = 0; i <= 5; i++)
                {
                    SystemMenuList.Add(new GeneralCodesVM
                    {
                        Id = null,
                        Code = "",
                        Description = ""
                    });
                }
            }
            else
            {
                await SaveSystemMenusToFileAsync(SystemMenuList); 
            }
        }

        // POST handler to save system menus data
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                ResponseMessage = new ResponseVM
                {
                    Status = "Error",
                    StatusDescription = "Validation failed"
                };
                return Page();
            }

            // ✅ Fixed: Save each menu individually if service expects single item
            var results = new List<ResponseVM>();

            foreach (var menu in SystemMenuList.Where(m => !string.IsNullOrEmpty(m.Code)))
            {
                var result = await system.SaveSystemMenusAsync(menu);
                results.Add(result);
            }

            // Check if any failed
            var failedCount = results.Count(r => r.Status == "Error" || r.Status == "Failed");

            ResponseMessage = new ResponseVM
            {
                Status = failedCount == 0 ? "Success" : "Partial",
                StatusDescription = failedCount == 0
                    ? "All menus saved successfully"
                    : $"{failedCount} menu(s) failed to save"
            };

            return RedirectToPage("./Success");
        }

        // Alternative: POST handler to save all menus in the table
        public async Task<IActionResult> OnPostSaveAllAsync()
        {
            if (!ModelState.IsValid)
            {
                ResponseMessage = new ResponseVM
                {
                    Status = "Error",
                    StatusDescription = "Validation failed"
                };
                return Page();
            }

            int successCount = 0;
            int failCount = 0;
            string lastError = string.Empty;

            // Filter and save only non-empty menus
            var menusToSave = SystemMenuList
                .Where(m => !string.IsNullOrWhiteSpace(m.Code) && !string.IsNullOrWhiteSpace(m.Description))
                .ToList();

            foreach (var menu in menusToSave)
            {
                var result = await system.SaveSystemMenusAsync(menu);

                if (result.Status == "Success")
                    successCount++;
                else
                {
                    failCount++;
                    lastError = result.StatusDescription;
                }
            }

            ResponseMessage = new ResponseVM
            {
                Status = failCount == 0 ? "Success" : "Partial",
                StatusDescription = failCount == 0
                    ? $"All {successCount} menu(s) saved successfully"
                    : $"Saved: {successCount}, Failed: {failCount}. Last error: {lastError}"
            };

            await LoadSystemMenusAsync();
            return Page();
        }

        private async Task SaveSystemMenusToFileAsync(List<GeneralCodesVM> systemMenus)
        {

            var wwwrootPath = Path.Combine(environment.WebRootPath);
           
            if (!Directory.Exists(wwwrootPath)) Directory.CreateDirectory(wwwrootPath);

            var filePath = Path.Combine(wwwrootPath, "SystemMenus.txt");

            var content = new StringBuilder();

            foreach (var menu in systemMenus)
            {
                content.AppendLine($"Id: {menu.Id}, Code: {menu.Code}, Description: {menu.Description}");
            }

            await System.IO.File.WriteAllTextAsync(filePath, content.ToString());
        }
    }
}



