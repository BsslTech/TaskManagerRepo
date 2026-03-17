using Microsoft.EntityFrameworkCore;
using TaskManagement;
using TaskManagement.Models;
using static BSSLTaskManagement.ViewModels.SystemViewModels;

#nullable disable
namespace BSSLTaskManagement.ServicesInterfaces
{
    public interface ISystemSerivces
    {
        Task<List<GeneralCodesVM>> GetGeneralCodesAsync(string code);
        Task<List<SystemTypeVM>> GetSystemTypesAsync();
        Task<ResponseVM> SaveSystemTypesAsync(List<SystemTypeVM> systemTypes);
        Task<List<ModuleDetailsVM>> GetModulesAsync();
        Task<List<ModuleDetailsVM>> GetModulesTypeSystemAsync(int? systemId);
        Task<ResponseVM> SaveModuleSetupAsync(ModuleSetupVM module);

        Task<List<GeneralCodesVM>> GetSystemMenusAsync();
        Task<ResponseVM> SaveSystemMenusAsync(GeneralCodesVM menu);
        Task<List<GeneralCodesVM>> GetClientsAsync();
        Task<ResponseVM> SaveClientAsync(GeneralCodesVM menu);
    }
    public class SystemSerivces(TaskDbContext context, IWebHostEnvironment environment) : ISystemSerivces
    {
        private readonly TaskDbContext _context = context;
        private readonly IWebHostEnvironment _environment = environment;

