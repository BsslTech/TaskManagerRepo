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
                // Provide 6 empty rows so the table is never blank on first load
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

        // ── Helper: check for duplicate code/description against existing menus ──
        private bool IsDuplicate(
            List<GeneralCodesVM> existingMenus,
            GeneralCodesVM incoming,
            out string duplicateField)
        {
            duplicateField = string.Empty;

            foreach (var menu in existingMenus)
            {
                // Skip the record being edited (same Id)
                if (menu.Id != null && menu.Id == incoming.Id) continue;
                // Skip empty rows
                if (string.IsNullOrWhiteSpace(menu.Code)) continue;

                if (string.Equals(menu.Code, incoming.Code, StringComparison.OrdinalIgnoreCase))
                {
                    duplicateField = "Menu Code";
                    return true;
                }

                if (string.Equals(menu.Description, incoming.Description, StringComparison.OrdinalIgnoreCase))
                {
                    duplicateField = "Menu Name";
                    return true;
                }
            }

            return false;
        }

        // ── Single-record save ──────────────────────────────────────────────────
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                ResponseMessage = new ResponseVM
                {
                    Status = "Error",
                    StatusDescription = "Validation failed. Please check the form and try again."
                };
                await LoadSystemMenusAsync();
                return Page();
            }

            // Load current list so we can check for duplicates server-side
            var currentMenus = await system.GetSystemMenusAsync();

            if (IsDuplicate(currentMenus, SystemMenu, out string duplicateField))
            {
                ResponseMessage = new ResponseVM
                {
                    Status = "Error",
                    StatusDescription = $"A record with the same {duplicateField} already exists. Please use a unique value."
                };
                await LoadSystemMenusAsync();
                return Page();
            }

            ResponseMessage = await system.SaveSystemMenusAsync(SystemMenu);
            await LoadSystemMenusAsync();
            return Page();
        }

        // ── Batch save (all rows in the table) ─────────────────────────────────
        public async Task<IActionResult> OnPostSaveAllAsync()
        {
            if (!ModelState.IsValid)
            {
                ResponseMessage = new ResponseVM
                {
                    Status = "Error",
                    StatusDescription = "Validation failed. Please check the form and try again."
                };
                await LoadSystemMenusAsync();
                return Page();
            }

            // Only process rows that have both Code and Description filled in
            var menusToSave = SystemMenuList
                .Where(m => !string.IsNullOrWhiteSpace(m.Code) && !string.IsNullOrWhiteSpace(m.Description))
                .ToList();

            // ── Detect duplicates within the submitted batch before hitting the DB ──
            var seenCodes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var seenNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var menu in menusToSave)
            {
                if (!seenCodes.Add(menu.Code))
                {
                    ResponseMessage = new ResponseVM
                    {
                        Status = "Error",
                        StatusDescription = $"Duplicate Menu Code \"{menu.Code}\" found in the submitted list. Please correct and resubmit."
                    };
                    await LoadSystemMenusAsync();
                    return Page();
                }

                if (!seenNames.Add(menu.Description))
                {
                    ResponseMessage = new ResponseVM
                    {
                        Status = "Error",
                        StatusDescription = $"Duplicate Menu Name \"{menu.Description}\" found in the submitted list. Please correct and resubmit."
                    };
                    await LoadSystemMenusAsync();
                    return Page();
                }
            }

            // ── Also check against existing DB records ──
            var currentMenus = await system.GetSystemMenusAsync();

            foreach (var menu in menusToSave)
            {
                if (IsDuplicate(currentMenus, menu, out string duplicateField))
                {
                    ResponseMessage = new ResponseVM
                    {
                        Status = "Error",
                        StatusDescription = $"A record with the same {duplicateField} (\"{(duplicateField == "Menu Code" ? menu.Code : menu.Description)}\") already exists in the database."
                    };
                    await LoadSystemMenusAsync();
                    return Page();
                }
            }

            // ── All clear — save each record ──
            int successCount = 0;
            int failCount = 0;
            string lastError = string.Empty;

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
                Status = failCount == 0 ? "Success" : "Error",
                StatusDescription = failCount == 0
                    ? $"All {successCount} menu(s) saved successfully."
                    : $"Saved: {successCount}, Failed: {failCount}. Last error: {lastError}"
            };

            await LoadSystemMenusAsync();
            return Page();
        }

        // ── Write current menus to a flat file in wwwroot ──────────────────────
        private async Task SaveSystemMenusToFileAsync(List<GeneralCodesVM> systemMenus)
        {
            var wwwrootPath = environment.WebRootPath;

            if (!Directory.Exists(wwwrootPath))
                Directory.CreateDirectory(wwwrootPath);

            var filePath = Path.Combine(wwwrootPath, "SystemMenus.txt");
            var content = new StringBuilder();

            foreach (var menu in systemMenus)
                content.AppendLine($"Id: {menu.Id}, Code: {menu.Code}, Description: {menu.Description}");

            await System.IO.File.WriteAllTextAsync(filePath, content.ToString());
        }
    }
}