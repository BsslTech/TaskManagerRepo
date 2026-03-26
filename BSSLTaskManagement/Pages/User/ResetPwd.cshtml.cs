using BSSLTaskManagement.ServicesInterfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using static BSSLTaskManagement.ViewModels.SystemViewModels;
using static BSSLTaskManagement.ViewModels.UserManagementViewModels;
using static TaskManagement.IdentityLib;

#nullable disable
namespace BSSLTaskManagement.Pages.User
{
    public class ResetPwdModel(UserManager<TaskIdentityUser> userManager, IUserManagementServices managementServices) : PageModel
    {
        private readonly UserManager<TaskIdentityUser> userManager = userManager;
        private readonly IUserManagementServices managementServices = managementServices;

        [BindProperty]
        public ResetPwdVM ResetPwd { get; set; } = new ResetPwdVM();

        public ResponseVM ResponseMessage { get; set; } = new ResponseVM();
        public void OnGet()
        {

        }
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
            ResponseMessage = await managementServices.ResetPasswordAsync(ResetPwd);
            ModelState.Clear();
            ResetPwd = new ResetPwdVM
            {
                UserName = "",
                Name = "",
                Password = "",
                ConfirmPwd = ""
            };
            return Page();
        }
    }
}

