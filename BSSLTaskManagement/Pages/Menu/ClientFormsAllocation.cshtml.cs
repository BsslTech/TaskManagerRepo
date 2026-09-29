using BSSLTaskManagement.ServicesInterfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using static BSSLTaskManagement.ViewModels.MainMenuSetupViewModels;

#nullable disable
namespace BSSLTaskManagement.Pages.Menu
{
    // Primary constructor injects the service
    // IMainMenuSetupServices → GetallClient, GetallSubsystem, GetallModulesBysubSystem,
    //                          GetAllSubMenusAsync(Subsystem, ModuleCode, ClientCode),
    //                          SaveClientFormsAllocation
    public class ClientFormsAllocationModel(IMainMenuSetupServices mainMenuServices) : PageModel
    {
        private readonly IMainMenuSetupServices _mainMenuServices = mainMenuServices;

        // ─── BOUND PROPERTIES ────────────────────────────────────────────────────

        // Powers the Client dropdown on page load
        public List<SelectListItem> ClientList { get; set; } = [];

        // Powers the Subsystem dropdown on page load
        public List<SelectListItem> SubsystemList { get; set; } = [];

        // Client/Subsystem/Module + sub-menu checkbox rows post straight into this
        // via standard model binding — Save is a plain <form method="post">, no JS/fetch.
        [BindProperty]
        public SubMenubyEntitySetupVM AllocationForm { get; set; } = new();

        // ─── PAGE LOAD ───────────────────────────────────────────────────────────

        // Runs on first GET — Client and Subsystem load up-front.
        // Modules and the sub-menu table load via read-only AJAX GETs as the user picks.
        public async Task OnGetAsync()
        {
            await LoadClientsAsync();
            await LoadSubsystemsAsync();
        }

        private async Task LoadClientsAsync()
        {
            var clients = await _mainMenuServices.GetallClient();
            ClientList = clients
                .OrderBy(x => x.ClientName, StringComparer.OrdinalIgnoreCase)
                .Select(x => new SelectListItem
                {
                    Value = x.ClientCode,
                    Text = $"{x.ClientCode} - {x.ClientName}"
                }).ToList();
        }

        private async Task LoadSubsystemsAsync()
        {
            var subsystems = await _mainMenuServices.GetallSubsystem();
            SubsystemList = subsystems
                .Select(x => new SelectListItem
                {
                    Value = x.SystemType,
                    Text = string.IsNullOrWhiteSpace(x.Description) ? x.SystemType : x.Description
                })
                .OrderBy(x => x.Text, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        // ─── AJAX — CASCADE: Subsystem → Modules ─────────────────────────────────

        // Called by JS when user selects a Subsystem. Read-only — no save concerns here.
        // URL pattern: ?handler=ModulesBySubsystem&subsystemCode=X
        public async Task<IActionResult> OnGetModulesBySubsystemAsync(string subsystemCode)
        {
            var modules = await _mainMenuServices.GetallModulesBysubSystem(subsystemCode);

            var result = modules
                .OrderBy(x => x.Description, StringComparer.OrdinalIgnoreCase)
                .Select(x => new
                {
                    value = x.Id,
                    text = x.Description,
                    code = x.ModuleCode
                });

            return new JsonResult(result);
        }

        // ─── AJAX — SUB-MENU LIST FOR THE SELECTED CLIENT/SUBSYSTEM/MODULE ───────

        // Called by JS once Client, Subsystem and Module are all selected. Read-only.
        // URL pattern: ?handler=SubMenus&subsystemCode=X&moduleId=2&clientCode=Y
        public async Task<IActionResult> OnGetSubMenusAsync(string subsystemCode, int moduleId, string clientCode)
        {
            var subMenus = await _mainMenuServices.GetAllSubMenusAsync(subsystemCode, moduleId, clientCode);

            var result = subMenus
                .OrderBy(x => x.SubMenuName, StringComparer.OrdinalIgnoreCase)
                .Select(x => new
                {
                    subMenuId = x.SubMenuId,
                    subMenuName = x.SubMenuName,
                    isActive = x.IsActive
                });

            return new JsonResult(result);
        }

        // ─── POST — SAVE ──────────────────────────────────────────────────────────

        // Plain form submit. AllocationForm is populated by the model binder straight
        // from the posted form fields (client/subsystem selects, the hidden module
        // code field, and the SubMenus[i].SubMenuId / SubMenus[i].IsActive rows) —
        // no JS, no fetch, no manual JSON parsing.
        public async Task<IActionResult> OnPostAsync()
        {
            var result = await _mainMenuServices.SaveClientFormsAllocation(AllocationForm);

            if (result.Status == "Success")
                TempData["Success"] = result.StatusDescription;
            else
                TempData["Error"] = result.StatusDescription;

            return RedirectToPage();
        }
    }
}
