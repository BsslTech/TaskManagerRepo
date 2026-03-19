using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TaskManagement;
using TaskManagement.Models;
using static BSSLTaskManagement.ViewModels.SystemViewModels;
using static BSSLTaskManagement.ViewModels.UserManagementViewModels;
using static Microsoft.CodeAnalysis.CSharp.SyntaxTokenParser;
using static TaskManagement.IdentityLib;

#nullable disable
namespace BSSLTaskManagement.ServicesInterfaces
{
    public interface IUserManagementServices
    {
        Task<ResetPwdVM> GetUserDetailsResetAsync(string user);
        Task<ChPwdVM> GetUserDetailsAsync(string user);
        Task<ResponseVM> ChangePasswordAsync(ChPwdVM chPwd);
        Task<ResponseVM> ResetPasswordAsync(ResetPwdVM reset);
        Task<ChRoleVM> GetUserRoleAsync(string user);
        Task<ResponseVM> ChangeRoleAsync(ChRoleVM role);
        Task<List<UserRoleVM>> GetUserRolesAsync();
        Task<ResponseVM> CreateModifyRoleAsync(UserRoleVM roleVM);
        Task<List<StaffTabVM>> GetStaffDetailsAsync();
        Task<ResponseVM> CreateModifyStaffAsync(StaffTabVM staffDetails);
    }
    public class UserManagementServices(TaskDbContext context,UserManager<TaskIdentityUser> userManager, 
        RoleManager<TaskIdentityRole> roleManager, IWebHostEnvironment environment) : IUserManagementServices
    {
        private readonly TaskDbContext _context = context;
        private readonly UserManager<TaskIdentityUser> _userManager = userManager;
        private readonly RoleManager<TaskIdentityRole> _roleManager = roleManager;
        private readonly IWebHostEnvironment environment = environment;

        public async Task<ChPwdVM> GetUserDetailsAsync(string user)
        {
            var userDetails = await _userManager.FindByNameAsync(user);
            var role = await _roleManager.FindByNameAsync(userDetails.AccountRole);
            string staffCode = userDetails.UserName;
            
            var details = new ChPwdVM
            {
                StaffCode = staffCode,
                UserName = userDetails.UserName,
                Name = userDetails.FullName,
                Role = role.Name,
                Email = userDetails.Email,
                OldPassword = "",
                Password = "",
            };
            if (userDetails.UserName != "Admin")
            {
                var staff = await _context.StaffTab.Where(st => st.Id == userDetails.UserID).FirstOrDefaultAsync();

                details.StaffCode = staff.StaffId;
                details.Name = staff.StaffName;
            }
            return details; 
        }
        public async Task<ResetPwdVM> GetUserDetailsResetAsync(string user)
        {
            var details = new ResetPwdVM();
            var userDetails = await _userManager.FindByNameAsync(user) ?? await _userManager.FindByEmailAsync(user);
            if (userDetails != null)
            {
                var role = await _roleManager.FindByNameAsync(userDetails.AccountRole);
                string staffCode = userDetails.UserName;

                details = new ResetPwdVM
                {
                    UserName = userDetails.UserName,
                    Name = userDetails.FullName,
                    Password = "",
                    ConfirmPwd = "",
                };
                if (userDetails.UserName != "Admin")
                {
                    var staff = await _context.StaffTab.Where(st => st.Id == userDetails.UserID).FirstOrDefaultAsync();

                    details.Name = staff.StaffName;
                }                
            }
            return details;
        }
        public async Task<ResponseVM> ChangePasswordAsync(ChPwdVM chPwd)
        {
            if (string.IsNullOrWhiteSpace(chPwd.UserName) ||
                string.IsNullOrWhiteSpace(chPwd.OldPassword) ||
                string.IsNullOrWhiteSpace(chPwd.Password))
            {
                return new ResponseVM
                {
                    Status = "Failed",
                    StatusDescription = "All fields are required."
                };
            }

            try
            {
                var user = await _userManager.FindByNameAsync(chPwd.UserName)
                          ?? await _userManager.FindByEmailAsync(chPwd.UserName);

                if (user == null)
                    return new ResponseVM { Status = "NotFound", StatusDescription = "User not found." };

                // No need to CheckPasswordAsync
                var result = await _userManager.ChangePasswordAsync(user, chPwd.OldPassword, chPwd.Password);

                if (result.Succeeded)
                {
                    return new ResponseVM
                    {
                        Status = "Success",
                        StatusDescription = "Password successfully changed."
                    };
                }

                return new ResponseVM
                {
                    Status = "Failed",
                    StatusDescription = string.Join(", ", result.Errors.Select(e => e.Description))
                };
            }
            catch (Exception ex)
            {
                return new ResponseVM
                {
                    Status = "Error",
                    StatusDescription = $"Error: {ex.Message}"
                };
            }
        }
        public async Task<ResponseVM> ResetPasswordAsync(ResetPwdVM reset)
        {
            if (string.IsNullOrWhiteSpace(reset.UserName) ||
                string.IsNullOrWhiteSpace(reset.Password))
            {
                return new ResponseVM
                {
                    Status = "Failed",
                    StatusDescription = "Required fields missing."
                };
            }

            try
            {
                var user = await _userManager.FindByNameAsync(reset.UserName)
                          ?? await _userManager.FindByEmailAsync(reset.UserName);

                if (user == null)
                    return new ResponseVM { Status = "NotFound", StatusDescription = "User not found." };

                var token = await _userManager.GeneratePasswordResetTokenAsync(user);

                var result = await _userManager.ResetPasswordAsync(user, token, reset.Password);

                if (result.Succeeded)
                {
                    if (reset.ChangePwd == "1")
                    {
                        // Update custom field
                        user.MustChangePwd = true;
                        await _userManager.UpdateAsync(user);
                    }
                    return new ResponseVM
                    {
                        Status = "Success",
                        StatusDescription = "Password reset successful."
                    };
                }

                return new ResponseVM
                {
                    Status = "Failed",
                    StatusDescription = string.Join(", ", result.Errors.Select(e => e.Description))
                };
            }
            catch (Exception ex)
            {
                return new ResponseVM
                {
                    Status = "Error",
                    StatusDescription = ex.Message
                };
            }
        }
        public async Task<ChRoleVM> GetUserRoleAsync(string userInput)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(userInput)
                          ?? await _userManager.FindByEmailAsync(userInput);

                if (user == null)
                    return new ChRoleVM();

                var roles = await _userManager.GetRolesAsync(user);

                var firstRole = roles.FirstOrDefault();
                if (string.IsNullOrEmpty(firstRole))
                    return new ChRoleVM();

                var roleEntity = await _roleManager.FindByNameAsync(firstRole);

                return new ChRoleVM
                {
                    UserId = user.Id,
                    CurRole = firstRole,
                    RoleId = roleEntity?.Id
                };
            }
            catch
            {
                return new ChRoleVM();
            }
        }
        public async Task<ResponseVM> ChangeRoleAsync(ChRoleVM role)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(role.UserId);
                if (user == null)
                    return new ResponseVM { Status = "NotFound", StatusDescription = "User not found." };

                var newRole = await _roleManager.FindByIdAsync(role.NewRole);
                if (newRole == null)
                    return new ResponseVM { Status = "NotFound", StatusDescription = "Role not found." };

                var currentRoles = await _userManager.GetRolesAsync(user);

                // Remove old roles
                if (currentRoles.Any())
                    await _userManager.RemoveFromRolesAsync(user, currentRoles);

                // Add new role
                var result = await _userManager.AddToRoleAsync(user, newRole.Name);

                if (!result.Succeeded)
                {
                    return new ResponseVM
                    {
                        Status = "Failed",
                        StatusDescription = string.Join(", ", result.Errors.Select(e => e.Description))
                    };
                }

                // Update custom field
                user.AccountRole = newRole.Name;
                await _userManager.UpdateAsync(user);

                return new ResponseVM
                {
                    Status = "Success",
                    StatusDescription = "User role successfully changed."
                };
            }
            catch (Exception ex)
            {
                return new ResponseVM
                {
                    Status = "Error",
                    StatusDescription = ex.Message
                };
            }
        }
        public async Task<List<UserRoleVM>> GetUserRolesAsync()
        {
            try
            {
                return await _context.TaskIdentityRoles
                    .AsNoTracking()
                    .Select(i => new UserRoleVM
                    {
                        Id = i.Id,
                        RoleId = i.RoleId,
                        RoleName = i.Name
                    })
                    .ToListAsync();
            }
            catch
            {
                return new List<UserRoleVM>();
            }
        }
        public async Task<ResponseVM> CreateModifyRoleAsync(UserRoleVM roleVM)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(roleVM.RoleId))
                    return new ResponseVM { Status = "Failed", StatusDescription = "Role code required." };
                if (string.IsNullOrWhiteSpace(roleVM.RoleName))
                    return new ResponseVM { Status = "Failed", StatusDescription = "Role description required." };

                var existing = await _roleManager.FindByIdAsync(roleVM.Id) ?? await _roleManager.FindByNameAsync(roleVM.RoleName);
                existing ??= await _context.TaskIdentityRoles.FirstOrDefaultAsync(i => i.RoleId.ToLower() == roleVM.RoleId.ToLower());

                if (existing == null) // Create
                {
                    var newRole = new TaskIdentityRole
                    {
                        Name = roleVM.RoleName,
                        NormalizedName = roleVM.RoleName.ToUpper(),
                        RoleId = roleVM.RoleId
                    };

                    var result = await _roleManager.CreateAsync(newRole);

                    return new ResponseVM
                    {
                        Status = result.Succeeded ? "Success" : "Failed",
                        StatusDescription = result.Succeeded
                            ? "Role created successfully."
                            : string.Join(", ", result.Errors.Select(e => e.Description))
                    };
                }
                else // Update
                {
                    var oldName = existing.Name;

                    existing.Name = roleVM.RoleName;
                    existing.NormalizedName = roleVM.RoleName.ToUpper();
                    existing.RoleId = roleVM.RoleId;

                    var result = await _roleManager.UpdateAsync(existing);

                    if (result.Succeeded)
                    {
                        // Update users (optional optimization)
                        var users = await _context.TaskIdentityUsers
                            .Where(x => x.AccountRole == oldName)
                            .ToListAsync();

                        users.ForEach(u => u.AccountRole = existing.Name);
                        await _context.SaveChangesAsync();
                    }

                    return new ResponseVM
                    {
                        Status = result.Succeeded ? "Success" : "Failed",
                        StatusDescription = result.Succeeded
                            ? "Role updated successfully."
                            : string.Join(", ", result.Errors.Select(e => e.Description))
                    };
                }
            }
            catch (Exception ex)
            {
                return new ResponseVM
                {
                    Status = "Error",
                    StatusDescription = ex.Message
                };
            }
        }

        public async Task<List<StaffTabVM>> GetAllCreatedStaffDetailsAsync()
        {
            List<StaffTabVM> staffDetails = [];
            try
            {
                staffDetails = await _context.TaskIdentityUsers.AsNoTracking().Select(st => new StaffTabVM
                {Id=st.UserID,
                StaffId = st.Id,
                    StaffName = st.FullName,
                    Email = st.Email,
                    StaffType = st.AccountRole,
                    Suspend = "",
                    CreateAccount = "2",
                }).ToListAsync();

                var roles = await GetUserRolesAsync();
                var rolesDictionary = roles.ToDictionary(x => x.RoleName);
                foreach (var staff in staffDetails)
                {
                    if (rolesDictionary.TryGetValue(staff.StaffType, out var existing))
                    {
                        staff.StaffType = existing.Id;
                    }
                }
            }
            catch
            {
                return new List<StaffTabVM>();
            }
            return staffDetails;
        }
        public async Task<List<StaffTabVM>> GetStaffDetailsAsync()
        {
            List<StaffTabVM> staffDetails = [];
            try
            {
                staffDetails =  await _context.StaffTab.AsNoTracking().Select(st => new StaffTabVM
                {
                    Id = st.Id,
                    StaffId = st.StaffId,
                    StaffName = st.StaffName,
                    Email = st.Email,
                    OldEmail =  st.Email,
                    StaffType = st.StaffType,
                    Suspend = "",
                    CreateAccount = "",
                }).ToListAsync();

                var users =  await GetAllCreatedStaffDetailsAsync();
                var roles =  await GetUserRolesAsync();
                var rolesDictionary = roles.ToDictionary(x => x.RoleName);
                var usersDictionary = users.Where(k=>k.Id != null).ToDictionary(x => x.Id);
                foreach (var staff in staffDetails)
                {
                    if (rolesDictionary.TryGetValue(staff.StaffType, out var existing))
                    {
                        staff.StaffType = existing.Id;
                    }
                    if (usersDictionary.TryGetValue(staff.Id, out var accountCreated))
                    {
                        staff.CreateAccount = accountCreated.CreateAccount;
                    }
                }
            }
            catch(Exception ex)
            {
                return new List<StaffTabVM>();
            }
            return staffDetails;
        }
        public async Task<ResponseVM> CreateModifyStaffAsync(StaffTabVM staffDetails)
        {
            if (string.IsNullOrWhiteSpace(staffDetails.StaffId))
                return new ResponseVM { Status = "Failed", StatusDescription = "Staff ID is required." };

            if (string.IsNullOrWhiteSpace(staffDetails.StaffName))
                return new ResponseVM { Status = "Failed", StatusDescription = "Staff name is required." };

            if (string.IsNullOrWhiteSpace(staffDetails.Email))
                return new ResponseVM { Status = "Failed", StatusDescription = "Email is required." };

            if (string.IsNullOrWhiteSpace(staffDetails.StaffType))
                return new ResponseVM { Status = "Failed", StatusDescription = "Staff role required." };

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // 🔹 Get or create staff
                var staff = await _context.StaffTab
                    .FirstOrDefaultAsync(i => i.Id == staffDetails.Id) ?? new StaffTab();

                bool isNew = staffDetails.Id == null;

                // 🔹 Resolve role
                var roles = await GetUserRolesAsync();
                var role = roles.FirstOrDefault(r => r.Id == staffDetails.StaffType);

                if (role == null)
                {
                    return new ResponseVM
                    {
                        Status = "Failed",
                        StatusDescription = "Invalid staff role."
                    };
                }

                // 🔹 Map staff
                staff.StaffId = staffDetails.StaffId;
                staff.StaffName = staffDetails.StaffName;
                staff.Email = staffDetails.Email;
                staff.StaffType = role.RoleName;

                // 🔹 Save staff FIRST (to get Id)
                if (isNew)
                    _context.StaffTab.Add(staff);
                else
                    _context.StaffTab.Update(staff);

                await _context.SaveChangesAsync();

                // ===========================
                // 🔐 CREATE IDENTITY ACCOUNT
                // ===========================
                if (staffDetails.CreateAccount == "1")
                {
                    // 🔹 Check username
                    var existingUser = await _userManager.FindByNameAsync(staffDetails.UserName);
                    if (existingUser != null)
                    {
                        return new ResponseVM
                        {
                            Status = "Failed",
                            StatusDescription = "Username already exists."
                        };
                    }

                    // 🔹 Ensure role exists
                    
                        if (!await _roleManager.RoleExistsAsync(role.RoleName))
                        {
                            return new ResponseVM
                            {
                                Status = "Failed",
                                StatusDescription = $"Role '{role.RoleName}' is not configured in the system."
                            };
                        }

                    // 🔹 Create user WITH Staff.Id
                    var user = new TaskIdentityUser
                    {
                        UserID = staff.Id, // ✅ THIS is your requirement
                        FullName = staff.StaffName,
                        AccountRole = role.RoleName,
                        Email = staff.Email,
                        UserName = staffDetails.UserName,
                        MustChangePwd = staffDetails.ChangePwd == "1",
                        EmailConfirmed = true,
                        Approved = true,
                    };

                    var createUserResult = await _userManager.CreateAsync(user, staffDetails.Password);

                    if (!createUserResult.Succeeded)
                    {
                        await transaction.RollbackAsync();
                        return new ResponseVM
                        {
                            Status = "Failed",
                            StatusDescription = string.Join(", ", createUserResult.Errors.Select(e => e.Description))
                        };
                    }

                    // 🔹 Assign role
                    var addRoleResult = await _userManager.AddToRoleAsync(user, role.RoleName);

                    if (!addRoleResult.Succeeded)
                    {
                        await transaction.RollbackAsync();
                        return new ResponseVM
                        {
                            Status = "Failed",
                            StatusDescription = string.Join(", ", addRoleResult.Errors.Select(e => e.Description))
                        };
                    }
                }
                else
                {
                    var user = await _userManager.FindByEmailAsync(staffDetails.OldEmail);
                    if (user != null)
                    {
                        // Check duplicate email
                        var existingEmailUser = await _userManager.FindByEmailAsync(staffDetails.Email);
                        if (existingEmailUser != null && existingEmailUser.Id != user.Id)
                        {
                            return new ResponseVM
                            {
                                Status = "Failed",
                                StatusDescription = "Email already exists."
                            };
                        }

                        // Update email
                        var emailResult = await _userManager.SetEmailAsync(user, staffDetails.Email);
                        if (!emailResult.Succeeded)
                        {
                            return new ResponseVM
                            {
                                Status = "Failed",
                                StatusDescription = string.Join(", ", emailResult.Errors.Select(e => e.Description))
                            };
                        }

                        // Sync username if needed
                        await _userManager.SetUserNameAsync(user, staffDetails.Email);
                    }
                }
                // 🔹 Commit everything
                await transaction.CommitAsync();

                return new ResponseVM
                {
                    Status = "Success",
                    StatusDescription = isNew
                        ? "Staff created successfully."
                        : "Staff updated successfully."
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                return new ResponseVM
                {
                    Status = "Error",
                    StatusDescription = ex.Message
                };
            }
        }
    }
}
