
using Microsoft.EntityFrameworkCore;
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
                mainMenus = await context.MainMenu.Where(i => i.ModuleSetupId == moduleId && i.ModuleSetup.SystemTypeTabId == systemType)
                    .Select(p=>new MainMenuSetupVM
                    {
                        Description = p.Description,
                        Id = p.Id,
                        OrderNo = p.OrderNo,
                    }).ToListAsync();
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

                var dbModules = await context.MainMenu.Where(k=>k.ModuleSetupId ==  main.ModuleSetupId && k.ModuleSetup.SystemTypeTabId == main.SystemTypeTabId).ToListAsync();

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
                    }
                    else
                    {
                        var newItem = new MainMenu
                        {
                            Description = item.Description,
                            OrderNo = item.OrderNo,
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
                menus = await context.MenusetupTab.Where(i => i.MainMenuId == mainMenuId)
                    .Select(p => new MenuSetupVM
                    {
                        MenuName = p.MenuName,
                        SavedName = p.MenuName,
                        Id = p.Id,
                        MenuCode = p.MenuCode,
                        OrderNo = p.OrderNo,
                    }).ToListAsync();
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
                var dbTypes = await context.MenusetupTab.ToListAsync();

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
    }
}
