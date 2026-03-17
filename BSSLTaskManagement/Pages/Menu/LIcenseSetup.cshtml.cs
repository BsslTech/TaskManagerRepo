using BSSLTaskManagement.ServicesInterfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using static BSSLTaskManagement.ViewModels.SystemViewModels;

namespace BSSLTaskManagement.Pages.Menu
{
    public class LIcenseSetupModel(ISystemSerivces system, IWebHostEnvironment environment) : PageModel
    {
        private readonly ISystemSerivces system = system;
        private readonly IWebHostEnvironment environment = environment;

        [BindProperty]
        public List<SystemTypeVM> SystemTypes { get; set; } = [];

        public void OnGet()
        {
        }
    }
}
