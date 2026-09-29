
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Reflection.PortableExecutable;
using TaskManagement;
using TaskManagement.Models;
using static BSSLTaskManagement.ViewModels.MainMenuSetupViewModels;
using static BSSLTaskManagement.ViewModels.SystemViewModels;


#nullable disable
namespace BSSLTaskManagement.ServicesInterfaces
{
    public interface IMainMenuSetupServices
    {
        Task<List<MainMenuSetupVM>> GetMainMenusAsync(int? systemType, int? moduleId);
        Task<ResponseVM> SaveMainMenusAsync(MainMenuDefVM main);
        Task<List<MenuSetupVM>> GetMenuSetupAsync(int? mainMenuId);
        Task<ResponseVM> SaveMenuSetupAsync(MenuSetupDefVM menus);
        Task<List<SubMenuSetupListVM>> GetSubMenuSetupListAsync(int? moduleId, int? mainMenuId, string menuId);
        Task<SubMenuSetupVM> GetSubMenuSetupSingleAsync(int? id);
        Task<ResponseVM> SaveSubMenuSetupAsync(SubMenuSetupVM submenu);
        Task<List<SubMenuSetupVM>> GetAllSubMenusAsync();
        Task<List<SubMenubyEntitySetupListVM>> GetAllSubMenusAsync(string Subsystem, int ModuleCode, String ClientCode);
        Task<List<ClientTab>> GetallClient();
        Task<List<SystemTypeTab>> GetallSubsystem();
        Task<List<ModuleSetup>> GetallModulesBysubSystem(string subSystemCode);
        Task<ResponseVM> SaveClientFormsAllocation(SubMenubyEntitySetupVM vm);
    }
    public class MainMenuSetupServices(ISystemSerivces system, TaskDbContext context) : IMainMenuSetupServices
    {
        private readonly ISystemSerivces system = system;
        private readonly TaskDbContext context = context;

        public async Task<List<MainMenuSetupVM>> GetMainMenusAsync(int? systemType, int? moduleId)
        {
            List<MainMenuSetupVM> mainMenus = [];
            try
            {
                mainMenus = await context.MainMenu.AsNoTracking().Where(i => i.ModuleSetupId == moduleId && i.ModuleSetup.SystemTypeTabId == systemType)
                    .Select(p => new MainMenuSetupVM
                    {
                        Description = p.Description,
                        Id = p.Id,
                        OrderNo = p.OrderNo,
                    }).OrderBy(i=>i.OrderNo).ToListAsync();
            }
            catch (Exception ex)
            {
                _ = ex.Message.ToString();
                mainMenus = [];
            }
            return mainMenus;


        }
        public async Task<ResponseVM> SaveMainMenusAsync(MainMenuDefVM main)
        {
            if (main.SystemTypeTabId == null)
            {
                return new ResponseVM
                {
                    Status = "Error",
                    StatusDescription = "Select system type"
                };
            }
            else
            {
                var systemTypes = await system.GetSystemTypesAsync();
                if (!systemTypes.Any(x => x.SystemId == main.SystemTypeTabId))
                {
                    return new ResponseVM
                    {
                        Status = "Error",
                        StatusDescription = "Invalid system type selected"
                    };
                }
            }
            if (main.ModuleSetupId == null)
            {
                return new ResponseVM
                {
                    Status = "Error",
                    StatusDescription = "Select module name"
                };
            }
            else
            {
                var modules = await system.GetModulesTypeSystemAsync(main.SystemTypeTabId);
                if (!modules.Any(x => x.ModuleId == main.ModuleSetupId))
                {
                    return new ResponseVM
                    {
                        Status = "Error",
                        StatusDescription = "Invalid module name selected"
                    };
                }
            }
            if (main.MainMenuSetup == null || main.MainMenuSetup.Count == 0)
                return new ResponseVM
                {
                    Status = "Error",
                    StatusDescription = "Main menus missing"
                };


            try
            {

                var dbModules = await context.MainMenu.Where(k => k.ModuleSetupId == main.ModuleSetupId && k.ModuleSetup.SystemTypeTabId == main.SystemTypeTabId).ToListAsync();

                var dbDict = dbModules
                    .ToDictionary(x => x.Description);

                var incomingDict = main.MainMenuSetup
                    .ToDictionary(x => x.SavedDescription, StringComparer.OrdinalIgnoreCase);

                foreach (var item in main.MainMenuSetup)
                {

                    if (dbDict.TryGetValue(item.SavedDescription, out var existing))
                    {
                        existing.Description = item.Description;
                        existing.OrderNo = item.OrderNo;
                        existing.ModuleSetupId = Convert.ToInt32(main.ModuleSetupId);
                    }
                    else
                    {
                        var newItem = new MainMenu
                        {
                            Description = item.Description,
                            OrderNo = item.OrderNo,
                            ModuleSetupId = Convert.ToInt32(main.ModuleSetupId),
                        };

                        context.MainMenu.Add(newItem);
                    }
                }

                // DELETE records not in incoming list
                var toDelete = dbModules
                    .Where(x => !incomingDict.ContainsKey(x.Description))
                    .ToList();



                if (toDelete.Any())
                    context.MainMenu.RemoveRange(toDelete);

                await context.SaveChangesAsync();

                return new ResponseVM
                {
                    Status = "Success",
                    StatusDescription = "Main menu successfully saved"
                };
            }
            catch (Exception ex)
            {
                return new ResponseVM
                {
                    Status = "Error",
                    StatusDescription = $"Main menu not successfully saved: {ex.Message}"
                };
            }
        }

