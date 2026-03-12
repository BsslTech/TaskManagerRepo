using System;
using System.Collections.Generic;

namespace BSSLTaskManagement.Models;

public partial class SubMenusetupTab
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
