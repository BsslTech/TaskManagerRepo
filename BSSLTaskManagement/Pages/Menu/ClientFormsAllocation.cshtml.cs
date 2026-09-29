using BSSLTaskManagement.ServicesInterfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.Json;
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

        // ─── PAGE LOAD ───────────────────────────────────────────────────────────

        // Runs on first GET — Client and Subsystem load up-front.
        // Modules load via AJAX once a Subsystem is picked.
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

        // Called by JS when user selects a Subsystem
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

        // Called by JS once Client, Subsystem and Module are all selected
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

        // ─── AJAX — SAVE ALLOCATIONS ──────────────────────────────────────────────

        // Reads and deserializes the body manually instead of relying on
        // [FromBody] content-type negotiation. Some client machines/proxies
        // strip or rewrite the "Content-Type: application/json" header on the
        // way in, which makes MVC's JSON input formatter silently skip
        // binding — the handler then runs with a null payload. Parsing the
        // raw body ourselves avoids that failure mode entirely.
        public async Task<IActionResult> OnPostSaveAsync()
        {
            SubMenubyEntitySetupVM payload;
            try
            {
                using var reader = new StreamReader(Request.Body);
                var body = await reader.ReadToEndAsync();
                payload = JsonSerializer.Deserialize<SubMenubyEntitySetupVM>(
                    body,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            catch (JsonException)
            {
                return new JsonResult(new { status = "Error", statusDescription = "Could not read the submitted data. Please try saving again." });
            }

            var result = await _mainMenuServices.SaveClientFormsAllocation(payload);

            return new JsonResult(new
            {
                status = result.Status,
                statusDescription = result.StatusDescription
            });
        }
    }
}
