using System;
using System.Collections.Generic;

namespace TaskManagement.Models;

public class SubMenusetupTab
{
    public int Id { get; set; }

    public int? MenuId { get; set; }

    public string? SubMenuCode { get; set; }

    public string? SubMenuName { get; set; }

    public string? FormId { get; set; }

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

    public int? ModuleSetupId { get; set; }

    public bool IsApprform { get; set; }

    public string? AccessType { get; set; }

    public string? CompPrefix { get; set; }

    public string? FormNameHeader { get; set; }

    public string? VideoUrl { get; set; }

    public virtual MenusetupTab? Menu { get; set; }

    public virtual ICollection<MenuAccessTab> MenuAccessTabs { get; set; } = new List<MenuAccessTab>();
}
public class SubMenusetupByEntityTab
{
    public int Id { get; set; }
    public int SubMenusetupTabId { get; set; }
    public bool IsActive { get; set; } =false;
    public String ClientCode { get; set; }
    public SubMenusetupTab SubMenusetupTab { get; set; }

}
public class ClientListTab
{
    public int Id { get; set; }
    public String ClientCode { get; set; }
    public String ClientName { get; set; }

}
public class SubsystemListTab
{
    public int Id { get; set; }
    public String SubsystemCode { get; set; }
    public String SubsystemName  { get; set; }

}
