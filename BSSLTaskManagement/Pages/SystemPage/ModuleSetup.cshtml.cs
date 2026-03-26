using BSSLTaskManagement.ServicesInterfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Reflection;
using static BSSLTaskManagement.ViewModels.SystemViewModels;

namespace BSSLTaskManagement.Pages.SystemPage
{
    public class ModuleSetupModel(ISystemSerivces system, IWebHostEnvironment environment) : PageModel
    {
        private readonly ISystemSerivces system = system;
        private readonly IWebHostEnvironment environment = environment;


        [BindProperty]
        public SystemTypeVM SystemTypes { get; set; } = new();
        public List<SystemTypeVM> SystemTypesList { get; set; } = [];
        public ModuleDetailsVM module { get; set; } = new();
        public List<ModuleDetailsVM> moduleList { get; set; } = [];
        public ResponseVM ResponseMessage { get; set; } = new ResponseVM();
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            await PopulateDropdownsAsync();
            await LoadModulesAsync();
            return Page();
        }

        private async Task LoadModulesAsync()
        {
            moduleList = await system.GetModulesAsync();
            if (moduleList.Count == 0)
            {
                // Initialize with 6 empty module entries for user
                for (int i = 0; i <= 5; i++)
                {
                    moduleList.Add(new ModuleDetailsVM
                    {
                        ModuleId = null,
                        ModuleCode = "",
                        ModuleDescription = ""
                    });
                }
            }
        }

        private async Task PopulateDropdownsAsync()
        {
            throw new NotImplementedException();
        }
    }
}