        public async Task<List<MenuSetupVM>> GetMenuSetupAsync(int? mainMenuId)
        {
            List<MenuSetupVM> menus = [];
            try
            {
                menus = await context.MenusetupTab.AsNoTracking().Where(i => i.MainMenuId == mainMenuId)
                    .Select(p => new MenuSetupVM
                    {
                        MenuName = p.MenuName,
                        SavedName = p.MenuName,
                        Id = p.Id,
                        MenuCode = p.MenuCode,
                        OrderNo = p.OrderNo,
                    }).OrderBy(i=>i.OrderNo).ToListAsync();
            }
            catch (Exception ex)
            {
                _ = ex.Message.ToString();
                menus = [];
            }
            return menus;
        }
        public async Task<ResponseVM> SaveMenuSetupAsync(MenuSetupDefVM menus)
        {
            if (menus.SystemTypeTabId == null)
            {
                return new ResponseVM
                {
                    Status = "Error",
                    StatusDescription = "Select system type"
                };
            }
            else
            {
                var systemTypes = await system.GetSystemTypesAsync();
                if (!systemTypes.Any(x => x.SystemId == menus.SystemTypeTabId))
                {
                    return new ResponseVM
                    {
                        Status = "Error",
                        StatusDescription = "Invalid system type selected"
                    };
                }
            }
            if (menus.ModuleSetupId == null)
            {
                return new ResponseVM
                {
                    Status = "Error",
                    StatusDescription = "Select module name"
                };
            }
            else
            {
                var modules = await system.GetModulesTypeSystemAsync(menus.SystemTypeTabId);
                if (!modules.Any(x => x.ModuleId == menus.ModuleSetupId))
                {
                    return new ResponseVM
                    {
                        Status = "Error",
                        StatusDescription = "Invalid module name selected"
                    };
                }
            }
            if (menus.MainMenuSetupId == null)
            {
                return new ResponseVM
                {
                    Status = "Error",
                    StatusDescription = "Select main menu name"
                };
            }
            else
            {
                var mains = await GetMainMenusAsync(menus.SystemTypeTabId, menus.ModuleSetupId);
                if (!mains.Any(x => x.Id == menus.MainMenuSetupId))
                {
                    return new ResponseVM
                    {
                        Status = "Error",
                        StatusDescription = "Invalid main menu name selected"
                    };
                }
            }
            if (menus.MenuSetup == null || menus.MenuSetup.Count == 0)
                return new ResponseVM
                {
                    Status = "Error",
                    StatusDescription = "Menu setup missing"
                };


            try
            {
                var dbTypes = await context.MenusetupTab.Where(k=>k.MainMenuId == menus.MainMenuSetupId).ToListAsync();

                var dbDict = dbTypes
                    .ToDictionary(x => x.MenuName, StringComparer.OrdinalIgnoreCase);

                var incomingDict = menus.MenuSetup
                    .ToDictionary(x => x.SavedName, StringComparer.OrdinalIgnoreCase);

                foreach (var item in menus.MenuSetup)
                {

                    if (dbDict.TryGetValue(item.SavedName, out var existing))
                    {
                        existing.ModuleName = "";
                        existing.MainMenuId = menus.MainMenuSetupId;
                        existing.IconName = "";
                        existing.OrderNo = item.OrderNo;
                        existing.MenuCode = item.MenuCode;
                        existing.MenuName = item.MenuName;
                        existing.Deactivate = false;
                    }
                    else
                    {
                        var newItem = new MenusetupTab
                        {
                            ModuleName = "",
                            MainMenuId = menus.MainMenuSetupId,
                            IconName = "",
                            OrderNo = item.OrderNo,
                            MenuCode = item.MenuCode,
                            MenuName = item.MenuName,
                            Deactivate = false,
                        };

                        context.MenusetupTab.Add(newItem);
                    }
                }

                // DELETE records not in incoming list
                var toDelete = dbTypes
                    .Where(x => !incomingDict.ContainsKey(x.MenuName))
                    .ToList();



                if (toDelete.Any())
                    context.MenusetupTab.RemoveRange(toDelete);

                await context.SaveChangesAsync();

                return new ResponseVM
                {
                    Status = "Success",
                    StatusDescription = "Menus successfully saved"
                };
            }
            catch (Exception ex)
            {
                return new ResponseVM
                {
                    Status = "Error",
                    StatusDescription = $"Menus not successfully saved: {ex.Message}"
                };
            }
        }