        public async Task<List<GeneralCodesVM>> GetGeneralCodesAsync(string code)
        {
            List<GeneralCodesVM> codes = [];
            try
            {
                string type = code.ToUpper();
                if (type == "SYSTEM TYPES")
                {
                    var systemTypes = await GetSystemTypesAsync();
                    if (systemTypes.Count > 0)
                    {
                        codes = [.. systemTypes.Select(i => new GeneralCodesVM { Id = i.SystemId, Description = i.SystemDescription })];
                    }
                }
                else if (type == "MODULES")
                {
                    var moduleTypes = await GetModulesAsync();
                    if (moduleTypes.Count > 0)
                    {
                        codes = [.. moduleTypes.Select(i => new GeneralCodesVM { Id = i.ModuleId, Description = i.ModuleDescription })];
                    }
                }
            }
            catch (Exception ex)
            {
                _ = ex;
                codes = [];
            }
            return codes;
        }
        public async Task<List<SystemTypeVM>> GetSystemTypesAsync()
        {
            List<SystemTypeVM> systemTypes = [];
            try
            {
                systemTypes = await _context.SystemTypeTab.AsNoTracking()
                    .Select(i => new SystemTypeVM
                    {
                        SystemId = i.Id,
                        Code = i.SystemType,
                        SystemCode = i.SystemType,
                        SystemDescription =  i.Description,
                        HelperFileName = i.SystemTypeFileName,
                        HelperFile = null,
                    }).ToListAsync();
            }
            catch (Exception ex)
            {
                _=ex;
                systemTypes = [];
            }
            return systemTypes;
        }
        public async Task<ResponseVM> SaveSystemTypesAsync(List<SystemTypeVM> systemTypes)
        {
            if (systemTypes == null || systemTypes.Count == 0)
                return new ResponseVM
                {
                    Status = "Error",
                    StatusDescription = "System types missing"
                };

            string folderPath = Path.Combine(_environment.WebRootPath, "HelpFile");

            try
            {
                // Ensure folder exists
                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                var dbTypes = await _context.SystemTypeTab.ToListAsync();

                var dbDict = dbTypes
                    .ToDictionary(x => x.SystemType, StringComparer.OrdinalIgnoreCase);

                var incomingDict = systemTypes
                    .ToDictionary(x => x.Code, StringComparer.OrdinalIgnoreCase);

                foreach (var item in systemTypes)
                {
                    string fileName = item.HelperFileName;
                    string filePath = string.IsNullOrWhiteSpace(fileName) ? "" : Path.Combine(folderPath, fileName);

                    // Handle file upload
                    if (item.HelperFile != null && item.HelperFile.Length > 0)
                    {
                        var extension = Path.GetExtension(item.HelperFile.FileName).ToLower();

                        var allowedExtensions = new[] { ".pdf", ".doc", ".docx" };

                        if (!allowedExtensions.Contains(extension))
                        {
                            return new ResponseVM
                            {
                                Status = "Error",
                                StatusDescription = $"Invalid file type for {item.SystemCode}"
                            };
                        }

                       

                        using var stream = new FileStream(filePath, FileMode.Create);
                        await item.HelperFile.CopyToAsync(stream);
                    }

                    if (dbDict.TryGetValue(item.Code, out var existing))
                    {
                        existing.SystemType = item.SystemCode;
                        existing.Description = item.SystemDescription;
                        existing.SystemTypeFileName = fileName;
                        existing.FolderPath = filePath;
                    }
                    else
                    {
                        var newItem = new SystemTypeTab
                        {
                            SystemType = item.SystemCode,
                            Description = item.SystemDescription,
                            SystemTypeFileName = fileName ?? "",
                            FolderPath = !string.IsNullOrWhiteSpace(fileName) ? filePath : ""
                        };

                        _context.SystemTypeTab.Add(newItem);
                    }
                }

                // DELETE records not in incoming list
                var toDelete = dbTypes
                    .Where(x => !incomingDict.ContainsKey(x.SystemType))
                    .ToList();

                foreach (var item in toDelete)
                {
                    var oldFile = Path.Combine(folderPath, item.SystemTypeFileName ?? "");

                    if (File.Exists(oldFile))
                        File.Delete(oldFile);
                }

                if (toDelete.Any())
                    _context.SystemTypeTab.RemoveRange(toDelete);

                await _context.SaveChangesAsync();

                return new ResponseVM
                {
                    Status = "Success",
                    StatusDescription = "System types successfully saved"
                };
            }
            catch (Exception ex)
            {
                return new ResponseVM
                {
                    Status = "Error",
                    StatusDescription = $"System types not successfully saved: {ex.Message}"
                };
            }
        }
        public async Task<List<ModuleDetailsVM>> GetModulesAsync()
        {
            List<ModuleDetailsVM> moduleList = [];
            try
            {
                moduleList = await _context.ModuleSetup.AsNoTracking()
                    .Select(i => new ModuleDetailsVM
                    {
                        SystemId = i.SystemTypeTabId,
                        SystemCode = i.SystemTypeTab.SystemType,
                        SystemDescription = i.SystemTypeTab.Description,
                        ModuleId = i.Id,
                        ModuleCode = i.ModuleCode,
                        ModuleDescription = i.Description,
                        HelperFileName = i.ModuleFileName,
                        HelperFile = null,
                        YouTubeHash = i.VideoUrl
                    }).ToListAsync();
            }
            catch (Exception ex)
            {
                _ = ex;
                moduleList = [];
            }
            return moduleList;
        }
        public async Task<List<ModuleDetailsVM>> GetModulesTypeSystemAsync(int? systemId)
        {
            List<ModuleDetailsVM> moduleList = [];
            try
            {
                moduleList = await _context.ModuleSetup.AsNoTracking().Where(i=>i.SystemTypeTabId == systemId)
                    .Select(i => new ModuleDetailsVM
                    {
                        SystemId = i.SystemTypeTabId,
                        SystemCode = i.SystemTypeTab.SystemType,
                        SystemDescription = i.SystemTypeTab.Description,
                        ModuleId = i.Id,
                        ModuleCode = i.ModuleCode,
                        ModuleDescription = i.Description,
                        HelperFileName = i.ModuleFileName,
                        HelperFile = null,
                        YouTubeHash = i.VideoUrl
                    }).ToListAsync();
            }
            catch (Exception ex)
            {
                _ = ex;
                moduleList = [];
            }
            return moduleList;
        }

