using BSSLTaskManagement.ServicesInterfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using static BSSLTaskManagement.ViewModels.SystemViewModels;
using static BSSLTaskManagement.ViewModels.UserManagementViewModels;

#nullable disable
namespace BSSLTaskManagement.Pages.User
{
    public class RoleManagerModel(IUserManagementServices managementServices) : PageModel
    {
        private readonly IUserManagementServices _managementServices = managementServices;

        [BindProperty]
        public UserRoleVM UserRole { get; set; } = new();
        public List<UserRoleVM> UserRoles { get; set; } = [];
        public ResponseVM ResponseMessage { get; set; } = new ResponseVM();

        public async Task<IActionResult> OnGetAsync()
        {
            UserRoles = await _managementServices.GetUserRolesAsync();
            return Page();
        }
        // POST handler to save system menus data
        public async Task<IActionResult> OnPostAsync()
        {
            UserRoles = await _managementServices.GetUserRolesAsync();
            if (!ModelState.IsValid)
            {
                ResponseMessage = new ResponseVM
                {
                    Status = "Error",
                    StatusDescription = "Validation failed"
                };
                return Page();
            }

            ResponseMessage = await _managementServices.CreateModifyRoleAsync(UserRole);
            ModelState.Clear();
            UserRole = new UserRoleVM
            {
                Id = "",
                RoleId = "",
                RoleName = "",
            };
            return Page();
        }
    }
}
