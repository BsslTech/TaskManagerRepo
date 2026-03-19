using BSSLTaskManagement.ServicesInterfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TaskManagement.Models;
using static BSSLTaskManagement.ViewModels.SystemViewModels;
using static BSSLTaskManagement.ViewModels.UserManagementViewModels;

namespace BSSLTaskManagement.Pages.User
{
    public class StaffModel(IUserManagementServices managementServices) : PageModel
    {
        private readonly IUserManagementServices _managementServices = managementServices;

        [BindProperty]
        public StaffTabVM StaffTab { get; set; } = new();
        public List<StaffTabVM> StaffList { get; set; } = [];
        public List<UserRoleVM> UserRoles { get; set; } = [];
        public ResponseVM ResponseMessage { get; set; } = new ResponseVM();

        public async Task<IActionResult> OnGetAsync()
        {
            await GetDetails("", StaffTab);
            return Page();
        }
        public async Task GetDetails(string status, StaffTabVM staffTab)
        {
            StaffTab = new StaffTabVM
            {
                UseEmail = status == "Success" && status == "" ? "1" : staffTab.UseEmail,
                UserName = status == "Success" && status == "" ? "Staff@gamil.com" : staffTab.UserName,
                Password = status == "Success" && status == "" ? "Staff@gamil.com" : staffTab.Password,
                ConfirmPwd = status == "Success" && status == "" ? "Staff@gamil.com" : staffTab.ConfirmPwd,
                ChangePwd = status == "Success" && status == "" ? "1" : staffTab.ChangePwd,
            };
            UserRoles = await _managementServices.GetUserRolesAsync();
            StaffList = await _managementServices.GetStaffDetailsAsync();
        }
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
            var objectVM = StaffTab;
            ResponseMessage = await _managementServices.CreateModifyStaffAsync(StaffTab);
            ModelState.Clear();
            await GetDetails(ResponseMessage.Status, objectVM);
            return Page();
        }
    }
}
