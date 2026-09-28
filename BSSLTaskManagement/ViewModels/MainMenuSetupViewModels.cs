using System.ComponentModel.DataAnnotations;



#nullable disable
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
        public class SubMenuSetupVM
        {
            public int? Id { get; set; }

            public int? MenuId { get; set; }

            public string? SubMenuCode { get; set; }


            public string? SubMenuName { get; set; }

            //public string? FormId { get; set; }

            public string? PageUrl { get; set; }

            public string? ReportPageUrl { get; set; }

            public bool IsReport { get; set; }

            public bool Deactivate { get; set; }

            public string? IconImagename { get; set; }

            public bool ShowonDashboard { get; set; }

            public bool MakeDashboardMain { get; set; }

            public string? FileName { get; set; }

            public string? FolderPath { get; set; }

            public bool Approval { get; set; }

            public int? OrderNo { get; set; }


            public int? MainMenuId { get; set; }


            public int? SystemId { get; set; }

            public int? ModuleSetupId { get; set; }

            public bool IsApprform { get; set; }


            public string? AccessType { get; set; }

            public string? CompPrefix { get; set; }

            public string? FormNameHeader { get; set; }

            public string? VideoUrl { get; set; }
        }
        public class SubMenuSetupListVM
        {
            public int? Id { get; set; }
            public int? MainId { get; set; }

            public string? SubMenuCode { get; set; }

            public string? SubMenuName { get; set; }

            public string? PageUrl { get; set; }

            public int? OrderNo { get; set; }
        }
        public class SubMenubyEntitySetupListVM
        {
            public int Id { get; set; }
            public int SubMenuId { get; set; }
            public bool IsActive { get; set; }
            public string SubMenuName { get; set; }
        }
        public class SubMenubyEntitySetupVM
        {
            public string EntityCode { get; set; }
            public string ModuleCode { get; set; }
            public string SubSystemCode { get; set; }
            public List<SubMenubyEntitySetupListVM> SubMenus { get; set; } = new List<SubMenubyEntitySetupListVM>();


        }
    }
}
