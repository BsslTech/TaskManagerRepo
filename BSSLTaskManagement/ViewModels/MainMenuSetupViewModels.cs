using TaskManagement.Models;

namespace BSSLTaskManagement.ViewModels
{
    public class MainMenuSetupViewModels
    {
        public class MainMenuDefVM
        {
                public int? SystemTypeTabId { get; set; }
                public int? ModuleSetupId { get; set; }
            public List<MainMenuSetupVM> MainMenuSetup { get; set; } = [];
        }
        public class MainMenuSetupVM
        {
            public int? Id { get; set; }

            public string? SavedDescription { get; set; }
            public string? Description { get; set; }

            public int? OrderNo { get; set; }
        }
        public class MenuSetupDefVM
        {
            public int? SystemTypeTabId { get; set; }
            public int? ModuleSetupId { get; set; }
            public int? MainMenuSetupId { get; set; }
            public List<MenuSetupVM> MenuSetup { get; set; } = [];
        }
        public class MenuSetupVM
        {
            public int? Id { get; set; }

            public string? MenuCode { get; set; }

            public string? SavedName { get; set; }
            public string? MenuName { get; set; }

            public int? OrderNo { get; set; }
        }
    }
}
