using BSSLTaskManagement.ServicesInterfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using static BSSLTaskManagement.ViewModels.MainMenuSetupViewModels;

namespace BSSLTaskManagement.Pages.Menu
{
    // Primary constructor injects the service
    // ISystemSerivces   → GetSystemTypesAsync, SaveSystemTypesAsync
    // IMainMenuSetupServices → GetMainMenusAsync

    public class MenuSetupModel
        (ISystemSerivces systemServices,
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

        // ─── AJAX — CASCADE STEP 1 ───────────────────────────────────────────────

        // Called by JS when user selects a System Type in either the form or list panel
        // Returns all modules that belong to the selected system
        // URL pattern: ?handler=ModulesBySystem&systemId=1
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

        // ─── AJAX — CASCADE STEP 2 ───────────────────────────────────────────────
        // URL pattern: ?handler=MainMenusByModule&systemId=1&moduleId=2
        public async Task<IActionResult> OnGetMainMenusByModuleAsync(int systemId, int moduleId)
        {
            var mainMenus = await _mainMenuServices.GetMainMenusAsync(systemId, moduleId);

            var result = mainMenus.Select(x => new
            {
                value = x.Id,
                text = x.Description
            });

            return new JsonResult(result);
        }

        // ─── AJAX — CASCADE STEP 3 ───────────────────────────────────────────────


        // ─── AJAX — LOAD EXISTING MENUS WHEN MAIN MENU IS SELECTED ──────────────────
        // URL pattern: ?handler=MenusByMainMenu&mainMenuId=3
        public async Task<IActionResult> OnGetMenusByMainMenuAsync(int mainMenuId)
        {
            var menus = await _mainMenuServices.GetMenuSetupAsync(mainMenuId);

            var result = menus.Select(x => new
            {
                id = x.Id,
                menuCode = x.MenuCode,
                menuName = x.MenuName,
                savedName = x.SavedName,
                orderNo = x.OrderNo
            });

            return new JsonResult(result);
        }

        // ─── AJAX — SAVE MENU ROWS ───────────────────────────────────────────────────
        public async Task<IActionResult> OnPostSaveMenuSetupAsync([FromBody] MenuSetupDefVM payload)
        {
            var result = await _mainMenuServices.SaveMenuSetupAsync(payload);
            return new JsonResult(result);
        }
    }



}
