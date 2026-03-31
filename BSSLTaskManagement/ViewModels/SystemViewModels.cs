#nullable disable
namespace BSSLTaskManagement.ViewModels
{
    using Newtonsoft.Json;
    public class SystemViewModels
    {
        public class GeneralCodesVM
        {
            public int? Id { get; set; }
            public string Code { get; set; }
            public string Description { get; set; }
        }
        public class ResponseVM{
            public string Status { get; set; }
            public string StatusDescription { get; set; }
        }
        public class SystemTypeVM
        {
            public int? SystemId { get; set; }
            public string? Code { get; set; } = "";
            public string? SystemCode { get; set; } = "";
            public string? SystemDescription { get; set; } = "";
            public string? HelperFileName { get; set; } = "";
            [JsonIgnore]
            public IFormFile? HelperFile { get; set; } = null;
        }
        public class  ModuleSetupVM
        {
            public int? SystemId { get; set; }
            public List<ModuleDetailsVM> ModuleDetails { get; set; } = [];
        }
        public class ModuleDetailsVM
        {
            public int? SystemId { get; set; }
            public string? SystemCode { get; set; } = "";
            public string? SystemDescription { get; set; } = "";
            public int? ModuleId { get; set; }
            public string? ModuleCode { get; set; } = "";
            public string? Code { get; set; } = "";
            public string? ModuleDescription { get; set; } = "";
            [JsonIgnore]
            public IFormFile? HelperFile { get; set; } = null;
            public string? HelperFileName { get; set; } = "";
            public string? YouTubeHash { get; set; } = "";
        }
    }
}
