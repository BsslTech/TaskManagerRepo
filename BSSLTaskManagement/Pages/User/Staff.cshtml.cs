using BSSLTaskManagement.ServicesInterfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TaskManagement.Models;
using static BSSLTaskManagement.ViewModels.SystemViewModels;
using static BSSLTaskManagement.ViewModels.UserManagementViewModels;

#nullable disable
namespace BSSLTaskManagement.Pages.User
{
    public class StaffModel(IUserManagementServices managementServices) : PageModel
    {
        private readonly IUserManagementServices _managementServices = managementServices;

        [BindProperty]
        public StaffTabVM StaffTab { get; set; } = new();
        public List<StaffTabDetailsVM> StaffList { get; set; } = [];
        public List<UserRoleVM> UserRoles { get; set; } = [];
        public ResponseVM ResponseMessage { get; set; } = new ResponseVM();

        public async Task<IActionResult> OnGetAsync()
        {
            await GetDetails("", StaffTab);
            return Page();
        }
        public async Task GetDetails(string status, StaffTabVM staffTab)
        {
            if (status == "1")
            {
                StaffTab.UseEmail = "1";
                StaffTab.UserName = "Staff@gamil.com";
                StaffTab.Password = "Staff@gamil.com";
                StaffTab.ConfirmPwd = "Staff@gamil.com";
                StaffTab.ChangePwd = "1";
            }
            else
            {
                StaffTab = new StaffTabVM
                {
                    UseEmail = status == "Success" && status == "" ? "1" : staffTab.UseEmail,
                    UserName = status == "Success" && status == "" ? "Staff@gamil.com" : staffTab.UserName,
                    Password = status == "Success" && status == "" ? "Staff@gamil.com" : staffTab.Password,
                    ConfirmPwd = status == "Success" && status == "" ? "Staff@gamil.com" : staffTab.ConfirmPwd,
                    ChangePwd = status == "Success" && status == "" ? "1" : staffTab.ChangePwd,
                };
            }
            UserRoles = await _managementServices.GetUserRolesAsync();
            StaffList = await _managementServices.GetAllStaffDetailsAsync();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            var objectVM = StaffTab; ResponseMessage = new();
            if (!ModelState.IsValid)
            {
                if (objectVM.ToDoId == 1)
                    StaffList = await _managementServices.GetAllStaffDetailsAsync();
                else if (objectVM.ToDoId == 2)
                {
                    var staffList = await _managementServices.GetStaffDetailsAsync();
                    StaffTab = staffList.FirstOrDefault(i => i.Id == objectVM.Id);
                    if (StaffTab != null)
                        StaffList = [];

                    await GetDetails("1", objectVM);
                }
                else
                {
                    ResponseMessage = new ResponseVM
                    {
                        Status = "Error",
                        StatusDescription = "Validation failed"
                    };
                }
                return Page();
            }
            if (objectVM.ToDoId == 0)
            {
                ResponseMessage = await _managementServices.CreateModifyStaffAsync(StaffTab);
            }

            ModelState.Clear();
            await GetDetails(ResponseMessage.Status, objectVM);
            return Page();
        }
    }
}
