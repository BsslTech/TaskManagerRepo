using BSSLTaskManagement.ServicesInterfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

#nullable disable
namespace BSSLTaskManagement.Pages.Menu
{
    // Primary constructor injects both services
    // ISystemSerivces   → GetSystemTypesAsync, GetModulesTypeSystemAsync
    // IMainMenuSetupServices → GetMainMenusAsync, GetMenuSetupAsync
    public class SubMenuSetupModel(
        ISystemSerivces systemServices,
        IMainMenuSetupServices mainMenuServices) : PageModel
    {
        private readonly ISystemSerivces _systemServices = systemServices;
        private readonly IMainMenuSetupServices _mainMenuServices = mainMenuServices;

        // Bound to the System Type <select> on page load
        public List<SelectListItem> SystemTypeList { get; set; } = [];

        // ─── PAGE LOAD ────────────────────────────────────────────────────────────

        // Runs on first GET — only System Type needs to be pre-loaded
        // Everything else loads via AJAX as user makes selections
        public async Task OnGetAsync()
        {
            await LoadSystemTypesAsync();
        }

        // Fetches all system types from DB and converts to SelectListItem
        private async Task LoadSystemTypesAsync()
        {
            var systemTypes = await _systemServices.GetSystemTypesAsync();
            SystemTypeList = systemTypes
                .Select(x => new SelectListItem
                {
                    Value = x.SystemId.ToString(),
                    Text = x.SystemDescription
                }).ToList();
        }

        // ─── AJAX HANDLERS ───────────────────────────────────────────────────────
        // Each handler corresponds to a dropdown cascade step
        // They all return JSON so JavaScript can populate the next dropdown

        // STEP 1 — User picks System Type → load Modules for that system
        // Called by JS: fetch(`?handler=ModulesBySystem&systemId=${systemId}`)
        public async Task<IActionResult> OnGetModulesBySystemAsync(int systemId)
        {
            var modules = await _systemServices.GetModulesTypeSystemAsync(systemId);

            var result = modules.Select(x => new
            {
                value = x.ModuleId,
                text = x.ModuleDescription
            });

            return new JsonResult(result);
        }

        // STEP 2 — User picks Module → load Main Menus for that system + module
        // Called by JS: fetch(`?handler=MainMenusByModule&systemId=${systemId}&moduleId=${moduleId}`)
        public async Task<IActionResult> OnGetMainMenusByModuleAsync(int systemId, int moduleId)
        {
            // GetMainMenusAsync filters by both systemId and moduleId
            var mainMenus = await _mainMenuServices.GetMainMenusAsync(systemId, moduleId);

            var result = mainMenus.Select(x => new
            {
                value = x.Id,
                text = x.Description
            });

            return new JsonResult(result);
        }

        // STEP 3 — User picks Main Menu → load Menus under that main menu
        // Called by JS: fetch(`?handler=MenusByMainMenu&mainMenuId=${mainMenuId}`)
        public async Task<IActionResult> OnGetMenusByMainMenuAsync(int mainMenuId)
        {
            // GetMenuSetupAsync filters by mainMenuId
            var menus = await _mainMenuServices.GetMenuSetupAsync(mainMenuId);

            var result = menus.Select(x => new
            {
                value = x.Id,
                text = x.MenuName
            });

            return new JsonResult(result);
        }
    }
}