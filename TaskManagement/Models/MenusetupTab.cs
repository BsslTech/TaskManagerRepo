using System;
using System.Collections.Generic;

namespace TaskManagement.Models;

public class MenusetupTab
{
    public int Id { get; set; }

    public string? MenuCode { get; set; }

    public string? MenuName { get; set; }

    public bool Deactivate { get; set; }

    public string? IconName { get; set; }

    public int? MainMenuId { get; set; }

    public string? ModuleName { get; set; }

    public int? OrderNo { get; set; }

    public virtual MainMenu? MainMenu { get; set; }

    public virtual ICollection<SubMenusetupTab> SubMenusetupTabs { get; set; } = new List<SubMenusetupTab>();
}