        public async Task<ResponseVM> SaveModuleSetupAsync(ModuleSetupVM module)
        {
            var type = new SystemTypeVM();
            if (module.SystemId ==  null)
                return new ResponseVM
                {
                    Status = "Error",
                    StatusDescription = "Select system type"
                };
            var systemTypes = await GetSystemTypesAsync();
            if (systemTypes.Count > 0)
            {
                type =  systemTypes.FirstOrDefault(k=>k.SystemId == module.SystemId);
                if(type ==  null)
                    return new ResponseVM
                    {
                        Status = "Error",
                        StatusDescription = "System type code not found"
                    };
            }
            else
                return new ResponseVM
                {
                    Status = "Error",
                    StatusDescription = "System types not found"
                };

            if (module.ModuleDetails == null || module.ModuleDetails.Count == 0)
                return new ResponseVM
                {
                    Status = "Error",
                    StatusDescription = "Module types missing"
                };

            string folderPath = Path.Combine(_environment.WebRootPath, "HelpFile");

            try
            {
                // Ensure folder exists
                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                var dbModules = await _context.ModuleSetup.Where(x=>x.SystemTypeTabId == module.SystemId).ToListAsync();

                var dbDict = dbModules
                    .ToDictionary(x => x.ModuleCode, StringComparer.OrdinalIgnoreCase);

                var incomingDict = module.ModuleDetails
                    .ToDictionary(x => x.ModuleCode, StringComparer.OrdinalIgnoreCase);

                foreach (var item in module.ModuleDetails)
                {
                    string newFileName = null;
                    string newFilePath = null;

                    // Handle file upload
                    if (item.HelperFile != null && item.HelperFile.Length > 0)
                    {
                        var extension = Path.GetExtension(item.HelperFile.FileName).ToLower();

                        var allowedExtensions = new[] { ".pdf", ".doc", ".docx" };

                        if (!allowedExtensions.Contains(extension))
                        {
                            return new ResponseVM
                            {
                                Status = "Error",
                                StatusDescription = $"Invalid file type for {item.SystemCode}"
                            };
                        }

                        newFileName = $"{type.SystemCode}{item.ModuleCode}_{DateTime.Now:yyyyMMddHHmmss}{extension}";
                        newFilePath = Path.Combine(folderPath, newFileName);

                        using var stream = new FileStream(newFilePath, FileMode.Create);
                        await item.HelperFile.CopyToAsync(stream);
                    }

                    if (dbDict.TryGetValue(item.ModuleCode, out var existing))
                    {
                        // Delete old file if new file uploaded
                        if (!string.IsNullOrEmpty(newFileName))
                        {
                            var oldFile = Path.Combine(folderPath, existing.ModuleFileName ?? "");

                            if (File.Exists(oldFile))
                                File.Delete(oldFile);

                            existing.ModuleFileName = newFileName;
                            existing.FolderPath = newFilePath;
                        }
                        existing.SystemTypeTabId =(int)module.SystemId;
                        existing.SystemType = type.SystemCode;
                        existing.ModuleCode = item.ModuleCode;
                        existing.Description = item.ModuleDescription;
                        existing.VideoUrl = item.YouTubeHash;
                    }
                    else
                    {
                        var newItem = new ModuleSetup
                        {
                            SystemType = type.SystemCode,
                            SystemTypeTabId = (int)module.SystemId,

                            ModuleCode = item.ModuleCode,
                            Description = item.ModuleDescription,
                            ModuleFileName = newFileName ?? "",
                            FolderPath = newFilePath ?? "",
                            VideoUrl = item.YouTubeHash,
                        };

                        _context.ModuleSetup.Add(newItem);
                    }
                }

                // DELETE records not in incoming list
                var toDelete = dbModules
                    .Where(x => !incomingDict.ContainsKey(x.ModuleCode))
                    .ToList();

                foreach (var item in toDelete)
                {
                    var oldFile = Path.Combine(folderPath, item.ModuleFileName ?? "");

                    if (File.Exists(oldFile))
                        File.Delete(oldFile);
                }

                if (toDelete.Any())
                    _context.ModuleSetup.RemoveRange(toDelete);

                await _context.SaveChangesAsync();

                return new ResponseVM
                {
                    Status = "Success",
                    StatusDescription = "System type modules successfully saved"
                };
            }
            catch (Exception ex)
            {
                return new ResponseVM
                {
                    Status = "Error",
                    StatusDescription = $"System type module not successfully saved: {ex.Message}"
                };
            }
        }