        public async Task<List<SubMenuSetupListVM>> GetSubMenuSetupListAsync(int? moduleId, int? mainMenuId, string menuId)
        {
            List<SubMenuSetupListVM> menus = [];
            try
            {
                menus = await context.SubMenusetupTab.AsNoTracking().Where(i => i.MainMenuId == mainMenuId && i.ModuleSetupId == moduleId)
                    .Select(p => new SubMenuSetupListVM
                    {
                        Id = p.Id,
                        MainId = p.MenuId,
                        SubMenuCode = p.SubMenuCode,
                        SubMenuName = p.SubMenuName,
                        PageUrl = p.PageUrl,
                        OrderNo = p.OrderNo,
                    }).ToListAsync();
                if (menuId != "All")
                    menus = menus.Where(i => i.MainId == Convert.ToInt32(menuId)).ToList();
            }
            catch (Exception ex)
            {
                _ = ex.Message.ToString();
                menus = [];
            }
            return menus;
        }
        public async Task<SubMenuSetupVM> GetSubMenuSetupSingleAsync(int? id)
        {
            SubMenuSetupVM subMenu = new();
            try
            {
                subMenu = await context.SubMenusetupTab.AsNoTracking().Where(i => i.Id == id)
                    .Select(p => new SubMenuSetupVM
                    {
                        SystemId = p.Menu.MainMenu.ModuleSetup.SystemTypeTabId,
                        ModuleSetupId = p.ModuleSetupId,
                        MainMenuId = p.MainMenuId,
                        MenuId = p.MenuId,
                        Id = p.Id,
                        SubMenuCode = p.SubMenuCode,
                        SubMenuName = p.SubMenuName,
                        //FormId = p.FormId,
                        PageUrl = p.PageUrl,
                        FormNameHeader = string.IsNullOrWhiteSpace(p.FormNameHeader) ? p.SubMenuName : p.FormNameHeader,
                        OrderNo = p.OrderNo,
                        ReportPageUrl = p.ReportPageUrl,
                        IsApprform = p.IsApprform,
                        IsReport = p.IsReport,
                        AccessType = p.AccessType,
                        Approval = p.Approval,
                        CompPrefix = p.CompPrefix,
                        Deactivate = p.Deactivate,
                        FileName = p.FileName,
                        FolderPath = p.FolderPath,
                        IconImagename = p.IconImagename,
                        MakeDashboardMain = p.MakeDashboardMain,
                        ShowonDashboard = p.ShowonDashboard,
                        VideoUrl = p.VideoUrl,
                    }).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _ = ex.Message.ToString();
                subMenu = new();
            }
            return subMenu;
        }

        public async Task<List<SubMenuSetupVM>> GetAllSubMenusAsync()
        {
            return await context.SubMenusetupTab
                .Select(x => new SubMenuSetupVM
                {
                    Id = x.Id,
                    SubMenuCode = x.SubMenuCode,
                    SubMenuName = x.SubMenuName,
                    PageUrl = x.PageUrl
                })
                .ToListAsync();
        }

