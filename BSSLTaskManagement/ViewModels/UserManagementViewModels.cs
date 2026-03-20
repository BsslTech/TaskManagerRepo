using System.ComponentModel.DataAnnotations;
using TaskManagement.Models;

namespace BSSLTaskManagement.ViewModels
{
    public class UserManagementViewModels
    {
        public class RoleVM
        {
            public string? RoleId { get; set; }

            public string? RoleCode { get; set; }

            public string? RoleName { get; set; }
        }
        public class StaffTabVM
        {
            public int? ToDoId { get; set; }
            public int? Id { get; set; }

            public string? StaffId { get; set; }


            [Required(ErrorMessage = "Staff email field is required")]
            [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.(com|org|net)$",
                   ErrorMessage = "Email must be a valid address ending with .com, .org, or .net")]
            public string Email { get; set; }
            public string? OldEmail { get; set; }

            //public string? PhoneNumber { get; set; } = "";

            [Required(ErrorMessage = "Staff name field is required")]
            public string? StaffName { get; set; }

            public string? StaffType { get; set; }
            public string? Suspend { get; set; }
            public string? CreateAccount { get; set; }
            public string UseEmail { get; set; }

            [Required(ErrorMessage = "Username or Email field is required")]
            public string UserName { get; set; }
            [Required(ErrorMessage = "New Password field is required")]
            [StringLength(maximumLength: 10000, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 8)]
            [DataType(DataType.Password)]
            public string Password { get; set; }
            [Required(ErrorMessage = "Confirm password field is required")]
            [DataType(DataType.Password)]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPwd { get; set; }
            [Required(ErrorMessage = "Change password on login is required")]
            public string ChangePwd { get; set; }
        }
        public class StaffTabDetailsVM
        {
            public int? Id { get; set; }

            public string? StaffId { get; set; }
            public string Email { get; set; }
            public string? StaffName { get; set; }

            public string? RoleName { get; set; }
            public string? StaffType { get; set; }
            public string? Suspend { get; set; }
            public string? Status { get; set; }
            public string? CreateAccount { get; set; }
           
        }
        public class ResetPwdVM
        {
            [Required(ErrorMessage = "Username or Email field is required")]
            public string UserName { get; set; }
            [Required(ErrorMessage = "Name of user field is required")]
            public string Name { get; set; }
            [Required(ErrorMessage = "New Password field is required")]
            [StringLength(maximumLength: 10000, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 8)]
            [DataType(DataType.Password)]
            public string Password { get; set; }
            [Required(ErrorMessage = "Confirm password field is required")]
            [DataType(DataType.Password)]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPwd { get; set; }
            [Required(ErrorMessage = "Change password on login is required")]
            public string ChangePwd { get; set; }
        }
        public class ChPwdVM
        {
            public string StaffCode { get; set; }
            public string Role { get; set; }
            public string Email { get; set; }
            public string UserName { get; set; }
            public string Name { get; set; }
            [Required(ErrorMessage = "Current password field is required")]
            public string OldPassword { get; set; }
            [Required(ErrorMessage = "New password field is required")]
            [StringLength(maximumLength: 10000, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 8)]
            [DataType(DataType.Password)]
            public string Password { get; set; }
            [Required(ErrorMessage = "Confirm password field is required")]
            [DataType(DataType.Password)]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        
            public string ConfirmPwd { get; set; }
        }
        public class ChRoleVM
        {
            public string RoleId { get; set; }
            public int? UserTabId { get; set; }
            public string UserId { get; set; }
            public string NameUser { get; set; }
            public string CurRole { get; set; }
            public string NewRole { get; set; }
            public string UserName { get; set; } = "";
            public string UserTabName { get; set; } = "";
        }
        public class UserRoleVM
        {
            public string RoleId { get; set; }
            public string? Id { get; set; } = "";
            public string RoleName { get; set; }
        }
    }
}
