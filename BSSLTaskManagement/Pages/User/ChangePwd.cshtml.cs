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
    public class ChangePwdModel(UserManager<TaskIdentityUser> userManager, IUserManagementServices managementServices) : PageModel
    {
        private readonly UserManager<TaskIdentityUser> userManager = userManager;
        private readonly IUserManagementServices managementServices = managementServices;

        [BindProperty]
        public ChPwdVM ChPwd { get; set; } = new ChPwdVM();

        public ResponseVM ResponseMessage { get; set; } = new ResponseVM();
        public async Task<IActionResult> OnGetAsync()
        {
            var userDetails = await userManager.GetUserAsync(User);
            ChPwd = await managementServices.GetUserDetailsAsync(userDetails.UserName);
            return Page();
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
            ResponseMessage =  await managementServices.ChangePasswordAsync(ChPwd);
            var user = await userManager.GetUserAsync(User);

            ModelState.Clear();
            ChPwd = await managementServices.GetUserDetailsAsync(user.UserName);
            return Page();
        }
    }
}
