using BSSLTaskManagement.ServicesInterfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BSSLTaskManagement.Pages.Menu
{
    // Primary constructor injects the service
    // ISystemSerivces   → GetSystemTypesAsync, SaveSystemTypesAsync

    public class MenuSetupModel
        (ISystemSerivces systemServices) : PageModel
    {
        private readonly ISystemSerivces _systemServices = systemServices;

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
    }


}