using BSSLTaskManagement.ServicesInterfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text;
using static BSSLTaskManagement.ViewModels.SystemViewModels;

namespace BSSLTaskManagement.Pages.Setup
{
    public class ClientModel : PageModel
    {
        private readonly ISystemSerivces system;
        private readonly IWebHostEnvironment environment;

        [BindProperty]
        public GeneralCodesVM SystemMenu { get; set; } = new();
        public List<GeneralCodesVM> SystemMenuList { get; set; } = [];
        public ResponseVM ResponseMessage { get; set; } = new ResponseVM();


        public ClientModel(ISystemSerivces system, IWebHostEnvironment environment)
        {
            this.system = system;
            this.environment = environment;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            SystemMenuList = await system.GetClientsAsync();
            return Page();
        }
        // POST handler to save system menus data
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                ResponseMessage = new ResponseVM
                {
                    Status = "Error",
                    StatusDescription = "Validation failed"
                };
                return Page();
            }

            ResponseMessage = await system.SaveClientAsync(SystemMenu);
            ModelState.Clear();
            SystemMenu = new GeneralCodesVM
            {
                Id = null,
                Code = "",
                Description = "",
            };
            SystemMenuList = await system.GetClientsAsync();
            return Page();
        }
    }
}
