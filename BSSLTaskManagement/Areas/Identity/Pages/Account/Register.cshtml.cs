// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using TaskManagement;
using static TaskManagement.IdentityLib;
using static BSSLTaskManagement.ViewModels.SystemViewModels;
using BSSLTaskManagement.ServicesInterfaces;
using static BSSLTaskManagement.ViewModels.UserManagementViewModels;

namespace BSSLTaskManagement.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<TaskIdentityUser> _signInManager;
        private readonly UserManager<TaskIdentityUser> _userManager;
        private readonly RoleManager<TaskIdentityRole> roleManager;
        private readonly IUserStore<TaskIdentityUser> _userStore;
        private readonly IUserEmailStore<TaskIdentityUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly TaskDbContext _context;
        private readonly IUserManagementServices _userManagement;

        public RegisterModel( TaskDbContext context, IUserManagementServices userManagement,
            UserManager<TaskIdentityUser> userManager, RoleManager<TaskIdentityRole> roleManager,
            IUserStore<TaskIdentityUser> userStore,
            SignInManager<TaskIdentityUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender)
        {
            _context = context;
            _userManagement = userManagement;
            _userManager = userManager;
            this.roleManager = roleManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public IList<AuthenticationScheme> ExternalLogins { get; set; }
        public ResponseVM Response { get; set; } = new();
        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required (ErrorMessage = "Staff ID is required")]
            public string StaffID { get; set; }
            public string StaffName { get; set; }
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }
            [Required(ErrorMessage = "Username is required")]
            public string UserName { get; set; }
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }


        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                //Checking if staff exist

                var staffDetails = await _userManagement.GetStaffDetailsAsync();
                var staffDetail = staffDetails.FirstOrDefault(st=>st.StaffId == Input.StaffID);
                if (staffDetail ==  null)
                {
                    Response = new ResponseVM
                    {
                        Status = "Failed",
                        StatusDescription = "Staff details not found"
                    };return Page();
                }
                //Checking if email exist
                var usermanager = await _userManagement.CheckEmailUserNameAsync("1", Input.Email);
                if (!string.IsNullOrWhiteSpace(usermanager.Status))
                {
                    Response = new ResponseVM
                    {
                        Status = "Failed",
                        StatusDescription = usermanager.StatusDescription
                    }; return Page();
                }
                //Checking if Username exist
                usermanager = await _userManagement.CheckEmailUserNameAsync("2", Input.UserName);
                if (!string.IsNullOrWhiteSpace(usermanager.Status))
                {
                    Response = new ResponseVM
                    {
                        Status = "Failed",
                        StatusDescription = usermanager.StatusDescription
                    }; return Page();
                }
                var role = await roleManager.FindByIdAsync(staffDetail.StaffType);
                if (role == null)
                {
                    Response = new ResponseVM
                    {
                        Status = "Failed",
                        StatusDescription = "Staff not assign to role"
                    }; return Page();
                }
                var createStaff = new StaffTabVM
                {
                    UserName = Input.UserName,
                    Password = Input.Password,
                    ConfirmPwd = Input.ConfirmPassword,
                    ChangePwd = "0",
                    Id = staffDetail.Id,
                    StaffId = Input.StaffID,
                    Email = Input.Email,
                    OldEmail = Input.Email,
                    StaffName = staffDetail.StaffName,
                    CreateAccount = "1",
                    StaffType = staffDetail.StaffType,
                    Suspend = "0",
                    ToDoId = 1,
                    UseEmail = "2",
                };
                Response = await _userManagement.CreateModifyStaffAsync(createStaff);
               
                if (Response.Status.Equals("SUCCESS", StringComparison.CurrentCultureIgnoreCase))
                {
                    _logger.LogInformation("User created a new account with password.");

                    var user = await _userManager.FindByNameAsync(Input.UserName);
                    if (user == null) 
                    {
                        Response = new ResponseVM
                        {
                            Status = "Failed",
                            StatusDescription = "User creation failed, please contact admin"
                        }; return Page();
                    }
                    var userId = await _userManager.GetUserIdAsync(user);
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    //if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    //{
                    //    return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    //}
                    //else
                    //{
                    //    await _signInManager.SignInAsync(user, isPersistent: false);
                    //    return LocalRedirect(returnUrl);
                    //}
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }

        

        private IUserEmailStore<TaskIdentityUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<TaskIdentityUser>)_userStore;
        }
    }
}
