using BSSLTaskManagement.ServicesInterfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using static BSSLTaskManagement.ViewModels.SystemViewModels;

namespace BSSLTaskManagement.Pages.Setup
{
    public class LicenseModel(ISystemSerivces system, IWebHostEnvironment environment) : PageModel
    {
        private readonly ISystemSerivces system = system;
        private readonly IWebHostEnvironment environment = environment;

        [BindProperty]
        public List<SystemTypeVM> SystemTypes { get; set; } = [];
        public List<GeneralCodesVM> ClientNames { get; set; } = [];

        public async Task<IActionResult> OnGetAsync()
        {
            ClientNames =  await system.GetClientsAsync();
            return Page();
        }
    }
}
