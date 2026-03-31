using BSSLTaskManagement.ServicesInterfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using static BSSLTaskManagement.ViewModels.MainMenuSetupViewModels;
using static BSSLTaskManagement.ViewModels.SystemViewModels;

namespace BSSLTaskManagement.Pages.Menu
{
    // Primary constructor injects the service
    // ISystemSerivces   → GetSystemTypesAsync, SaveSystemTypesAsync, GetModulesTypeSystemAsync

    public class MainMenuSetupModel
    (ISystemSerivces systemServices,
     IMainMenuSetupServices mainMenuServices) : PageModel
    {
        private readonly ISystemSerivces _systemServices = systemServices;
        private readonly IMainMenuSetupServices _mainMenuServices = mainMenuServices;

        public List<SelectListItem> SystemTypeList { get; set; } = [];

        public async Task OnGetAsync()
        {
            await LoadSystemTypesAsync();
        }

        private async Task LoadSystemTypesAsync()
        {
            var systemTypes = await _systemServices.GetSystemTypesAsync();
            SystemTypeList = [.. systemTypes
            .Select(x => new SelectListItem
            {
                Value = x.SystemId.ToString(),
                Text = x.SystemDescription
            })];
        }

        // ─── AJAX — CASCADE STEP 1 ───────────────────────────────────────────────
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

        // ─── AJAX — LOAD EXISTING MAIN MENUS WHEN MODULE IS SELECTED ────────────
        // URL pattern: ?handler=MainMenusByModule&systemId=1&moduleId=2
        public async Task<IActionResult> OnGetMainMenusByModuleAsync(int systemId, int moduleId)
        {
            var menus = await _mainMenuServices.GetMainMenusAsync(systemId, moduleId);
            var result = menus.Select(x => new
            {
                id = x.Id,
                description = x.Description,
                savedDescription = x.Description,
                orderNo = x.OrderNo
            });
            return new JsonResult(result);
        }

        // ─── AJAX — SAVE MAIN MENU ROWS ──────────────────────────────────────────
        public async Task<IActionResult> OnPostSaveMainMenusAsync([FromBody] MainMenuDefVM payload)
        {
            var result = await _mainMenuServices.SaveMainMenusAsync(payload);
            return new JsonResult(result);
        }
    }
}


//    public class MainMenuSetupModel
//        (ISystemSerivces systemServices) : PageModel
//    {
//        private readonly ISystemSerivces _systemServices = systemServices;

//        // Bound to the System Type <select> on page load

//        // Bound to the System Type <select> on page load
//        public List<SelectListItem> SystemTypeList { get; set; } = [];

//        // ─── PAGE LOAD ────────────────────────────────────────────────────────────

//        // Runs on first GET — only System Type needs to be pre-loaded
//        // Everything else loads via AJAX as user makes selections
//        public async Task OnGetAsync()
//        {
//            await LoadSystemTypesAsync();
//        }

//        // Fetches all system types from DB and converts to SelectListItem
//        private async Task LoadSystemTypesAsync()
//        {
//            var systemTypes = await _systemServices.GetSystemTypesAsync();
//            SystemTypeList = systemTypes
//                .Select(x => new SelectListItem
//                {
//                    Value = x.SystemId.ToString(),
//                    Text = x.SystemDescription
//                }).ToList();

//        }

//        // ─── AJAX — CASCADE STEP 1 ───────────────────────────────────────────────

//        // Called by JS when user selects a System Type in either the form or list panel
//        // Returns all modules that belong to the selected system
//        // URL pattern: ?handler=ModulesBySystem&systemId=1
//        public async Task<IActionResult> OnGetModulesBySystemAsync(int systemId)
//        {
//            var modules = await _systemServices.GetModulesTypeSystemAsync(systemId);

//            var result = modules.Select(x => new
//            {
//                value = x.ModuleId,
//                text = x.ModuleDescription
//            });

//            return new JsonResult(result);
//        }
//    }
//}