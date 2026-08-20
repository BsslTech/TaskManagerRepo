using BSSLTaskManagement.ServicesInterfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using static BSSLTaskManagement.ViewModels.MainMenuSetupViewModels;
using static BSSLTaskManagement.ViewModels.SystemViewModels;

#nullable disable
namespace BSSLTaskManagement.Pages.Menu
{
    // Primary constructor injects both services
    // ISystemSerivces        → GetSystemTypesAsync, GetModulesTypeSystemAsync
    // IMainMenuSetupServices → GetMainMenusAsync, GetMenuSetupAsync,
    //                          GetSubMenuSetupListAsync, GetSubMenuSetupSingleAsync,
    //                          SaveSubMenuSetupAsync
    public class SubMenuSetupModel(
        ISystemSerivces systemServices,
        IMainMenuSetupServices mainMenuServices) : PageModel
    {
        private readonly ISystemSerivces _systemServices = systemServices;
        private readonly IMainMenuSetupServices _mainMenuServices = mainMenuServices;

        // ─── BOUND PROPERTIES ────────────────────────────────────────────────────

        // Powers the System Type dropdown on page load
        public List<SelectListItem> SystemTypeList { get; set; } = [];

        // Receives all form field values when Save is clicked (POST)
        // [BindProperty] tells Razor Pages to map incoming POST data to this object
        [BindProperty]
        public SubMenuSetupVM SubMenuForm { get; set; } = new();

        // ─── PAGE LOAD ───────────────────────────────────────────────────────────

        // Runs on first GET request — only System Type dropdown needs pre-loading
        // All other dropdowns load via AJAX as user makes selections
        public async Task OnGetAsync()
        {
            await LoadSystemTypesAsync();
        }

        // Fetches all system types from DB and maps them to SelectListItem
        // SelectListItem is what Razor uses to render <option> tags
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

        // ─── POST — SAVE ─────────────────────────────────────────────────────────

        // Called when the Save button is clicked
        // Returns JSON instead of a redirect because we save via AJAX (no page reload)
        // The frontend reads result.status and result.statusDescription to show alerts
        public async Task<IActionResult> OnPostAsync()
        {
            var result = await _mainMenuServices.SaveSubMenuSetupAsync(SubMenuForm);

            return new JsonResult(new
            {
                status = result.Status,
                statusDescription = result.StatusDescription
            });
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

        // Called by JS when user selects a Module
        // Needs both systemId AND moduleId because GetMainMenusAsync filters on both
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

        // Called by JS when user selects a Main Menu
        // Returns all menus under that main menu
        // URL pattern: ?handler=MenusByMainMenu&mainMenuId=3
        public async Task<IActionResult> OnGetMenusByMainMenuAsync(int mainMenuId)
        {
            var menus = await _mainMenuServices.GetMenuSetupAsync(mainMenuId);

            var result = menus.Select(x => new
            {
                value = x.Id,
                text = x.MenuName
            });

            return new JsonResult(result);
        }

        // ─── AJAX — FETCH LIST ───────────────────────────────────────────────────

        // Called by JS when the Fetch button is clicked in the View List panel
        // Returns all submenus matching the selected module + main menu + menu
        // These populate the results table (S/N, SubMenu Code, SubMenu Name, Page URL)
        // URL pattern: ?handler=SubMenuList&moduleId=1&mainMenuId=2&menuId=3
        public async Task<IActionResult> OnGetSubMenuListAsync(int moduleId, int mainMenuId, string menuId)
        {
            var list = await _mainMenuServices.GetSubMenuSetupListAsync(moduleId, mainMenuId, menuId);

            var result = list.Select(x => new
            {
                id = x.Id,
                subMenuCode = x.SubMenuCode,
                subMenuName = x.SubMenuName,
                pageUrl = x.PageUrl,
                orderNo = x.OrderNo
            });

            return new JsonResult(result);
        }

        // ─── AJAX — SELECT SINGLE RECORD ─────────────────────────────────────────

        // Called by JS when the Select button is clicked on a table row
        // Returns the full details of that submenu record
        // JS uses this to fill every field in the form (including checkboxes + cascade)
        // URL pattern: ?handler=SubMenuById&id=5
        public async Task<IActionResult> OnGetSubMenuByIdAsync(int id)
        {
            var item = await _mainMenuServices.GetSubMenuSetupSingleAsync(id);
            return new JsonResult(item);
        }

        // ─── AJAX — DUPLICATE CHECK ──────────────────────────────────────────────────
        // Checks SubMenuCode, SubMenuName, and PageUrl for duplicates.
        // Excludes the record currently being edited (excludeId = 0 for new records).
        // Returns a flags object so the client knows exactly which fields conflict.
        // URL: ?handler=CheckDuplicate&subMenuCode=X&subMenuName=Y&pageUrl=Z&excludeId=0
        public async Task<IActionResult> OnGetCheckDuplicatessAsync(
         string subMenuCode, string subMenuName, string pageUrl, int excludeId = 0)
        {
            // Use the dedicated method — no filter dependencies, always returns all records
            var others = (await _mainMenuServices.GetAllSubMenusAsync())
                .Where(x => x.Id != excludeId)
                .ToList();

            return new JsonResult(new
            {
                subMenuCodeTaken = others.Any(x =>
                    !string.IsNullOrWhiteSpace(subMenuCode) &&
                    string.Equals(x.SubMenuCode?.Trim(), subMenuCode.Trim(), StringComparison.OrdinalIgnoreCase)),

                subMenuNameTaken = others.Any(x =>
                    !string.IsNullOrWhiteSpace(subMenuName) &&
                    string.Equals(x.SubMenuName?.Trim(), subMenuName.Trim(), StringComparison.OrdinalIgnoreCase)),

                pageUrlTaken = others.Any(x =>
                    !string.IsNullOrWhiteSpace(pageUrl) &&
                    string.Equals(x.PageUrl?.Trim(), pageUrl.Trim(), StringComparison.OrdinalIgnoreCase))
            });
        }

        public async Task<IActionResult> OnGetCheckDuplicateAsync(
    string? subMenuCode,
    string? subMenuName,
    string? pageUrl,
    int excludeId = 0)
        {
            var others = (await _mainMenuServices.GetAllSubMenusAsync())
                .Where(x => x.Id != excludeId)
                .ToList();

            var code = subMenuCode?.Trim();
            var name = subMenuName?.Trim();
            var url = pageUrl?.Trim();

            var subMenuCodeTaken =
                !string.IsNullOrWhiteSpace(code) &&
                others.Any(x =>
                    string.Equals(
                        x.SubMenuCode?.Trim(),
                        code,
                        StringComparison.OrdinalIgnoreCase));

            var subMenuNameTaken =
                !string.IsNullOrWhiteSpace(code) &&
                !string.IsNullOrWhiteSpace(name) &&
                others.Any(x =>
                    string.Equals(
                        x.SubMenuCode?.Trim(),
                        code,
                        StringComparison.OrdinalIgnoreCase) &&
                    string.Equals(
                        x.SubMenuName?.Trim(),
                        name,
                        StringComparison.OrdinalIgnoreCase));

            var pageUrlTaken =
                !string.IsNullOrWhiteSpace(code) &&
                !string.IsNullOrWhiteSpace(name) &&
                !string.IsNullOrWhiteSpace(url) &&
                others.Any(x =>
                    string.Equals(
                        x.SubMenuCode?.Trim(),
                        code,
                        StringComparison.OrdinalIgnoreCase) &&
                    string.Equals(
                        x.SubMenuName?.Trim(),
                        name,
                        StringComparison.OrdinalIgnoreCase) &&
                    string.Equals(
                        x.PageUrl?.Trim(),
                        url,
                        StringComparison.OrdinalIgnoreCase));

            return new JsonResult(new
            {
                subMenuCodeTaken,
                subMenuNameTaken,
                pageUrlTaken
            });
        }
    }
}