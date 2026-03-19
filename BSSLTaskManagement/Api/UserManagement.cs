using BSSLTaskManagement.ServicesInterfaces;
using Microsoft.AspNetCore.Mvc;
using static BSSLTaskManagement.ViewModels.UserManagementViewModels;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860
#nullable disable
namespace BSSLTaskManagement.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserManagement(IUserManagementServices managementServices) : ControllerBase
    {
        // GET: api/<UserManagement>
        [HttpGet("GetUserDetails")]
        public async Task<IActionResult> GetUserDetails(string userName)
        {
            var userDetails = await managementServices.GetUserDetailsResetAsync(userName);
            if (userDetails == null)
            {
                return NotFound();
            }
            return Ok(userDetails);
        }
        [HttpGet("GetUserRoleDetails")]
        public async Task<IActionResult> GetUserRoleDetails(string role, string option)
        {
            var roleName = new UserRoleVM();
            var userDetails = await managementServices.GetUserRolesAsync();
            if (userDetails.Count > 0)
            {
                if(option == "1")
                    roleName = userDetails.Where(x => x.RoleId.Equals(role, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault()
                        ?? new UserRoleVM();
                else if(option == "2")
                    roleName = userDetails.Where(x => x.RoleName.Equals(role, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault() ?? new UserRoleVM();
            }
            else return NotFound();
            return Ok(roleName);
        }
        [HttpGet("GetStaffDetails")]
        public async Task<IActionResult> GetStaffDetails(string staffId)
        {
            var roleName = new StaffTabVM();
            var userDetails = await managementServices.GetStaffDetailsAsync();
            if (userDetails.Count > 0)
            {
                    roleName = userDetails.Where(x => x.StaffId.Equals(staffId, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault()
                        ?? new StaffTabVM();
            }
            else return Ok(new StaffTabVM());
            return Ok(roleName);
        }
    }
}