        public async Task<List<GeneralCodesVM>> GetSystemMenusAsync()
        {
            List<GeneralCodesVM> generalCodes = [];
            try
            {
                generalCodes = await _context.SystemMenuTab.AsNoTracking()
                    .Select(i => new GeneralCodesVM
                    {
                        Id = i.Id,
                        Code = i.Code,
                        Description = i.Desc,
                    }).ToListAsync();
            }
            catch (Exception ex)
            {
                _ = ex;
                generalCodes = [];
            }
            return generalCodes;
        }

        public async Task<ResponseVM> SaveSystemMenusAsync(GeneralCodesVM menu)
        {
            if (string.IsNullOrWhiteSpace(menu.Code))
                return new ResponseVM
                {
                    Status = "Error",
                    StatusDescription = "Enter menu code"
                };
            if (string.IsNullOrWhiteSpace(menu.Description))
                return new ResponseVM
                {
                    Status = "Error",
                    StatusDescription = "Enter menu description"
                };
            
            try
            {

                var dbModule = await _context.SystemMenuTab.Where(x => x.Id == menu.Id).FirstOrDefaultAsync() ?? new SystemMenuTab();
                dbModule.Code = menu.Code;
                dbModule.Desc = menu.Description;

                if(menu.Id == null)
                _context.SystemMenuTab.Add(dbModule);
                else
                _context.SystemMenuTab.Update(dbModule);
               
              int succeeded =   await _context.SaveChangesAsync();

                return new ResponseVM
                {
                    Status = succeeded > 0 ?"Success" : "Failed",
                    StatusDescription = succeeded > 0 ? "System menu successfully saved" : "System menu not successfully saved"
                };
            }
            catch (Exception ex)
            {
                return new ResponseVM
                {
                    Status = "Error",
                    StatusDescription = $"System menu not successfully saved: {ex.Message}"
                };
            }
        }

        public async Task<List<GeneralCodesVM>> GetClientsAsync()
        {
            List<GeneralCodesVM> generalCodes = [];
            try
            {
                generalCodes = await _context.ClientTab.AsNoTracking()
                    .Select(i => new GeneralCodesVM
                    {
                        Id = i.Id,
                        Code = i.ClientCode,
                        Description = i.ClientName,
                    }).ToListAsync();
            }
            catch (Exception ex)
            {
                _ = ex;
                generalCodes = [];
            }
            return generalCodes;
        }

        public async Task<ResponseVM> SaveClientAsync(GeneralCodesVM menu)
        {
            if (string.IsNullOrWhiteSpace(menu.Code))
                return new ResponseVM
                {
                    Status = "Error",
                    StatusDescription = "Enter client code"
                };
            if (string.IsNullOrWhiteSpace(menu.Description))
                return new ResponseVM
                {
                    Status = "Error",
                    StatusDescription = "Enter client name"
                };

            try
            {

                var dbModule = await _context.ClientTab.Where(x => x.Id == menu.Id).FirstOrDefaultAsync() ?? new ClientTab();
                dbModule.ClientCode = menu.Code;
                dbModule.ClientName = menu.Description;

                if (menu.Id == null)
                    _context.ClientTab.Add(dbModule);
                else
                    _context.ClientTab.Update(dbModule);

                int succeeded = await _context.SaveChangesAsync();

                return new ResponseVM
                {
                    Status = succeeded > 0 ? "Success" : "Failed",
                    StatusDescription = succeeded > 0 ? "Client name successfully saved" : "Client name not successfully saved"
                };
            }
            catch (Exception ex)
            {
                return new ResponseVM
                {
                    Status = "Error",
                    StatusDescription = $"Client name not successfully saved: {ex.Message}"
                };
            }
        }
    }
}