        public async Task<ResponseVM> SaveSubMenuSetupAsync(SubMenuSetupVM submenu)
        {
            if (submenu.SystemId == null)
            {
                return new ResponseVM
                {
                    Status = "Error",
                    StatusDescription = "Select system type"
                };
            }
            else
            {
                var systemTypes = await system.GetSystemTypesAsync();
                if (!systemTypes.Any(x => x.SystemId == submenu.SystemId))
                {
                    return new ResponseVM
                    {
                        Status = "Error",
                        StatusDescription = "Invalid system type selected"
                    };
                }
            }
            if (submenu.ModuleSetupId == null)
            {
                return new ResponseVM
                {
                    Status = "Error",
                    StatusDescription = "Select module name"
                };
            }
            else
            {
                var modules = await system.GetModulesTypeSystemAsync(submenu.SystemId);
                if (!modules.Any(x => x.ModuleId == submenu.ModuleSetupId))
                {
                    return new ResponseVM
                    {
                        Status = "Error",
                        StatusDescription = "Invalid module name selected"
                    };
                }
            }
            if (submenu.MainMenuId == null)
            {
                return new ResponseVM
                {
                    Status = "Error",
                    StatusDescription = "Select main menu name"
                };
            }
            else
            {
                var mains = await GetMainMenusAsync(submenu.SystemId, submenu.ModuleSetupId);
                if (!mains.Any(x => x.Id == submenu.MainMenuId))
                {
                    return new ResponseVM
                    {
                        Status = "Error",
                        StatusDescription = "Invalid main menu name selected"
                    };
                }
            }
            if (submenu.MenuId == null)
            {
                return new ResponseVM
                {
                    Status = "Error",
                    StatusDescription = "Select main name"
                };
            }
            else
            {
                var mains = await GetMenuSetupAsync(submenu.MainMenuId);
                if (!mains.Any(x => x.Id == submenu.MenuId))
                {
                    return new ResponseVM
                    {
                        Status = "Error",
                        StatusDescription = "Invalid main name selected"
                    };
                }
            }
            if (string.IsNullOrWhiteSpace(submenu.SubMenuCode))
            {
                return new ResponseVM
                {
                    Status = "Error",
                    StatusDescription = "Enter sub-menu code"
                };
            }
            if (string.IsNullOrWhiteSpace(submenu.SubMenuName))
            {
                return new ResponseVM
                {
                    Status = "Error",
                    StatusDescription = "Enter sub-menu name"
                };
            }
            if (string.IsNullOrWhiteSpace(submenu.AccessType))
            {
                return new ResponseVM
                {
                    Status = "Error",
                    StatusDescription = "Select access type"
                };
            }
            try
            {
                var dbSubmenu = await context.SubMenusetupTab.Where(i=>i.Id == submenu.Id).FirstOrDefaultAsync() ?? new SubMenusetupTab();

                var p = submenu;
                dbSubmenu.ModuleSetupId = p.ModuleSetupId;
                dbSubmenu.MainMenuId = p.MainMenuId;
                dbSubmenu.MenuId = p.MenuId;

                dbSubmenu.SubMenuCode = p.SubMenuCode;
                dbSubmenu.SubMenuName = p.SubMenuName;
                dbSubmenu.FormId = p.SubMenuCode;
                dbSubmenu.PageUrl = p.PageUrl;
                dbSubmenu.FormNameHeader = p.FormNameHeader;
                dbSubmenu.OrderNo = p.OrderNo;
                dbSubmenu.ReportPageUrl = p.ReportPageUrl;
                dbSubmenu.IsApprform = p.IsApprform;
                dbSubmenu.IsReport = p.IsReport;
                dbSubmenu.AccessType = p.AccessType;
                dbSubmenu.Approval = p.Approval;
                dbSubmenu.CompPrefix = string.IsNullOrWhiteSpace(p.CompPrefix) ? "ALL" : p.CompPrefix;
                dbSubmenu.Deactivate = p.Deactivate;
                dbSubmenu.FileName = p.FileName;
                dbSubmenu.FolderPath = p.FolderPath;
                dbSubmenu.IconImagename = p.IconImagename;
                dbSubmenu.MakeDashboardMain = p.MakeDashboardMain;
                dbSubmenu.ShowonDashboard = p.ShowonDashboard;
                dbSubmenu.VideoUrl = p.VideoUrl;
               
                if(p.Id == null)
                context.SubMenusetupTab.Add(dbSubmenu);
                else
                context.SubMenusetupTab.Update(dbSubmenu);

                int succeeded = await context.SaveChangesAsync();

                return new ResponseVM
                {
                    Status = succeeded > 0 ?"Success" : "Failed",
                    StatusDescription = succeeded > 0 ? "Sub-Menu successfully saved" : "Sub-Menu not successfully saved"
                };
            }
            catch (Exception ex)
            {
                return new ResponseVM
                {
                    Status = "Error",
                    StatusDescription = $"Sub-Menu not successfully saved: {ex.Message}"
                };
            }
        }
        public async Task<List<SubMenubyEntitySetupListVM>> GetAllSubMenusAsync(string Subsystem, int ModuleCode, String ClientCode)
        {
            var result =  new List<SubMenubyEntitySetupListVM>();
            try
            {
                var getmodule = await context.ModuleSetup
                    .Where(m => m.SystemType.Trim() == Subsystem && m.Id == ModuleCode)
                    .AsNoTracking().FirstOrDefaultAsync();  

                result = await context.SubMenusetupTab
                    .Where(m => m.ModuleSetupId == ModuleCode  )
                    .Select(x => new SubMenubyEntitySetupListVM
                    {
                        Id = x.Id,
                        SubMenuId = x.Id,
                        IsActive = false,
                        SubMenuName = x.SubMenuName
                    }).AsNoTracking().ToListAsync();

              var getsaved=   await context.SubMenusetupByEntityTab
                    .Where(m=>m.ClientCode.Trim()== ClientCode
                    && m.SubMenusetupTab.ModuleSetupId == ModuleCode )
                    .AsNoTracking().ToListAsync();
                if(getsaved != null && getsaved.Count > 0)
                {
                    foreach (var item in result)
                    {
                        if (getsaved.Any(x => x.SubMenusetupTabId == item.SubMenuId
                        && x.IsActive ==true))
                        {
                            item.IsActive = true;
                        }
                    }
                }   
            }
            catch (Exception ex)
            {
                _ = ex.Message.ToString();
                result = new List<SubMenubyEntitySetupListVM>();
            }
            return result;
        }
        public async Task<List<ClientTab>> GetallClient()
        {
            return await context.ClientTab
                .Select(x => new ClientTab
                {
                    Id = x.Id,
                    ClientCode = x.ClientCode,
                    ClientName = x.ClientName
                }).AsNoTracking().ToListAsync();
        }
        public async Task<List<SystemTypeTab>> GetallSubsystem()
        {
            return await context.SystemTypeTab
                .Select(x => new SystemTypeTab  
                {
                    Id = x.Id,
                    SystemType = x.SystemType,
                    Description = x.Description
                }).AsNoTracking() .ToListAsync();
        }
        public async Task<List<ModuleSetup>> GetallModulesBysubSystem(string subSystemCode)
        {
            return await context.ModuleSetup
                .Where(x => x.SystemType == subSystemCode)
                .Select(x => new ModuleSetup
                {
                    Id = x.Id,
                    ModuleCode = x.ModuleCode,
                    Description = x.Description
                }).AsNoTracking().ToListAsync();
        }
       // C#
       public async Task<ResponseVM> SaveClientFormsAllocation(SubMenubyEntitySetupVM vm)
       {
                if (vm == null)
                {
                    return new ResponseVM { Status = "Error", StatusDescription = "Payload missing" };
                }
                if (string.IsNullOrWhiteSpace(vm.EntityCode))
                {
                    return new ResponseVM { Status = "Error", StatusDescription = "EntityCode required" };
                }

                try
                {
                var getmodule = await context.ModuleSetup
                 .Where(m => m.SystemType.Trim() == vm.SubSystemCode && m.Id == vm.ModuleCode)
                 .AsNoTracking().FirstOrDefaultAsync();
                var clientCode = vm.EntityCode.Trim();
                    var incoming = vm.SubMenus ?? new List<SubMenubyEntitySetupListVM>();
                    var incomingSubMenuIds = incoming.Select(s => s.SubMenuId).ToList();

                    // Load existing allocations for this client
                    var existing = await context.Set<SubMenusetupByEntityTab>()
                        .Where(x => x.ClientCode == clientCode
                        && x.SubMenusetupTabId == vm.ModuleCode)
                        .ToListAsync();

                    var existingBySubMenu = existing.ToDictionary(x => x.SubMenusetupTabId);

                    // Upsert incoming items
                    foreach (var item in incoming)
                    {
                        if (existingBySubMenu.TryGetValue(item.SubMenuId, out var ex))
                        {
                            ex.IsActive = item.IsActive;
                        }
                        else
                        {
                            var newEnt = new SubMenusetupByEntityTab
                            {
                                SubMenusetupTabId = item.SubMenuId,
                                IsActive = item.IsActive,
                                ClientCode = clientCode
                            };
                            context.Set<SubMenusetupByEntityTab>().Add(newEnt);
                        }
                    }

                    // Remove allocations for this client that are not present in incoming list
                    var toRemove = existing.Where(e => !incomingSubMenuIds.Contains(e.SubMenusetupTabId)).ToList();
                    if (toRemove.Any())
                        context.Set<SubMenusetupByEntityTab>().RemoveRange(toRemove);

                    await context.SaveChangesAsync();

                    return new ResponseVM { Status = "Success", StatusDescription = "Client form allocations saved" };
                }
                catch (Exception ex)
                {
                    return new ResponseVM { Status = "Error", StatusDescription = $"Save failed: {ex.Message}" };
                }
            }
    }
}
